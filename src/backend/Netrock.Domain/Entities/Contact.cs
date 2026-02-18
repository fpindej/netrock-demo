namespace Netrock.Domain.Entities;

/// <summary>
/// Represents a user-owned contact in the sales pipeline with status tracking and deal value.
/// </summary>
public class Contact : BaseEntity
{
    /// <summary>
    /// Gets the full name of the contact.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the email address of the contact.
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// Gets the company name associated with the contact.
    /// </summary>
    public string? Company { get; private set; }

    /// <summary>
    /// Gets the phone number of the contact.
    /// </summary>
    public string? Phone { get; private set; }

    /// <summary>
    /// Gets the pipeline status of the contact.
    /// </summary>
    public ContactStatus Status { get; private set; }

    /// <summary>
    /// Gets the acquisition source of the contact.
    /// </summary>
    public ContactSource Source { get; private set; }

    /// <summary>
    /// Gets the estimated deal value for this contact.
    /// </summary>
    public decimal? Value { get; private set; }

    /// <summary>
    /// Gets free-form notes about the contact.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the contact is marked as a favorite.
    /// </summary>
    public bool IsFavorite { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who owns this contact.
    /// </summary>
    public Guid UserId { get; private init; }

    /// <summary>
    /// Protected constructor for EF Core.
    /// </summary>
    protected Contact()
    {
    }

    /// <summary>
    /// Creates a new contact for the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="name">The contact name.</param>
    /// <param name="email">The contact email.</param>
    /// <param name="company">The company name.</param>
    /// <param name="phone">The phone number.</param>
    /// <param name="status">The pipeline status.</param>
    /// <param name="source">The acquisition source.</param>
    /// <param name="value">The estimated deal value.</param>
    /// <param name="notes">Free-form notes.</param>
    public Contact(
        Guid userId,
        string name,
        string? email,
        string? company,
        string? phone,
        ContactStatus status,
        ContactSource source,
        decimal? value,
        string? notes)
    {
        UserId = userId;
        Name = name;
        Email = email;
        Company = company;
        Phone = phone;
        Status = status;
        Source = source;
        Value = value;
        Notes = notes;
    }

    /// <summary>
    /// Updates the contact's mutable properties.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <param name="email">The new email.</param>
    /// <param name="company">The new company.</param>
    /// <param name="phone">The new phone.</param>
    /// <param name="status">The new status.</param>
    /// <param name="source">The new source.</param>
    /// <param name="value">The new deal value.</param>
    /// <param name="notes">The new notes.</param>
    /// <param name="isFavorite">Whether the contact is a favorite.</param>
    public void Update(
        string name,
        string? email,
        string? company,
        string? phone,
        ContactStatus status,
        ContactSource source,
        decimal? value,
        string? notes,
        bool isFavorite)
    {
        Name = name;
        Email = email;
        Company = company;
        Phone = phone;
        Status = status;
        Source = source;
        Value = value;
        Notes = notes;
        IsFavorite = isFavorite;
    }
}
