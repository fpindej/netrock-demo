using Netrock.Domain.Entities;

namespace Netrock.Application.Features.Contacts.Dtos;

/// <summary>
/// Output containing aggregated statistics about a user's contact pipeline.
/// </summary>
/// <param name="TotalCount">Total number of contacts.</param>
/// <param name="CustomerCount">Number of contacts with Customer status.</param>
/// <param name="TotalPipelineValue">Sum of Value for non-churned contacts.</param>
/// <param name="ByStatus">Contact count per status.</param>
/// <param name="BySource">Contact count per source.</param>
/// <param name="PipelineValue">Total deal value per status.</param>
/// <param name="RecentContacts">The last 5 contacts ordered by creation date.</param>
public record ContactsStatsOutput(
    int TotalCount,
    int CustomerCount,
    decimal TotalPipelineValue,
    Dictionary<ContactStatus, int> ByStatus,
    Dictionary<ContactSource, int> BySource,
    Dictionary<ContactStatus, decimal> PipelineValue,
    List<ContactOutput> RecentContacts);
