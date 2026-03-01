using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Authentication.Dtos.TwoFactor;

/// <summary>
/// Request to regenerate 2FA recovery codes. Requires password confirmation.
/// </summary>
public class TwoFactorRegenerateCodesRequest
{
    /// <summary>
    /// The user's current password for confirmation.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Password { get; [UsedImplicitly] init; } = string.Empty;
}
