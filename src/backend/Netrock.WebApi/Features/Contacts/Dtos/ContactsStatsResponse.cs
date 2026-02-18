using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Represents aggregated pipeline statistics in API responses.
/// </summary>
public class ContactsStatsResponse
{
    /// <summary>Total number of contacts.</summary>
    public int TotalCount { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>Number of contacts with Customer status.</summary>
    public int CustomerCount { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>Sum of deal values for non-churned contacts.</summary>
    public decimal TotalPipelineValue { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>Contact count per pipeline status.</summary>
    public Dictionary<string, int> ByStatus { [UsedImplicitly] get; [UsedImplicitly] init; } = new();

    /// <summary>Contact count per acquisition source.</summary>
    public Dictionary<string, int> BySource { [UsedImplicitly] get; [UsedImplicitly] init; } = new();

    /// <summary>Total deal value per pipeline status.</summary>
    public Dictionary<string, decimal> PipelineValue { [UsedImplicitly] get; [UsedImplicitly] init; } = new();

    /// <summary>The last 5 contacts ordered by creation date.</summary>
    public List<ContactResponse> RecentContacts { [UsedImplicitly] get; [UsedImplicitly] init; } = [];
}
