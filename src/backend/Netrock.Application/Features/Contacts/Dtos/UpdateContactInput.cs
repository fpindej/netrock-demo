using Netrock.Domain.Entities;

namespace Netrock.Application.Features.Contacts.Dtos;

/// <summary>
/// Input for updating an existing contact.
/// </summary>
/// <param name="Name">The new name.</param>
/// <param name="Email">The new email.</param>
/// <param name="Company">The new company.</param>
/// <param name="Phone">The new phone number.</param>
/// <param name="Status">The new pipeline status.</param>
/// <param name="Source">The new acquisition source.</param>
/// <param name="Value">The new deal value.</param>
/// <param name="Notes">The new notes.</param>
/// <param name="IsFavorite">Whether the contact is a favorite.</param>
public record UpdateContactInput(
    string Name,
    string? Email,
    string? Company,
    string? Phone,
    ContactStatus Status,
    ContactSource Source,
    decimal? Value,
    string? Notes,
    bool IsFavorite);
