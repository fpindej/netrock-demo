using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Authentication.Dtos.Login;

/// <summary>
/// Response containing authentication tokens for API clients, or a 2FA challenge.
/// Web clients can ignore the token fields as tokens are also set in HttpOnly cookies.
/// </summary>
public class AuthenticationResponse
{
    /// <summary>
    /// The JWT access token for Bearer authentication.
    /// Include this in the Authorization header as "Bearer {accessToken}" for subsequent API requests.
    /// Empty when <see cref="RequiresTwoFactor"/> is true.
    /// </summary>
    public string AccessToken { [UsedImplicitly] get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// The refresh token for obtaining new access tokens.
    /// Use this with the /api/auth/refresh endpoint when the access token expires.
    /// Empty when <see cref="RequiresTwoFactor"/> is true.
    /// </summary>
    public string RefreshToken { [UsedImplicitly] get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// When true, the user must complete a two-factor authentication challenge before tokens are issued.
    /// Use <see cref="ChallengeToken"/> with the 2FA verify endpoint.
    /// </summary>
    public bool RequiresTwoFactor { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>
    /// The challenge token for 2FA verification. Only present when <see cref="RequiresTwoFactor"/> is true.
    /// </summary>
    public string? ChallengeToken { [UsedImplicitly] get; [UsedImplicitly] init; }
}
