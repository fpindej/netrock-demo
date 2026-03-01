namespace Netrock.Application.Features.Authentication.Dtos;

/// <summary>
/// Output containing the TOTP shared key and provisioning URI for 2FA setup.
/// </summary>
/// <param name="SharedKey">The base-32 shared key for manual entry into an authenticator app.</param>
/// <param name="AuthenticatorUri">The otpauth:// URI for QR code generation.</param>
public record TwoFactorSetupOutput(
    string SharedKey,
    string AuthenticatorUri
);
