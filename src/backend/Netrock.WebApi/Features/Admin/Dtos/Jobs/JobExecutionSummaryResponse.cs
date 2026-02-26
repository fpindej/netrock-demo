using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Admin.Dtos.Jobs;

/// <summary>
/// Summary of a single job execution for list views.
/// </summary>
public class JobExecutionSummaryResponse
{
    /// <summary>
    /// The execution identifier.
    /// </summary>
    public Guid Id { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>
    /// The recurring job identifier.
    /// </summary>
    public string RecurringJobId { [UsedImplicitly] get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// The execution status ("Running", "Succeeded", "Failed").
    /// </summary>
    public string Status { [UsedImplicitly] get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// When the execution started (UTC).
    /// </summary>
    public DateTimeOffset StartedAt { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>
    /// When the execution completed (UTC), or null if still running.
    /// </summary>
    public DateTimeOffset? CompletedAt { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>
    /// How long the execution took, or null if still running.
    /// </summary>
    public TimeSpan? Duration { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>
    /// The error message if the execution failed, or null on success.
    /// </summary>
    public string? ErrorMessage { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>
    /// How the execution was triggered ("Schedule" or "Manual").
    /// </summary>
    public string? TriggeredBy { [UsedImplicitly] get; [UsedImplicitly] init; }
}
