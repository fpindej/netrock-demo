namespace Netrock.Application.Features.Jobs.Dtos;

/// <summary>
/// Summary output for a single job execution in a paginated list.
/// </summary>
/// <param name="Id">The execution identifier.</param>
/// <param name="RecurringJobId">The recurring job identifier.</param>
/// <param name="Status">The execution status ("Running", "Succeeded", "Failed").</param>
/// <param name="StartedAt">When the execution started (UTC).</param>
/// <param name="CompletedAt">When the execution completed (UTC), or null if still running.</param>
/// <param name="Duration">How long the execution took, or null if still running.</param>
/// <param name="ErrorMessage">The error message if the execution failed, or null on success.</param>
/// <param name="TriggeredBy">How the execution was triggered ("Schedule" or "Manual").</param>
public record JobExecutionSummaryOutput(
    Guid Id,
    string RecurringJobId,
    string Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    TimeSpan? Duration,
    string? ErrorMessage,
    string? TriggeredBy
);
