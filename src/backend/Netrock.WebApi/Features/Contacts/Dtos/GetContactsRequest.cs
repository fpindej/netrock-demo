using System.ComponentModel;
using JetBrains.Annotations;
using Netrock.Domain.Entities;
using Netrock.WebApi.Shared;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Request parameters for listing contacts with optional search and filtering.
/// </summary>
public class GetContactsRequest : PaginatedRequest
{
    /// <summary>
    /// Optional search term to filter by name, email, or company.
    /// </summary>
    [Description("Optional search term to filter by name, email, or company.")]
    public string? Search { get; [UsedImplicitly] set; }

    /// <summary>
    /// Optional pipeline status filter.
    /// </summary>
    [Description("Optional pipeline status filter.")]
    public ContactStatus? Status { get; [UsedImplicitly] set; }

    /// <summary>
    /// Optional acquisition source filter.
    /// </summary>
    [Description("Optional acquisition source filter.")]
    public ContactSource? Source { get; [UsedImplicitly] set; }
}
