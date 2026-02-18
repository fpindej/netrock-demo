using JetBrains.Annotations;
using Netrock.Domain.Entities;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Represents a request to create a new contact.
/// </summary>
[UsedImplicitly]
public class CreateContactRequest
{
    /// <summary>The contact name.</summary>
    public string Name { get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>The contact email.</summary>
    public string? Email { get; [UsedImplicitly] init; }

    /// <summary>The company name.</summary>
    public string? Company { get; [UsedImplicitly] init; }

    /// <summary>The phone number.</summary>
    public string? Phone { get; [UsedImplicitly] init; }

    /// <summary>The pipeline status.</summary>
    public ContactStatus Status { get; [UsedImplicitly] init; }

    /// <summary>The acquisition source.</summary>
    public ContactSource Source { get; [UsedImplicitly] init; }

    /// <summary>The estimated deal value.</summary>
    public decimal? Value { get; [UsedImplicitly] init; }

    /// <summary>Free-form notes.</summary>
    public string? Notes { get; [UsedImplicitly] init; }
}
