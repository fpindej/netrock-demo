using Netrock.Application.Features.Jobs.Dtos;
using Netrock.WebApi.Features.Admin.Dtos.Jobs;

namespace Netrock.WebApi.Features.Admin;

/// <summary>
/// Maps between job Application layer DTOs and WebApi response DTOs.
/// </summary>
internal static class JobsMapper
{
    /// <summary>
    /// Maps a <see cref="RecurringJobOutput"/> to a <see cref="RecurringJobResponse"/>.
    /// </summary>
    public static RecurringJobResponse ToResponse(this RecurringJobOutput output) => new()
    {
        Id = output.Id,
        Cron = output.Cron,
        NextExecution = output.NextExecution,
        LastExecution = output.LastExecution,
        LastStatus = output.LastStatus,
        IsPaused = output.IsPaused,
        CreatedAt = output.CreatedAt
    };

    /// <summary>
    /// Maps a <see cref="RecurringJobDetailOutput"/> to a <see cref="RecurringJobDetailResponse"/>.
    /// </summary>
    public static RecurringJobDetailResponse ToResponse(this RecurringJobDetailOutput output) => new()
    {
        Id = output.Id,
        Cron = output.Cron,
        NextExecution = output.NextExecution,
        LastExecution = output.LastExecution,
        LastStatus = output.LastStatus,
        IsPaused = output.IsPaused,
        CreatedAt = output.CreatedAt,
        ExecutionHistory = output.ExecutionHistory.Select(e => e.ToResponse()).ToList()
    };

    /// <summary>
    /// Maps a <see cref="JobExecutionOutput"/> to a <see cref="JobExecutionResponse"/>.
    /// </summary>
    public static JobExecutionResponse ToResponse(this JobExecutionOutput output) => new()
    {
        JobId = output.JobId,
        Status = output.Status,
        StartedAt = output.StartedAt,
        Duration = output.Duration,
        Error = output.Error
    };

    /// <summary>
    /// Maps a <see cref="JobExecutionSummaryOutput"/> to a <see cref="JobExecutionSummaryResponse"/>.
    /// </summary>
    public static JobExecutionSummaryResponse ToResponse(this JobExecutionSummaryOutput output) => new()
    {
        Id = output.Id,
        RecurringJobId = output.RecurringJobId,
        Status = output.Status,
        StartedAt = output.StartedAt,
        CompletedAt = output.CompletedAt,
        Duration = output.Duration,
        ErrorMessage = output.ErrorMessage,
        TriggeredBy = output.TriggeredBy
    };

    /// <summary>
    /// Maps a <see cref="JobExecutionDetailOutput"/> to a <see cref="JobExecutionDetailResponse"/>.
    /// </summary>
    public static JobExecutionDetailResponse ToResponse(this JobExecutionDetailOutput output) => new()
    {
        Id = output.Id,
        RecurringJobId = output.RecurringJobId,
        HangfireJobId = output.HangfireJobId,
        Status = output.Status,
        StartedAt = output.StartedAt,
        CompletedAt = output.CompletedAt,
        Duration = output.Duration,
        ErrorMessage = output.ErrorMessage,
        TriggeredBy = output.TriggeredBy,
        LogEntries = output.LogEntries.Select(l => l.ToResponse()).ToList()
    };

    /// <summary>
    /// Maps a <see cref="JobExecutionLogEntryOutput"/> to a <see cref="JobExecutionLogEntryResponse"/>.
    /// </summary>
    public static JobExecutionLogEntryResponse ToResponse(this JobExecutionLogEntryOutput output) => new()
    {
        Id = output.Id,
        Timestamp = output.Timestamp,
        Level = output.Level,
        Message = output.Message,
        Category = output.Category
    };

    /// <summary>
    /// Maps a <see cref="JobExecutionListOutput"/> to a <see cref="ListExecutionsResponse"/>.
    /// </summary>
    public static ListExecutionsResponse ToResponse(this JobExecutionListOutput output) => new()
    {
        Items = output.Executions.Select(e => e.ToResponse()).ToList(),
        TotalCount = output.TotalCount,
        PageNumber = output.PageNumber,
        PageSize = output.PageSize
    };
}
