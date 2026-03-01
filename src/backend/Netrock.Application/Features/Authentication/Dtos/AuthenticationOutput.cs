namespace Netrock.Application.Features.Authentication.Dtos;

/// <summary>
/// Output containing authentication tokens, or a 2FA challenge when two-factor authentication is required.
/// </summary>
/// <param name="AccessToken">The JWT access token for API authentication. Empty when 2FA is required.</param>
/// <param name="RefreshToken">The refresh token for obtaining new access tokens. Empty when 2FA is required.</param>
/// <param name="RequiresTwoFactor">Whether the user must complete a 2FA challenge before receiving tokens.</param>
/// <param name="ChallengeToken">The opaque challenge token for 2FA verification. Null when 2FA is not required.</param>
public record AuthenticationOutput(
    string AccessToken,
    string RefreshToken,
    bool RequiresTwoFactor = false,
    string? ChallengeToken = null
);
