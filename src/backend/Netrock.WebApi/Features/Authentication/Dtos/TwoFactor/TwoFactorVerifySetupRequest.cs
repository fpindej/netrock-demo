using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Authentication.Dtos.TwoFactor;

/// <summary>
/// Request to verify a TOTP code and complete 2FA setup.
/// </summary>
public class TwoFactorVerifySetupRequest
{
    /// <summary>
    /// The 6-digit TOTP code from the authenticator app.
    /// </summary>
    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string Code { get; [UsedImplicitly] init; } = string.Empty;
}
