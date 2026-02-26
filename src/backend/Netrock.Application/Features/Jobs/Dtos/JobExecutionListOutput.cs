namespace Netrock.Application.Features.Jobs.Dtos;

/// <summary>
/// Paginated list of job execution summaries.
/// </summary>
/// <param name="Executions">The execution summaries for the current page.</param>
/// <param name="TotalCount">The total number of executions across all pages.</param>
/// <param name="PageNumber">The current page number.</param>
/// <param name="PageSize">The number of items per page.</param>
public record JobExecutionListOutput(
    IReadOnlyList<JobExecutionSummaryOutput> Executions,
    int TotalCount,
    int PageNumber,
    int PageSize
);
