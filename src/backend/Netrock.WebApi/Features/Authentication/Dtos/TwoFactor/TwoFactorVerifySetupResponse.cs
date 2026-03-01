using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Authentication.Dtos.TwoFactor;

/// <summary>
/// Response containing recovery codes after completing 2FA setup or regeneration.
/// </summary>
public class TwoFactorVerifySetupResponse
{
    /// <summary>
    /// The generated recovery codes.
    /// </summary>
    public IReadOnlyList<string> RecoveryCodes { [UsedImplicitly] get; [UsedImplicitly] init; } = [];
}
