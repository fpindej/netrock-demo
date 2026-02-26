namespace Netrock.Infrastructure.Features.Jobs.Models;

/// <summary>
/// Represents a single structured log entry recorded during a job execution.
/// </summary>
public class JobExecutionLogEntry
{
    /// <summary>
    /// Gets or sets the unique identifier for this log entry.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the parent execution identifier.
    /// </summary>
    public Guid JobExecutionId { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when this entry was recorded.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the log level: "Info", "Warning", or "Error".
    /// </summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the log message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional grouping category for this log entry.
    /// </summary>
    public string? Category { get; set; }
}
