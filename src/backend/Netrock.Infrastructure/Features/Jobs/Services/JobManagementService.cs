using System.Collections.Concurrent;
using Hangfire;
using Hangfire.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Netrock.Application.Features.Jobs;
using Netrock.Application.Features.Jobs.Dtos;
using Netrock.Infrastructure.Features.Jobs.Extensions;
using Netrock.Infrastructure.Features.Jobs.Models;
using Netrock.Infrastructure.Persistence;
using Netrock.Shared;

namespace Netrock.Infrastructure.Features.Jobs.Services;

/// <summary>
/// Provides Hangfire recurring job management operations for the admin API.
/// Uses the DI-based <see cref="IRecurringJobManager"/> and <see cref="JobStorage"/>
/// to query, trigger, pause, and remove recurring jobs.
/// <para>
/// Pause state is persisted to the database and cached in a static dictionary
/// so it survives application restarts.
/// </para>
/// </summary>
internal sealed class JobManagementService(
    ILogger<JobManagementService> logger,
    NetrockDbContext dbContext,
    IRecurringJobManager jobManager,
    JobStorage jobStorage,
    IEnumerable<IRecurringJobDefinition> jobDefinitions,
    TimeProvider timeProvider) : IJobManagementService
{
    /// <summary>
    /// In-memory cache of paused job cron expressions, backed by the <c>hangfire.pausedjobs</c> table.
    /// Thread-safe since Hangfire may run on multiple workers.
    /// </summary>
    internal static readonly ConcurrentDictionary<string, string> PausedJobCrons = new();

    /// <summary>
    /// Cron expression that effectively pauses a job by scheduling it far in the future.
    /// Hangfire does not have a native pause — setting cron to "0 0 31 2 *" (Feb 31) ensures it never fires.
    /// </summary>
    internal const string NeverCron = "0 0 31 2 *";

    /// <inheritdoc />
    public Task<IReadOnlyList<RecurringJobOutput>> GetRecurringJobsAsync()
    {
        using var connection = jobStorage.GetConnection();
        var recurringJobs = connection.GetRecurringJobs();

        var result = recurringJobs.Select(job =>
        {
            var isPaused = PausedJobCrons.TryGetValue(job.Id, out var originalCron);
            return new RecurringJobOutput(
                Id: job.Id,
                Cron: isPaused ? originalCron! : job.Cron,
                NextExecution: job.NextExecution.HasValue
                    ? new DateTimeOffset(job.NextExecution.Value, TimeSpan.Zero)
                    : null,
                LastExecution: job.LastExecution.HasValue
                    ? new DateTimeOffset(job.LastExecution.Value, TimeSpan.Zero)
                    : null,
                LastStatus: job.LastJobState,
                IsPaused: isPaused,
                CreatedAt: job.CreatedAt.HasValue
                    ? new DateTimeOffset(job.CreatedAt.Value, TimeSpan.Zero)
                    : null
            );
        }).ToList();

        return Task.FromResult<IReadOnlyList<RecurringJobOutput>>(result);
    }

    /// <inheritdoc />
    public async Task<Result<RecurringJobDetailOutput>> GetRecurringJobDetailAsync(string jobId)
    {
        using var connection = jobStorage.GetConnection();
        var recurringJobs = connection.GetRecurringJobs();
        var job = recurringJobs.FirstOrDefault(j => j.Id == jobId);

        if (job is null)
        {
            return Result<RecurringJobDetailOutput>.Failure(ErrorMessages.Jobs.NotFound, ErrorType.NotFound);
        }

        var isPaused = PausedJobCrons.TryGetValue(job.Id, out var originalCron);
        var displayCron = isPaused ? originalCron! : job.Cron;

        var executionHistory = await GetRecentExecutionsAsync(job);

        var detail = new RecurringJobDetailOutput(
            Id: job.Id,
            Cron: displayCron,
            NextExecution: job.NextExecution.HasValue
                ? new DateTimeOffset(job.NextExecution.Value, TimeSpan.Zero)
                : null,
            LastExecution: job.LastExecution.HasValue
                ? new DateTimeOffset(job.LastExecution.Value, TimeSpan.Zero)
                : null,
            LastStatus: job.LastJobState,
            IsPaused: isPaused,
            CreatedAt: job.CreatedAt.HasValue
                ? new DateTimeOffset(job.CreatedAt.Value, TimeSpan.Zero)
                : null,
            ExecutionHistory: executionHistory
        );

        return Result<RecurringJobDetailOutput>.Success(detail);
    }

    /// <inheritdoc />
    public Task<Result> TriggerJobAsync(string jobId)
    {
        if (!JobExists(jobId))
        {
            logger.LogWarning("Attempted to trigger non-existent job '{JobId}'", jobId);
            return Task.FromResult(Result.Failure(ErrorMessages.Jobs.NotFound, ErrorType.NotFound));
        }

        try
        {
            ApplicationBuilderExtensions.ManualTriggers[jobId] = true;
            jobManager.Trigger(jobId);
            logger.LogInformation("Manually triggered job '{JobId}'", jobId);
            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to trigger job '{JobId}'", jobId);
            return Task.FromResult(Result.Failure(ErrorMessages.Jobs.TriggerFailed));
        }
    }

    /// <inheritdoc />
    public async Task<Result> RemoveJobAsync(string jobId)
    {
        if (!JobExists(jobId))
        {
            logger.LogWarning("Attempted to remove non-existent job '{JobId}'", jobId);
            return Result.Failure(ErrorMessages.Jobs.NotFound, ErrorType.NotFound);
        }

        jobManager.RemoveIfExists(jobId);

        var pausedJob = await dbContext.PausedJobs.FirstOrDefaultAsync(p => p.JobId == jobId);
        if (pausedJob is not null)
        {
            dbContext.PausedJobs.Remove(pausedJob);
            await dbContext.SaveChangesAsync();
        }

        PausedJobCrons.TryRemove(jobId, out _);
        logger.LogWarning("Removed recurring job '{JobId}'", jobId);
        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result> PauseJobAsync(string jobId)
    {
        using var connection = jobStorage.GetConnection();
        var recurringJobs = connection.GetRecurringJobs();
        var job = recurringJobs.FirstOrDefault(j => j.Id == jobId);

        if (job is null)
        {
            logger.LogWarning("Attempted to pause non-existent job '{JobId}'", jobId);
            return Result.Failure(ErrorMessages.Jobs.NotFound, ErrorType.NotFound);
        }

        if (PausedJobCrons.ContainsKey(jobId))
        {
            logger.LogDebug("Job '{JobId}' is already paused", jobId);
            return Result.Success();
        }

        dbContext.PausedJobs.Add(new PausedJob
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            OriginalCron = job.Cron,
            PausedAt = timeProvider.GetUtcNow().UtcDateTime
        });
        await dbContext.SaveChangesAsync();

        PausedJobCrons[jobId] = job.Cron;
        jobManager.AddOrUpdate(jobId, () => ApplicationBuilderExtensions.ExecuteJobAsync(jobId), NeverCron);
        logger.LogInformation("Paused job '{JobId}' (original cron: '{OriginalCron}')", jobId, job.Cron);
        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result> ResumeJobAsync(string jobId)
    {
        if (!JobExists(jobId))
        {
            logger.LogWarning("Attempted to resume non-existent job '{JobId}'", jobId);
            return Result.Failure(ErrorMessages.Jobs.NotFound, ErrorType.NotFound);
        }

        if (!PausedJobCrons.TryGetValue(jobId, out var originalCron))
        {
            logger.LogDebug("Job '{JobId}' is not paused — nothing to resume", jobId);
            return Result.Success();
        }

        var pausedJob = await dbContext.PausedJobs.FirstOrDefaultAsync(p => p.JobId == jobId);
        if (pausedJob is not null)
        {
            dbContext.PausedJobs.Remove(pausedJob);
            await dbContext.SaveChangesAsync();
        }

        PausedJobCrons.TryRemove(jobId, out _);
        jobManager.AddOrUpdate(jobId, () => ApplicationBuilderExtensions.ExecuteJobAsync(jobId), originalCron);
        logger.LogInformation("Resumed job '{JobId}' (restored cron: '{RestoredCron}')", jobId, originalCron);
        return Result.Success();
    }

    /// <inheritdoc />
    public Task<Result> RestoreJobsAsync()
    {
        try
        {
            var definitions = jobDefinitions.ToList();

            foreach (var definition in definitions)
            {
                var isPaused = PausedJobCrons.ContainsKey(definition.JobId);
                var cron = isPaused ? NeverCron : definition.CronExpression;

                jobManager.AddOrUpdate(
                    definition.JobId,
                    () => ApplicationBuilderExtensions.ExecuteJobAsync(definition.JobId),
                    cron);

                logger.LogInformation(
                    "Restored job '{JobId}' with cron '{Cron}'{PauseNote}",
                    definition.JobId,
                    definition.CronExpression,
                    isPaused ? " (paused)" : "");
            }

            logger.LogInformation("Restored {Count} recurring job(s)", definitions.Count);
            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to restore jobs");
            return Task.FromResult(Result.Failure(ErrorMessages.Jobs.RestoreFailed));
        }
    }

    private bool JobExists(string jobId)
    {
        using var connection = jobStorage.GetConnection();
        var recurringJobs = connection.GetRecurringJobs();
        return recurringJobs.Any(j => j.Id == jobId);
    }

    private async Task<IReadOnlyList<JobExecutionOutput>> GetRecentExecutionsAsync(RecurringJobDto job)
    {
        return await dbContext.JobExecutions
            .AsNoTracking()
            .Where(e => e.RecurringJobId == job.Id)
            .OrderByDescending(e => e.StartedAt)
            .Take(10)
            .Select(e => new JobExecutionOutput(
                JobId: e.Id.ToString(),
                Status: e.Status,
                StartedAt: new DateTimeOffset(e.StartedAt, TimeSpan.Zero),
                Duration: e.Duration,
                Error: e.ErrorMessage
            ))
            .ToListAsync();
    }
}
