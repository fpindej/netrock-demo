using Netrock.Application.Features.Authentication.Dtos;
using Netrock.Shared;

namespace Netrock.Application.Features.Authentication;

/// <summary>
/// Provides two-factor authentication operations including setup, verification, and recovery.
/// </summary>
public interface ITwoFactorService
{
    /// <summary>
    /// Initiates 2FA setup by generating a TOTP shared key and provisioning URI for the current user.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A result containing the shared key and authenticator URI for QR code generation.</returns>
    Task<Result<TwoFactorSetupOutput>> SetupAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a TOTP code and enables 2FA on the current user's account.
    /// Returns the generated recovery codes.
    /// </summary>
    /// <param name="code">The 6-digit TOTP code from the authenticator app.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A result containing the recovery codes on success.</returns>
    Task<Result<IReadOnlyList<string>>> VerifySetupAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables 2FA on the current user's account after verifying the password.
    /// </summary>
    /// <param name="password">The user's current password for confirmation.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    Task<Result> DisableAsync(string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a 2FA code (TOTP or recovery code) against a challenge token and returns authentication tokens.
    /// </summary>
    /// <param name="challengeToken">The challenge token received during login.</param>
    /// <param name="code">The TOTP code or recovery code.</param>
    /// <param name="isRecoveryCode">Whether the code is a recovery code rather than a TOTP code.</param>
    /// <param name="useCookies">Whether to set authentication cookies.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A result containing authentication tokens on success.</returns>
    Task<Result<AuthenticationOutput>> VerifyAsync(string challengeToken, string code, bool isRecoveryCode, bool useCookies, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the remaining recovery codes for the current user.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A result containing the count of remaining recovery codes.</returns>
    Task<Result<int>> GetRecoveryCodeCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Regenerates recovery codes for the current user after verifying the password.
    /// </summary>
    /// <param name="password">The user's current password for confirmation.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A result containing the new recovery codes on success.</returns>
    Task<Result<IReadOnlyList<string>>> RegenerateRecoveryCodesAsync(string password, CancellationToken cancellationToken = default);
}
