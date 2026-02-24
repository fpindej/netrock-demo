using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Represents a contact's details in API responses.
/// </summary>
public class ContactResponse
{
    /// <summary>
    /// The unique identifier of the contact.
    /// </summary>
    public Guid Id { [UsedImplicitly] get; init; }

    /// <summary>
    /// The contact's first name.
    /// </summary>
    public string FirstName { [UsedImplicitly] get; init; } = string.Empty;

    /// <summary>
    /// The contact's last name.
    /// </summary>
    public string LastName { [UsedImplicitly] get; init; } = string.Empty;

    /// <summary>
    /// The contact's email address.
    /// </summary>
    public string Email { [UsedImplicitly] get; init; } = string.Empty;

    /// <summary>
    /// The company the contact is associated with.
    /// </summary>
    public string? Company { [UsedImplicitly] get; init; }

    /// <summary>
    /// The pipeline status of the contact.
    /// </summary>
    public string Status { [UsedImplicitly] get; init; } = string.Empty;

    /// <summary>
    /// The acquisition source of the contact.
    /// </summary>
    public string Source { [UsedImplicitly] get; init; } = string.Empty;

    /// <summary>
    /// The estimated monetary value of the contact.
    /// </summary>
    public decimal Value { [UsedImplicitly] get; init; }

    /// <summary>
    /// Free-text notes about the contact.
    /// </summary>
    public string? Notes { [UsedImplicitly] get; init; }

    /// <summary>
    /// The contact's phone number.
    /// </summary>
    public string? Phone { [UsedImplicitly] get; init; }

    /// <summary>
    /// The identifier of the user who owns this contact.
    /// </summary>
    public Guid OwnerId { [UsedImplicitly] get; init; }

    /// <summary>
    /// The date and time when the contact was created.
    /// </summary>
    public DateTime CreatedAt { [UsedImplicitly] get; init; }

    /// <summary>
    /// The date and time when the contact was last updated.
    /// </summary>
    public DateTime? UpdatedAt { [UsedImplicitly] get; init; }
}
