namespace Netrock.Domain.Entities;

/// <summary>
/// Represents a CRM contact in the pipeline with associated status, source, and value.
/// </summary>
public class Contact : BaseEntity
{
    /// <summary>
    /// Gets or sets the contact's first name.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the contact's last name.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Gets or sets the contact's email address.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the company the contact is associated with.
    /// </summary>
    public string? Company { get; set; }

    /// <summary>
    /// Gets or sets the current pipeline status of the contact.
    /// </summary>
    public required ContactStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the source through which the contact was acquired.
    /// </summary>
    public required ContactSource Source { get; set; }

    /// <summary>
    /// Gets or sets the estimated monetary value of the contact.
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Gets or sets any free-text notes about the contact.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the contact's phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who owns this contact.
    /// </summary>
    public required Guid OwnerId { get; set; }
}

/// <summary>
/// Represents the pipeline status of a contact.
/// </summary>
public enum ContactStatus
{
    /// <summary>A new lead that has not been qualified yet.</summary>
    Lead = 0,

    /// <summary>A qualified prospect with potential interest.</summary>
    Prospect = 1,

    /// <summary>An active customer.</summary>
    Customer = 2,

    /// <summary>A customer at risk of leaving.</summary>
    Churning = 3
}

/// <summary>
/// Represents the acquisition source of a contact.
/// </summary>
public enum ContactSource
{
    /// <summary>Acquired through the website.</summary>
    Web = 0,

    /// <summary>Acquired through email outreach.</summary>
    Email = 1,

    /// <summary>Acquired through phone call.</summary>
    Phone = 2,

    /// <summary>Acquired through social media.</summary>
    SocialMedia = 3,

    /// <summary>Acquired through a referral.</summary>
    Referral = 4,

    /// <summary>Acquired through another source.</summary>
    Other = 5
}
