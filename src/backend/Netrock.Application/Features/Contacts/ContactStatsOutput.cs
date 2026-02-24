namespace Netrock.Application.Features.Contacts;

/// <summary>
/// Output representing aggregate pipeline statistics for the current user's contacts.
/// </summary>
/// <param name="TotalContacts">The total number of contacts.</param>
/// <param name="TotalValue">The total monetary value across all contacts.</param>
/// <param name="LeadCount">The number of contacts with Lead status.</param>
/// <param name="ProspectCount">The number of contacts with Prospect status.</param>
/// <param name="CustomerCount">The number of contacts with Customer status.</param>
/// <param name="ChurningCount">The number of contacts with Churning status.</param>
/// <param name="AverageValue">The average monetary value per contact, or zero if no contacts exist.</param>
/// <param name="SourceBreakdown">A dictionary of source names to their respective contact counts.</param>
/// <param name="RecentContacts">The most recent contacts, ordered by creation date descending.</param>
public record ContactStatsOutput(
    int TotalContacts,
    decimal TotalValue,
    int LeadCount,
    int ProspectCount,
    int CustomerCount,
    int ChurningCount,
    decimal AverageValue,
    IReadOnlyDictionary<string, int> SourceBreakdown,
    IReadOnlyList<ContactOutput> RecentContacts
);
