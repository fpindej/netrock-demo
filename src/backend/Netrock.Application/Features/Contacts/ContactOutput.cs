using Netrock.Domain.Entities;

namespace Netrock.Application.Features.Contacts;

/// <summary>
/// Output representing a single contact's details.
/// </summary>
/// <param name="Id">The contact's unique identifier.</param>
/// <param name="FirstName">The contact's first name.</param>
/// <param name="LastName">The contact's last name.</param>
/// <param name="Email">The contact's email address.</param>
/// <param name="Company">The company the contact is associated with, or <c>null</c> if not set.</param>
/// <param name="Status">The current pipeline status of the contact.</param>
/// <param name="Source">The acquisition source of the contact.</param>
/// <param name="Value">The estimated monetary value of the contact.</param>
/// <param name="Notes">Free-text notes about the contact, or <c>null</c> if not set.</param>
/// <param name="Phone">The contact's phone number, or <c>null</c> if not set.</param>
/// <param name="OwnerId">The identifier of the user who owns this contact.</param>
/// <param name="CreatedAt">The date and time when the contact was created.</param>
/// <param name="UpdatedAt">The date and time when the contact was last updated, or <c>null</c> if never updated.</param>
public record ContactOutput(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Company,
    ContactStatus Status,
    ContactSource Source,
    decimal Value,
    string? Notes,
    string? Phone,
    Guid OwnerId,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
