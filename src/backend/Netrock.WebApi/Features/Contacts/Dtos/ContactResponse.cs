using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Represents a contact in API responses.
/// </summary>
public class ContactResponse
{
    /// <summary>The unique identifier of the contact.</summary>
    public Guid Id { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>The contact name.</summary>
    public string Name { [UsedImplicitly] get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>The contact email.</summary>
    public string? Email { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>The company name.</summary>
    public string? Company { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>The phone number.</summary>
    public string? Phone { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>The pipeline status.</summary>
    public string Status { [UsedImplicitly] get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>The acquisition source.</summary>
    public string Source { [UsedImplicitly] get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>The estimated deal value.</summary>
    public decimal? Value { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>Free-form notes.</summary>
    public string? Notes { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>Whether the contact is a favorite.</summary>
    public bool IsFavorite { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>When the contact was created.</summary>
    public DateTime CreatedAt { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>When the contact was last updated, or null if never updated.</summary>
    public DateTime? UpdatedAt { [UsedImplicitly] get; [UsedImplicitly] init; }
}
