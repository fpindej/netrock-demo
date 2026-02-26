namespace Netrock.Application.Features.Jobs.Dtos;

/// <summary>
/// Output representing a single structured log entry from a job execution.
/// </summary>
/// <param name="Id">The log entry identifier.</param>
/// <param name="Timestamp">When the entry was recorded (UTC).</param>
/// <param name="Level">The log level ("Info", "Warning", "Error").</param>
/// <param name="Message">The log message.</param>
/// <param name="Category">Optional grouping category.</param>
public record JobExecutionLogEntryOutput(
    Guid Id,
    DateTimeOffset Timestamp,
    string Level,
    string Message,
    string? Category
);
