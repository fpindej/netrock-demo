using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Authentication.Dtos.TwoFactor;

/// <summary>
/// Request to disable 2FA on the current user's account.
/// </summary>
public class TwoFactorDisableRequest
{
    /// <summary>
    /// The user's current password for confirmation.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Password { get; [UsedImplicitly] init; } = string.Empty;
}
