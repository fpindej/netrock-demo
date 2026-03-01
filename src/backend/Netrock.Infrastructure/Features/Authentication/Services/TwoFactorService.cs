using System.Security.Cryptography;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netrock.Application.Caching;
using Netrock.Application.Caching.Constants;
using Netrock.Application.Cookies;
using Netrock.Application.Cookies.Constants;
using Netrock.Application.Features.Audit;
using Netrock.Application.Features.Authentication;
using Netrock.Application.Features.Authentication.Dtos;
using Netrock.Application.Identity;
using Netrock.Infrastructure.Cryptography;
using Netrock.Infrastructure.Features.Authentication.Models;
using Netrock.Infrastructure.Features.Authentication.Options;
using Netrock.Infrastructure.Persistence;
using Netrock.Shared;

namespace Netrock.Infrastructure.Features.Authentication.Services;

/// <summary>
/// Identity-backed implementation of <see cref="ITwoFactorService"/> with TOTP and challenge tokens.
/// </summary>
internal class TwoFactorService(
    UserManager<ApplicationUser> userManager,
    ITokenProvider tokenProvider,
    TimeProvider timeProvider,
    ICookieService cookieService,
    IUserContext userContext,
    ICacheService cacheService,
    IAuditService auditService,
    IOptions<AuthenticationOptions> authenticationOptions,
    ILogger<TwoFactorService> logger,
    NetrockDbContext dbContext) : ITwoFactorService
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan ChallengeLifetime = TimeSpan.FromMinutes(5);

    private readonly AuthenticationOptions.JwtOptions _jwtOptions = authenticationOptions.Value.Jwt;

    /// <inheritdoc />
    public async Task<Result<TwoFactorSetupOutput>> SetupAsync(CancellationToken cancellationToken = default)
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Result<TwoFactorSetupOutput>.Failure(ErrorMessages.Auth.NotAuthenticated, ErrorType.Unauthorized);
        }

        if (await userManager.GetTwoFactorEnabledAsync(user))
        {
            return Result<TwoFactorSetupOutput>.Failure(ErrorMessages.Auth.TwoFactorAlreadyEnabled);
        }

        await userManager.ResetAuthenticatorKeyAsync(user);
        var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);

        if (string.IsNullOrEmpty(unformattedKey))
        {
            return Result<TwoFactorSetupOutput>.Failure(ErrorMessages.Auth.TwoFactorSetupNotStarted);
        }

        var email = await userManager.GetEmailAsync(user) ?? user.UserName ?? "user";
        var authenticatorUri = GenerateQrCodeUri(email, unformattedKey);

        var output = new TwoFactorSetupOutput(
            SharedKey: FormatKey(unformattedKey),
            AuthenticatorUri: authenticatorUri
        );

        return Result<TwoFactorSetupOutput>.Success(output);
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<string>>> VerifySetupAsync(string code, CancellationToken cancellationToken = default)
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Result<IReadOnlyList<string>>.Failure(ErrorMessages.Auth.NotAuthenticated, ErrorType.Unauthorized);
        }

        if (await userManager.GetTwoFactorEnabledAsync(user))
        {
            return Result<IReadOnlyList<string>>.Failure(ErrorMessages.Auth.TwoFactorAlreadyEnabled);
        }

        var isValid = await userManager.VerifyTwoFactorTokenAsync(
            user, userManager.Options.Tokens.AuthenticatorTokenProvider, code);

        if (!isValid)
        {
            return Result<IReadOnlyList<string>>.Failure(ErrorMessages.Auth.TwoFactorCodeInvalid);
        }

        await userManager.SetTwoFactorEnabledAsync(user, true);
        var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

        await InvalidateUserCache(user.Id, cancellationToken);
        await auditService.LogAsync(AuditActions.TwoFactorEnabled, userId: user.Id, ct: cancellationToken);

        logger.LogInformation("User {UserId} enabled two-factor authentication", user.Id);

        return Result<IReadOnlyList<string>>.Success(recoveryCodes?.ToList() ?? []);
    }

    /// <inheritdoc />
    public async Task<Result> DisableAsync(string password, CancellationToken cancellationToken = default)
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Result.Failure(ErrorMessages.Auth.NotAuthenticated, ErrorType.Unauthorized);
        }

        if (!await userManager.GetTwoFactorEnabledAsync(user))
        {
            return Result.Failure(ErrorMessages.Auth.TwoFactorNotEnabled);
        }

        if (!await userManager.CheckPasswordAsync(user, password))
        {
            return Result.Failure(ErrorMessages.Auth.PasswordIncorrect);
        }

        await userManager.SetTwoFactorEnabledAsync(user, false);
        await userManager.ResetAuthenticatorKeyAsync(user);

        await InvalidateUserCache(user.Id, cancellationToken);
        await auditService.LogAsync(AuditActions.TwoFactorDisabled, userId: user.Id, ct: cancellationToken);

        logger.LogInformation("User {UserId} disabled two-factor authentication", user.Id);

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result<AuthenticationOutput>> VerifyAsync(
        string challengeToken, string code, bool isRecoveryCode, bool useCookies,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = HashHelper.Sha256(challengeToken);
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;

        var challenge = await dbContext.TwoFactorChallenges
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.TokenHash == tokenHash && !c.IsUsed, cancellationToken);

        if (challenge is null || challenge.ExpiresAt < utcNow)
        {
            return Result<AuthenticationOutput>.Failure(ErrorMessages.Auth.TwoFactorChallengeInvalid, ErrorType.Unauthorized);
        }

        if (challenge.FailedAttempts >= MaxFailedAttempts)
        {
            challenge.IsUsed = true;
            await dbContext.SaveChangesAsync(cancellationToken);
            return Result<AuthenticationOutput>.Failure(ErrorMessages.Auth.TwoFactorChallengeMaxAttempts, ErrorType.Unauthorized);
        }

        var user = challenge.User;
        if (user is null)
        {
            return Result<AuthenticationOutput>.Failure(ErrorMessages.Auth.UserNotFound, ErrorType.Unauthorized);
        }

        bool isValid;
        if (isRecoveryCode)
        {
            var result = await userManager.RedeemTwoFactorRecoveryCodeAsync(user, code);
            isValid = result.Succeeded;
        }
        else
        {
            isValid = await userManager.VerifyTwoFactorTokenAsync(
                user, userManager.Options.Tokens.AuthenticatorTokenProvider, code);
        }

        if (!isValid)
        {
            challenge.FailedAttempts++;
            await dbContext.SaveChangesAsync(cancellationToken);

            await auditService.LogAsync(AuditActions.TwoFactorFailure, userId: user.Id, ct: cancellationToken);

            return Result<AuthenticationOutput>.Failure(ErrorMessages.Auth.TwoFactorCodeInvalid, ErrorType.Unauthorized);
        }

        // Mark challenge as used
        challenge.IsUsed = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        // Issue tokens
        var accessToken = await tokenProvider.GenerateAccessToken(user);
        var refreshTokenString = tokenProvider.GenerateRefreshToken();

        var refreshLifetime = challenge.RememberMe
            ? _jwtOptions.RefreshToken.PersistentLifetime
            : _jwtOptions.RefreshToken.SessionLifetime;

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = HashHelper.Sha256(refreshTokenString),
            UserId = user.Id,
            CreatedAt = utcNow,
            ExpiredAt = utcNow.Add(refreshLifetime),
            IsUsed = false,
            IsInvalidated = false,
            IsPersistent = challenge.RememberMe
        };

        dbContext.RefreshTokens.Add(refreshTokenEntity);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (useCookies)
        {
            var now = timeProvider.GetUtcNow();
            cookieService.SetSecureCookie(
                key: CookieNames.AccessToken,
                value: accessToken,
                expires: challenge.RememberMe ? now.Add(_jwtOptions.AccessTokenLifetime) : null);

            cookieService.SetSecureCookie(
                key: CookieNames.RefreshToken,
                value: refreshTokenString,
                expires: challenge.RememberMe ? now.Add(refreshLifetime) : null);
        }

        await auditService.LogAsync(AuditActions.TwoFactorSuccess, userId: user.Id, ct: cancellationToken);

        return Result<AuthenticationOutput>.Success(new AuthenticationOutput(
            AccessToken: accessToken,
            RefreshToken: refreshTokenString
        ));
    }

    /// <inheritdoc />
    public async Task<Result<int>> GetRecoveryCodeCountAsync(CancellationToken cancellationToken = default)
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Result<int>.Failure(ErrorMessages.Auth.NotAuthenticated, ErrorType.Unauthorized);
        }

        if (!await userManager.GetTwoFactorEnabledAsync(user))
        {
            return Result<int>.Failure(ErrorMessages.Auth.TwoFactorNotEnabled);
        }

        var count = await userManager.CountRecoveryCodesAsync(user);
        return Result<int>.Success(count);
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<string>>> RegenerateRecoveryCodesAsync(string password, CancellationToken cancellationToken = default)
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Result<IReadOnlyList<string>>.Failure(ErrorMessages.Auth.NotAuthenticated, ErrorType.Unauthorized);
        }

        if (!await userManager.GetTwoFactorEnabledAsync(user))
        {
            return Result<IReadOnlyList<string>>.Failure(ErrorMessages.Auth.TwoFactorNotEnabled);
        }

        if (!await userManager.CheckPasswordAsync(user, password))
        {
            return Result<IReadOnlyList<string>>.Failure(ErrorMessages.Auth.PasswordIncorrect);
        }

        var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

        logger.LogInformation("User {UserId} regenerated two-factor recovery codes", user.Id);

        return Result<IReadOnlyList<string>>.Success(recoveryCodes?.ToList() ?? []);
    }

    /// <summary>
    /// Creates a new 2FA challenge token for the given user and stores its hash in the database.
    /// Called by <see cref="AuthenticationService"/> when a user with 2FA enabled logs in.
    /// </summary>
    internal async Task<string> CreateChallengeAsync(Guid userId, bool rememberMe, CancellationToken cancellationToken = default)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var tokenHash = HashHelper.Sha256(rawToken);
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;

        var challenge = new TwoFactorChallenge
        {
            Id = Guid.NewGuid(),
            TokenHash = tokenHash,
            UserId = userId,
            CreatedAt = utcNow,
            ExpiresAt = utcNow.Add(ChallengeLifetime),
            FailedAttempts = 0,
            IsUsed = false,
            RememberMe = rememberMe
        };

        dbContext.TwoFactorChallenges.Add(challenge);
        await dbContext.SaveChangesAsync(cancellationToken);

        return rawToken;
    }

    private async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        var userId = userContext.UserId;
        if (!userId.HasValue) return null;
        return await userManager.FindByIdAsync(userId.Value.ToString());
    }

    private async Task InvalidateUserCache(Guid userId, CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync(CacheKeys.User(userId), cancellationToken);
        await cacheService.RemoveAsync(CacheKeys.SecurityStamp(userId), cancellationToken);
    }

    private static string FormatKey(string unformattedKey)
    {
        var result = new char[unformattedKey.Length + (unformattedKey.Length - 1) / 4];
        var resultIndex = 0;
        for (var i = 0; i < unformattedKey.Length; i++)
        {
            if (i > 0 && i % 4 == 0)
            {
                result[resultIndex++] = ' ';
            }
            result[resultIndex++] = unformattedKey[i];
        }
        return new string(result, 0, resultIndex);
    }

    private static string GenerateQrCodeUri(string email, string unformattedKey)
    {
        return $"otpauth://totp/{UrlEncoder.Default.Encode("NETrock")}:{UrlEncoder.Default.Encode(email)}" +
               $"?secret={unformattedKey}&issuer={UrlEncoder.Default.Encode("NETrock")}&digits=6";
    }
}
