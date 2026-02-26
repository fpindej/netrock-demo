namespace Netrock.Application.Features.Jobs;

/// <summary>
/// Provides structured logging within a job execution scope.
/// <para>
/// Injected into recurring jobs that want to record per-execution log entries.
/// Entries are collected in memory during the execution and flushed to the database
/// when the execution completes.
/// </para>
/// <para>
/// This is opt-in — jobs that do not inject this interface require no changes.
/// </para>
/// </summary>
public interface IJobExecutionContext
{
    /// <summary>
    /// Records an informational log entry.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <param name="category">Optional grouping category.</param>
    void LogInfo(string message, string? category = null);

    /// <summary>
    /// Records a warning log entry.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <param name="category">Optional grouping category.</param>
    void LogWarning(string message, string? category = null);

    /// <summary>
    /// Records an error log entry.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <param name="ex">Optional exception to include in the message.</param>
    /// <param name="category">Optional grouping category.</param>
    void LogError(string message, Exception? ex = null, string? category = null);
}
