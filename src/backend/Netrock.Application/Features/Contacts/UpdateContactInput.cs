using Netrock.Domain.Entities;

namespace Netrock.Application.Features.Contacts;

/// <summary>
/// Input for updating an existing contact.
/// </summary>
/// <param name="FirstName">The contact's first name.</param>
/// <param name="LastName">The contact's last name.</param>
/// <param name="Email">The contact's email address.</param>
/// <param name="Company">The company the contact is associated with.</param>
/// <param name="Status">The pipeline status of the contact.</param>
/// <param name="Source">The acquisition source of the contact.</param>
/// <param name="Value">The estimated monetary value of the contact.</param>
/// <param name="Notes">Free-text notes about the contact.</param>
/// <param name="Phone">The contact's phone number.</param>
public record UpdateContactInput(
    string FirstName,
    string LastName,
    string Email,
    string? Company,
    ContactStatus Status,
    ContactSource Source,
    decimal Value,
    string? Notes,
    string? Phone
);
