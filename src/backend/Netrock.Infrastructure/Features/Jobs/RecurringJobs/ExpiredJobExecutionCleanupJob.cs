using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Netrock.Application.Features.Jobs;
using Netrock.Infrastructure.Persistence;

namespace Netrock.Infrastructure.Features.Jobs.RecurringJobs;

/// <summary>
/// Removes job execution records (and their cascade-deleted log entries) older than the
/// retention period. Runs daily to keep the <c>hangfire.jobexecutions</c> table bounded.
/// </summary>
internal sealed class ExpiredJobExecutionCleanupJob(
    NetrockDbContext dbContext,
    TimeProvider timeProvider,
    IJobExecutionContext executionContext,
    ILogger<ExpiredJobExecutionCleanupJob> logger) : IRecurringJobDefinition
{
    /// <inheritdoc />
    public string JobId => "expired-job-execution-cleanup";

    /// <inheritdoc />
    public string CronExpression => Cron.Daily();

    /// <summary>
    /// How long to keep execution records before pruning. 90 days gives enough history
    /// for diagnostics while keeping the table size manageable.
    /// </summary>
    private static readonly TimeSpan RetentionPeriod = TimeSpan.FromDays(90);

    /// <inheritdoc />
    public async Task ExecuteAsync()
    {
        var cutoff = timeProvider.GetUtcNow().UtcDateTime - RetentionPeriod;
        executionContext.LogInfo($"Retention cutoff: {cutoff:O} ({RetentionPeriod.TotalDays:F0} days)", "Cleanup");

        var deletedCount = await dbContext.JobExecutions
            .Where(e => e.StartedAt < cutoff)
            .ExecuteDeleteAsync();

        executionContext.LogInfo($"Deleted {deletedCount} expired execution records", "Cleanup");
        logger.LogInformation("Deleted {Count} expired job execution records (older than {Days} days)",
            deletedCount, RetentionPeriod.TotalDays);
    }
}
