using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Netrock.Application.Caching;
using Netrock.Application.Caching.Constants;
using Netrock.Application.Features.Demo;
using Netrock.Application.Features.Demo.Dtos;
using Netrock.Application.Features.FileStorage;
using Netrock.Application.Identity.Constants;
using Netrock.Infrastructure.Features.Authentication.Models;
using Netrock.Shared;

namespace Netrock.Infrastructure.Features.Demo.Services;

/// <summary>
/// Demo service that handles role elevation and short-lived demo account lifecycle.
/// Allows any authenticated user to toggle the Admin role for realistic end-to-end demo previews.
/// SuperAdmin is always rejected.
/// </summary>
internal class DemoService(
    UserManager<ApplicationUser> userManager,
    TimeProvider timeProvider,
    IFileStorageService fileStorageService,
    ICacheService cacheService,
    ILogger<DemoService> logger) : IDemoService
{
    private static readonly TimeSpan DemoAccountLifetime = TimeSpan.FromHours(24);

    /// <inheritdoc />
    public async Task<Result> SetDemoRoleAsync(Guid userId, string role, CancellationToken ct = default)
    {
        if (role == AppRoles.SuperAdmin)
        {
            logger.LogWarning("Demo role elevation rejected: SuperAdmin requested by user {UserId}", userId);
            return Result.Failure(ErrorMessages.Demo.InvalidRole);
        }

        if (!AppRoles.All.Contains(role) || (role != AppRoles.Admin && role != AppRoles.User))
        {
            logger.LogWarning("Demo role elevation rejected: unknown role '{Role}' requested by user {UserId}", role, userId);
            return Result.Failure(ErrorMessages.Demo.InvalidRole);
        }

        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result.Failure(ErrorMessages.Demo.UserNotFound, ErrorType.NotFound);
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        var hasAdmin = currentRoles.Contains(AppRoles.Admin);

        if (role == AppRoles.Admin && !hasAdmin)
        {
            var addResult = await userManager.AddToRoleAsync(user, AppRoles.Admin);
            if (!addResult.Succeeded)
            {
                logger.LogError("Failed to add Admin role for demo user {UserId}: {Errors}",
                    userId, string.Join(", ", addResult.Errors.Select(e => e.Description)));
                return Result.Failure(ErrorMessages.Demo.RoleChangeFailed);
            }

            logger.LogInformation("Demo role elevated to Admin for user {UserId}", userId);
        }
        else if (role == AppRoles.User && hasAdmin)
        {
            var removeResult = await userManager.RemoveFromRoleAsync(user, AppRoles.Admin);
            if (!removeResult.Succeeded)
            {
                logger.LogError("Failed to remove Admin role for demo user {UserId}: {Errors}",
                    userId, string.Join(", ", removeResult.Errors.Select(e => e.Description)));
                return Result.Failure(ErrorMessages.Demo.RoleChangeFailed);
            }

            logger.LogInformation("Demo role de-elevated to User for user {UserId}", userId);
        }
        else
        {
            logger.LogDebug("Demo role unchanged for user {UserId} (already {Role})", userId, role);
        }

        await userManager.UpdateSecurityStampAsync(user);
        await cacheService.RemoveAsync(CacheKeys.SecurityStamp(userId), ct);
        await cacheService.RemoveAsync(CacheKeys.User(userId), ct);

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result<DemoAccountOutput>> CreateDemoAccountAsync(CancellationToken ct = default)
    {
        var utcNow = timeProvider.GetUtcNow();
        var email = $"demo-{Guid.NewGuid():N}"[..14] + "@demo.netrock.app";
        var password = $"Demo!{Guid.NewGuid():N}";

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = "Demo",
            LastName = "User",
            EmailConfirmed = true,
            DemoExpiresAtUtc = utcNow.Add(DemoAccountLifetime)
        };

        var createResult = await userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            logger.LogError("Failed to create demo account: {Errors}",
                string.Join(", ", createResult.Errors.Select(e => e.Description)));
            return Result<DemoAccountOutput>.Failure(ErrorMessages.Demo.DemoAccountCreationFailed);
        }

        var roleResult = await userManager.AddToRoleAsync(user, AppRoles.User);

        if (!roleResult.Succeeded)
        {
            logger.LogError("Failed to assign User role to demo account {UserId}: {Errors}",
                user.Id, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            await userManager.DeleteAsync(user);
            return Result<DemoAccountOutput>.Failure(ErrorMessages.Demo.DemoAccountCreationFailed);
        }

        logger.LogInformation("Created demo account {UserId} ({Email}), expires at {ExpiresAt}",
            user.Id, email, user.DemoExpiresAtUtc);

        return Result<DemoAccountOutput>.Success(new DemoAccountOutput(user.Id, email, password));
    }

    /// <inheritdoc />
    public async Task CleanupDemoAccountAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null || user.DemoExpiresAtUtc is null)
        {
            return;
        }

        if (user.HasAvatar)
        {
            var avatarDeleteResult = await fileStorageService.DeleteAsync($"avatars/{userId}.webp", ct);
            if (!avatarDeleteResult.IsSuccess)
            {
                logger.LogWarning("Failed to delete avatar for demo user {UserId}: {Error}",
                    userId, avatarDeleteResult.Error);
            }
        }

        var deleteResult = await userManager.DeleteAsync(user);

        if (!deleteResult.Succeeded)
        {
            logger.LogError("Failed to delete demo account {UserId}: {Errors}",
                userId, string.Join(", ", deleteResult.Errors.Select(e => e.Description)));
            return;
        }

        await cacheService.RemoveAsync(CacheKeys.User(userId), ct);
        await cacheService.RemoveAsync(CacheKeys.SecurityStamp(userId), ct);

        logger.LogInformation("Cleaned up demo account {UserId}", userId);
    }
}
