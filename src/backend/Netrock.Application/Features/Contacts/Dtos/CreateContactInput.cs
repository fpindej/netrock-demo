using Netrock.Domain.Entities;

namespace Netrock.Application.Features.Contacts.Dtos;

/// <summary>
/// Input for creating a new contact.
/// </summary>
/// <param name="Name">The contact name.</param>
/// <param name="Email">The contact email.</param>
/// <param name="Company">The company name.</param>
/// <param name="Phone">The phone number.</param>
/// <param name="Status">The pipeline status.</param>
/// <param name="Source">The acquisition source.</param>
/// <param name="Value">The estimated deal value.</param>
/// <param name="Notes">Free-form notes.</param>
public record CreateContactInput(
    string Name,
    string? Email,
    string? Company,
    string? Phone,
    ContactStatus Status,
    ContactSource Source,
    decimal? Value,
    string? Notes);
