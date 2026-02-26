using System.Collections.Concurrent;
using System.Diagnostics;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Netrock.Application.Features.Jobs;
using Netrock.Infrastructure.Features.Jobs.Models;
using Netrock.Infrastructure.Features.Jobs.Options;
using Netrock.Infrastructure.Features.Jobs.Services;
using Netrock.Infrastructure.Persistence;

namespace Netrock.Infrastructure.Features.Jobs.Extensions;

/// <summary>
/// Extension methods for configuring the Hangfire middleware pipeline and registering recurring jobs.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Root service provider captured at startup, used by <see cref="ExecuteJobAsync"/>
    /// to create a fresh DI scope per job execution. Stored statically because Hangfire
    /// serializes job arguments as JSON — <see cref="IServiceProvider"/> is not serializable.
    /// </summary>
    private static IServiceProvider? _rootServiceProvider;

    /// <summary>
    /// Tracks job IDs that were manually triggered. Set in <c>TriggerJobAsync</c>,
    /// consumed (and removed) in <see cref="ExecuteJobAsync"/> to differentiate
    /// manual triggers from scheduled runs.
    /// </summary>
    internal static readonly ConcurrentDictionary<string, bool> ManualTriggers = new();

    /// <summary>
    /// Configures the Hangfire dashboard (development only) and registers all recurring jobs
    /// discovered via <see cref="IRecurringJobDefinition"/> implementations.
    /// <para>
    /// In development, the built-in Hangfire dashboard is available at <c>/hangfire</c>
    /// with no authentication. In production, use the admin API endpoints instead.
    /// </para>
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseJobScheduling(this IApplicationBuilder app)
    {
        var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(ApplicationBuilderExtensions));

        var options = app.ApplicationServices.GetRequiredService<IOptions<JobSchedulingOptions>>().Value;

        if (!options.Enabled)
        {
            logger.LogInformation("Job scheduling is disabled via configuration");
            return app;
        }

        _rootServiceProvider = app.ApplicationServices;

        var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

        if (env.IsDevelopment())
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = []
            });
            logger.LogInformation("Hangfire dashboard enabled at /hangfire (development only)");
        }

        var jobManager = app.ApplicationServices.GetRequiredService<IRecurringJobManager>();

        RegisterRecurringJobs(jobManager, app.ApplicationServices, logger);
        RestorePauseStateAsync(jobManager, app.ApplicationServices, logger).GetAwaiter().GetResult();

        return app;
    }

    private static void RegisterRecurringJobs(
        IRecurringJobManager jobManager, IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var jobDefinitions = scope.ServiceProvider.GetServices<IRecurringJobDefinition>().ToList();

        if (jobDefinitions.Count == 0)
        {
            logger.LogWarning("No IRecurringJobDefinition implementations found — no jobs registered");
            return;
        }

        foreach (var job in jobDefinitions)
        {
            jobManager.AddOrUpdate(
                job.JobId,
                () => ExecuteJobAsync(job.JobId),
                job.CronExpression);

            logger.LogInformation("Registered recurring job '{JobId}' with cron '{CronExpression}'",
                job.JobId, job.CronExpression);
        }

        logger.LogInformation("Registered {Count} recurring job(s)", jobDefinitions.Count);
    }

    /// <summary>
    /// Loads persisted pause state from the database and overrides paused jobs with a never-firing cron.
    /// Called once at startup after <see cref="RegisterRecurringJobs"/> to restore pause state.
    /// </summary>
    private static async Task RestorePauseStateAsync(
        IRecurringJobManager jobManager, IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NetrockDbContext>();

        var pausedJobs = await dbContext.PausedJobs.ToListAsync();

        if (pausedJobs.Count == 0)
        {
            return;
        }

        foreach (var pausedJob in pausedJobs)
        {
            JobManagementService.PausedJobCrons[pausedJob.JobId] = pausedJob.OriginalCron;

            jobManager.AddOrUpdate(
                pausedJob.JobId,
                () => ExecuteJobAsync(pausedJob.JobId),
                JobManagementService.NeverCron);

            logger.LogInformation(
                "Restored pause state for job '{JobId}' (original cron: '{OriginalCron}')",
                pausedJob.JobId, pausedJob.OriginalCron);
        }

        logger.LogInformation("Restored {Count} paused job state(s) from database", pausedJobs.Count);
    }

    /// <summary>
    /// Resolves a job definition from DI and executes it, recording the execution
    /// in the <c>hangfire.jobexecutions</c> table with structured log entries.
    /// <para>
    /// Only the <paramref name="jobId"/> string is passed through Hangfire's serialization —
    /// the service provider is accessed from the static field captured at startup.
    /// </para>
    /// </summary>
    /// <param name="jobId">The job identifier to resolve and execute.</param>
    public static async Task ExecuteJobAsync(string jobId)
    {
        if (_rootServiceProvider is null)
        {
            return;
        }

        var logger = _rootServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(ApplicationBuilderExtensions));

        using var scope = _rootServiceProvider.CreateScope();
        var jobDefinitions = scope.ServiceProvider.GetServices<IRecurringJobDefinition>();
        var job = jobDefinitions.FirstOrDefault(j => j.JobId == jobId);

        if (job is null)
        {
            logger.LogWarning("No IRecurringJobDefinition found for job '{JobId}' — skipping execution", jobId);
            return;
        }

        var dbContext = scope.ServiceProvider.GetRequiredService<NetrockDbContext>();
        var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>();
        var executionContext = scope.ServiceProvider.GetRequiredService<JobExecutionContext>();

        var isManual = ManualTriggers.TryRemove(jobId, out _);

        var execution = new JobExecution
        {
            Id = Guid.NewGuid(),
            RecurringJobId = jobId,
            Status = JobExecutionStatus.Running,
            StartedAt = timeProvider.GetUtcNow().UtcDateTime,
            TriggeredBy = isManual ? JobExecutionTrigger.Manual : JobExecutionTrigger.Schedule
        };

        dbContext.JobExecutions.Add(execution);
        await dbContext.SaveChangesAsync();

        executionContext.ExecutionId = execution.Id;

        logger.LogInformation("Executing job '{JobId}' (execution {ExecutionId})", jobId, execution.Id);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await job.ExecuteAsync();
            stopwatch.Stop();

            execution.Status = JobExecutionStatus.Succeeded;
            execution.CompletedAt = timeProvider.GetUtcNow().UtcDateTime;
            execution.Duration = stopwatch.Elapsed;

            logger.LogInformation("Job '{JobId}' completed in {ElapsedMs}ms", jobId, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            execution.Status = JobExecutionStatus.Failed;
            execution.CompletedAt = timeProvider.GetUtcNow().UtcDateTime;
            execution.Duration = stopwatch.Elapsed;
            execution.ErrorMessage = ex.Message.Length > 4000 ? ex.Message[..4000] : ex.Message;

            logger.LogError(ex, "Job '{JobId}' failed after {ElapsedMs}ms", jobId, stopwatch.ElapsedMilliseconds);

            await SafeFlushAsync(dbContext, execution, executionContext, logger, jobId);
            throw;
        }

        await SafeFlushAsync(dbContext, execution, executionContext, logger, jobId);
    }

    /// <summary>
    /// Persists the execution record and any collected log entries to the database.
    /// Catches and logs any persistence failures to avoid masking the original job exception.
    /// </summary>
    private static async Task SafeFlushAsync(
        NetrockDbContext dbContext, JobExecution execution, JobExecutionContext executionContext,
        ILogger logger, string jobId)
    {
        try
        {
            if (executionContext.Entries.Count > 0)
            {
                dbContext.JobExecutionLogEntries.AddRange(executionContext.Entries);
            }

            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to flush execution record for job '{JobId}' (execution {ExecutionId})",
                jobId, execution.Id);
        }
    }
}
