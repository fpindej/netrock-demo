using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Admin.Dtos.Jobs;

/// <summary>
/// Represents a single structured log entry from a job execution.
/// </summary>
public class JobExecutionLogEntryResponse
{
    /// <summary>
    /// The log entry identifier.
    /// </summary>
    public Guid Id { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>
    /// When the entry was recorded (UTC).
    /// </summary>
    public DateTimeOffset Timestamp { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>
    /// The log level ("Info", "Warning", "Error").
    /// </summary>
    public string Level { [UsedImplicitly] get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// The log message.
    /// </summary>
    public string Message { [UsedImplicitly] get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// Optional grouping category for this log entry.
    /// </summary>
    public string? Category { [UsedImplicitly] get; [UsedImplicitly] init; }
}
