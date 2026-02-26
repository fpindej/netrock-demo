using Microsoft.EntityFrameworkCore;
using Netrock.Application.Features.Jobs;
using Netrock.Application.Features.Jobs.Dtos;
using Netrock.Infrastructure.Persistence;
using Netrock.Infrastructure.Persistence.Extensions;
using Netrock.Shared;

namespace Netrock.Infrastructure.Features.Jobs.Services;

/// <summary>
/// Provides query operations for job execution history from the database.
/// </summary>
internal sealed class JobExecutionService(NetrockDbContext dbContext) : IJobExecutionService
{
    /// <inheritdoc />
    public async Task<JobExecutionListOutput> GetExecutionsAsync(
        string recurringJobId, int pageNumber, int pageSize,
        string? statusFilter = null, CancellationToken ct = default)
    {
        var query = dbContext.JobExecutions
            .AsNoTracking()
            .Where(e => e.RecurringJobId == recurringJobId);

        if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            query = query.Where(e => e.Status == statusFilter);
        }

        var totalCount = await query.CountAsync(ct);

        var executions = await query
            .OrderByDescending(e => e.StartedAt)
            .Paginate(pageNumber, pageSize)
            .Select(e => new JobExecutionSummaryOutput(
                e.Id,
                e.RecurringJobId,
                e.Status,
                new DateTimeOffset(e.StartedAt, TimeSpan.Zero),
                e.CompletedAt.HasValue ? new DateTimeOffset(e.CompletedAt.Value, TimeSpan.Zero) : null,
                e.Duration,
                e.ErrorMessage,
                e.TriggeredBy
            ))
            .ToListAsync(ct);

        return new JobExecutionListOutput(executions, totalCount, pageNumber, pageSize);
    }

    /// <inheritdoc />
    public async Task<Result<JobExecutionDetailOutput>> GetExecutionDetailAsync(
        Guid executionId, CancellationToken ct = default)
    {
        var execution = await dbContext.JobExecutions
            .AsNoTracking()
            .Include(e => e.LogEntries.OrderBy(l => l.Timestamp))
            .FirstOrDefaultAsync(e => e.Id == executionId, ct);

        if (execution is null)
        {
            return Result<JobExecutionDetailOutput>.Failure(ErrorMessages.Jobs.ExecutionNotFound, ErrorType.NotFound);
        }

        var logEntries = execution.LogEntries.Select(l => new JobExecutionLogEntryOutput(
            l.Id,
            new DateTimeOffset(l.Timestamp, TimeSpan.Zero),
            l.Level,
            l.Message,
            l.Category
        )).ToList();

        var detail = new JobExecutionDetailOutput(
            Id: execution.Id,
            RecurringJobId: execution.RecurringJobId,
            HangfireJobId: execution.HangfireJobId,
            Status: execution.Status,
            StartedAt: new DateTimeOffset(execution.StartedAt, TimeSpan.Zero),
            CompletedAt: execution.CompletedAt.HasValue
                ? new DateTimeOffset(execution.CompletedAt.Value, TimeSpan.Zero)
                : null,
            Duration: execution.Duration,
            ErrorMessage: execution.ErrorMessage,
            TriggeredBy: execution.TriggeredBy,
            LogEntries: logEntries
        );

        return Result<JobExecutionDetailOutput>.Success(detail);
    }
}
