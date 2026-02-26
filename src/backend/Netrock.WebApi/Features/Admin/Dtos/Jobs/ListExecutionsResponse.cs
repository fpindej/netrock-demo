using Netrock.WebApi.Shared;

namespace Netrock.WebApi.Features.Admin.Dtos.Jobs;

/// <summary>
/// Paginated response containing a list of job execution summaries.
/// </summary>
public class ListExecutionsResponse : PaginatedResponse
{
    /// <summary>
    /// The execution summaries for the current page.
    /// </summary>
    public IReadOnlyList<JobExecutionSummaryResponse> Items { get; init; } = [];
}
