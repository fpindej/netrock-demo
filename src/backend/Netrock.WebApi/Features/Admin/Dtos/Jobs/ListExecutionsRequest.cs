using System.ComponentModel;
using Netrock.WebApi.Shared;

namespace Netrock.WebApi.Features.Admin.Dtos.Jobs;

/// <summary>
/// Request parameters for listing job executions with pagination and optional status filter.
/// </summary>
public class ListExecutionsRequest : PaginatedRequest
{
    /// <summary>
    /// Optional status filter (e.g. "Succeeded", "Failed", "Running").
    /// </summary>
    [Description("Optional status filter (e.g. Succeeded, Failed, Running).")]
    public string? Status { get; set; }
}
