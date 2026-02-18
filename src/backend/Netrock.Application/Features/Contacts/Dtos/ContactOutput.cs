using Netrock.Domain.Entities;

namespace Netrock.Application.Features.Contacts.Dtos;

/// <summary>
/// Output representing a single contact.
/// </summary>
/// <param name="Id">The contact's unique identifier.</param>
/// <param name="Name">The contact name.</param>
/// <param name="Email">The contact email.</param>
/// <param name="Company">The company name.</param>
/// <param name="Phone">The phone number.</param>
/// <param name="Status">The pipeline status.</param>
/// <param name="Source">The acquisition source.</param>
/// <param name="Value">The estimated deal value.</param>
/// <param name="Notes">Free-form notes.</param>
/// <param name="IsFavorite">Whether the contact is a favorite.</param>
/// <param name="CreatedAt">When the contact was created.</param>
/// <param name="UpdatedAt">When the contact was last updated, or <c>null</c> if never updated.</param>
public record ContactOutput(
    Guid Id,
    string Name,
    string? Email,
    string? Company,
    string? Phone,
    ContactStatus Status,
    ContactSource Source,
    decimal? Value,
    string? Notes,
    bool IsFavorite,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
