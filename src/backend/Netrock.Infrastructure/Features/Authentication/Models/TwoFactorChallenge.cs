namespace Netrock.Infrastructure.Features.Authentication.Models;

/// <summary>
/// Represents a two-factor authentication challenge issued during login.
/// The challenge token is SHA-256 hashed before storage and has a limited TTL
/// and maximum number of verification attempts.
/// </summary>
public class TwoFactorChallenge
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the SHA-256 hash of the challenge token sent to the client.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the user who must complete the challenge.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the owning user.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the challenge was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the challenge expires.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the number of failed verification attempts against this challenge.
    /// </summary>
    public int FailedAttempts { get; set; }

    /// <summary>
    /// Gets or sets whether the challenge has been consumed by a successful verification.
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>
    /// Gets or sets whether the user selected "remember me" during login.
    /// Carried forward to set persistent cookies after 2FA verification.
    /// </summary>
    public bool RememberMe { get; set; }
}
