using JetBrains.Annotations;
using Netrock.Domain.Entities;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Represents a request to update an existing contact.
/// </summary>
[UsedImplicitly]
public class UpdateContactRequest
{
    /// <summary>The new contact name.</summary>
    public string Name { get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>The new contact email.</summary>
    public string? Email { get; [UsedImplicitly] init; }

    /// <summary>The new company name.</summary>
    public string? Company { get; [UsedImplicitly] init; }

    /// <summary>The new phone number.</summary>
    public string? Phone { get; [UsedImplicitly] init; }

    /// <summary>The new pipeline status.</summary>
    public ContactStatus Status { get; [UsedImplicitly] init; }

    /// <summary>The new acquisition source.</summary>
    public ContactSource Source { get; [UsedImplicitly] init; }

    /// <summary>The new deal value.</summary>
    public decimal? Value { get; [UsedImplicitly] init; }

    /// <summary>The new notes.</summary>
    public string? Notes { get; [UsedImplicitly] init; }

    /// <summary>Whether the contact is a favorite.</summary>
    public bool IsFavorite { get; [UsedImplicitly] init; }
}
