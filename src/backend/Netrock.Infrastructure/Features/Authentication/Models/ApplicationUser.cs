using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Netrock.Infrastructure.Features.Authentication.Models;

/// <summary>
/// Extended Identity user with additional profile fields.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    [MaxLength(255)]
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    [MaxLength(255)]
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the user's biography text.
    /// </summary>
    [MaxLength(1000)]
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets whether the user has an uploaded avatar image stored in file storage.
    /// </summary>
    public bool HasAvatar { get; set; }

    /// <summary>
    /// Gets or sets the expiry time for demo accounts. <c>null</c> for normal users.
    /// When set, indicates this is a temporary demo account that should be cleaned up after expiry.
    /// </summary>
    public DateTimeOffset? DemoExpiresAtUtc { get; set; }
}
