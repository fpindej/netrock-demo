using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Authentication.Dtos.TwoFactor;

/// <summary>
/// Response containing the TOTP shared key and provisioning URI for 2FA setup.
/// </summary>
public class TwoFactorSetupResponse
{
    /// <summary>
    /// The base-32 shared key formatted for display (spaces every 4 characters).
    /// </summary>
    public string SharedKey { [UsedImplicitly] get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// The otpauth:// URI for generating a QR code in the client.
    /// </summary>
    public string AuthenticatorUri { [UsedImplicitly] get; [UsedImplicitly] init; } = string.Empty;
}
