namespace Netrock.Application.Features.Jobs.Dtos;

/// <summary>
/// Detailed output for a single job execution, including structured log entries.
/// </summary>
/// <param name="Id">The execution identifier.</param>
/// <param name="RecurringJobId">The recurring job identifier.</param>
/// <param name="HangfireJobId">The Hangfire background job identifier, if available.</param>
/// <param name="Status">The execution status ("Running", "Succeeded", "Failed").</param>
/// <param name="StartedAt">When the execution started (UTC).</param>
/// <param name="CompletedAt">When the execution completed (UTC), or null if still running.</param>
/// <param name="Duration">How long the execution took, or null if still running.</param>
/// <param name="ErrorMessage">The error message if the execution failed, or null on success.</param>
/// <param name="TriggeredBy">How the execution was triggered ("Schedule" or "Manual").</param>
/// <param name="LogEntries">Structured log entries recorded during this execution.</param>
public record JobExecutionDetailOutput(
    Guid Id,
    string RecurringJobId,
    string? HangfireJobId,
    string Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt,
    TimeSpan? Duration,
    string? ErrorMessage,
    string? TriggeredBy,
    IReadOnlyList<JobExecutionLogEntryOutput> LogEntries
);
