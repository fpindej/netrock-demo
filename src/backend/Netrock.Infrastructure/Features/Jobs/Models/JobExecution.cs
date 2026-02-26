namespace Netrock.Infrastructure.Features.Jobs.Models;

/// <summary>
/// Represents a single execution of a recurring job, persisted in the <c>hangfire</c> schema.
/// Provides durable history that survives Hangfire's built-in retention limits.
/// </summary>
public class JobExecution
{
    /// <summary>
    /// Gets or sets the unique identifier for this execution record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the recurring job identifier (e.g. "expired-refresh-token-cleanup").
    /// </summary>
    public string RecurringJobId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the execution status: "Running", "Succeeded", or "Failed".
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC timestamp when execution started.
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when execution completed, or null if still running.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the total execution duration, or null if still running.
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Gets or sets the error message if the execution failed, or null on success.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets how the execution was triggered: "Schedule" or "Manual".
    /// </summary>
    public string? TriggeredBy { get; set; }

    /// <summary>
    /// Gets the structured log entries recorded during this execution.
    /// </summary>
    public ICollection<JobExecutionLogEntry> LogEntries { get; } = [];
}
