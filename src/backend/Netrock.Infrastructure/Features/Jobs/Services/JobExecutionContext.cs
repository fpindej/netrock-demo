using Netrock.Application.Features.Jobs;
using Netrock.Infrastructure.Features.Jobs.Models;

namespace Netrock.Infrastructure.Features.Jobs.Services;

/// <summary>
/// Scoped implementation of <see cref="IJobExecutionContext"/> that collects
/// <see cref="JobExecutionLogEntry"/> instances in memory during a job execution.
/// Entries are flushed to the database by <c>ExecuteJobAsync</c> after the job completes.
/// </summary>
internal sealed class JobExecutionContext(TimeProvider timeProvider) : IJobExecutionContext
{
    private readonly List<JobExecutionLogEntry> _entries = [];

    /// <summary>
    /// Gets or sets the execution identifier. Set by <c>ExecuteJobAsync</c> before the job runs.
    /// </summary>
    internal Guid ExecutionId { get; set; }

    /// <summary>
    /// Gets the collected log entries for flushing to the database.
    /// </summary>
    internal IReadOnlyList<JobExecutionLogEntry> Entries => _entries;

    /// <inheritdoc />
    public void LogInfo(string message, string? category = null)
        => AddEntry("Info", message, category);

    /// <inheritdoc />
    public void LogWarning(string message, string? category = null)
        => AddEntry("Warning", message, category);

    /// <inheritdoc />
    public void LogError(string message, Exception? ex = null, string? category = null)
    {
        var fullMessage = ex is not null ? $"{message}: {ex.Message}" : message;
        AddEntry("Error", fullMessage, category);
    }

    private void AddEntry(string level, string message, string? category)
    {
        _entries.Add(new JobExecutionLogEntry
        {
            Id = Guid.NewGuid(),
            JobExecutionId = ExecutionId,
            Timestamp = timeProvider.GetUtcNow().UtcDateTime,
            Level = level,
            Message = message.Length > 2000 ? message[..2000] : message,
            Category = category?.Length > 100 ? category[..100] : category
        });
    }
}
