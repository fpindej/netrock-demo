namespace Netrock.Application.Features.Jobs;

/// <summary>
/// Well-known status values for <see cref="Dtos.JobExecutionSummaryOutput.Status"/>.
/// </summary>
public static class JobExecutionStatus
{
    public const string Running = "Running";
    public const string Succeeded = "Succeeded";
    public const string Failed = "Failed";
}

/// <summary>
/// Well-known log level values for <see cref="Dtos.JobExecutionLogEntryOutput.Level"/>.
/// </summary>
public static class JobExecutionLogLevel
{
    public const string Info = "Info";
    public const string Warning = "Warning";
    public const string Error = "Error";
}

/// <summary>
/// Well-known trigger source values for <see cref="Dtos.JobExecutionSummaryOutput.TriggeredBy"/>.
/// </summary>
public static class JobExecutionTrigger
{
    public const string Schedule = "Schedule";
    public const string Manual = "Manual";
}
