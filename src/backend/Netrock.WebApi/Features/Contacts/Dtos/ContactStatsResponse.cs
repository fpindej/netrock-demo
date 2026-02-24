using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Represents aggregate pipeline statistics for the current user's contacts.
/// </summary>
public class ContactStatsResponse
{
    /// <summary>
    /// The total number of contacts.
    /// </summary>
    public int TotalContacts { [UsedImplicitly] get; init; }

    /// <summary>
    /// The total monetary value across all contacts.
    /// </summary>
    public decimal TotalValue { [UsedImplicitly] get; init; }

    /// <summary>
    /// The number of contacts with Lead status.
    /// </summary>
    public int LeadCount { [UsedImplicitly] get; init; }

    /// <summary>
    /// The number of contacts with Prospect status.
    /// </summary>
    public int ProspectCount { [UsedImplicitly] get; init; }

    /// <summary>
    /// The number of contacts with Customer status.
    /// </summary>
    public int CustomerCount { [UsedImplicitly] get; init; }

    /// <summary>
    /// The number of contacts with Churning status.
    /// </summary>
    public int ChurningCount { [UsedImplicitly] get; init; }

    /// <summary>
    /// The average monetary value per contact.
    /// </summary>
    public decimal AverageValue { [UsedImplicitly] get; init; }

    /// <summary>
    /// A dictionary of source names to their respective contact counts.
    /// </summary>
    public IReadOnlyDictionary<string, int> SourceBreakdown { [UsedImplicitly] get; init; } =
        new Dictionary<string, int>();

    /// <summary>
    /// The most recent contacts, ordered by creation date descending.
    /// </summary>
    public IReadOnlyList<ContactResponse> RecentContacts { [UsedImplicitly] get; init; } = [];
}
