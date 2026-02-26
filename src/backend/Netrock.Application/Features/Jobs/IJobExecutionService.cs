using Netrock.Application.Features.Jobs.Dtos;
using Netrock.Shared;

namespace Netrock.Application.Features.Jobs;

/// <summary>
/// Provides query operations for job execution history.
/// <para>
/// Separated from <see cref="IJobManagementService"/> to maintain single responsibility:
/// management (trigger, pause, remove) vs. history queries.
/// </para>
/// </summary>
public interface IJobExecutionService
{
    /// <summary>
    /// Gets a paginated list of executions for a recurring job.
    /// </summary>
    /// <param name="recurringJobId">The recurring job identifier.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="statusFilter">Optional status filter (e.g. "Succeeded", "Failed").</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated list of execution summaries.</returns>
    Task<JobExecutionListOutput> GetExecutionsAsync(
        string recurringJobId, int pageNumber, int pageSize,
        string? statusFilter = null, CancellationToken ct = default);

    /// <summary>
    /// Gets detailed information about a single execution, including log entries.
    /// </summary>
    /// <param name="executionId">The execution identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The execution detail, or a failure if not found.</returns>
    Task<Result<JobExecutionDetailOutput>> GetExecutionDetailAsync(
        Guid executionId, CancellationToken ct = default);
}
