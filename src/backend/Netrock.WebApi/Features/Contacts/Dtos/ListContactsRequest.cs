using System.ComponentModel;
using Netrock.WebApi.Shared;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Request parameters for listing contacts with pagination and optional search.
/// </summary>
public class ListContactsRequest : PaginatedRequest
{
    /// <summary>
    /// Optional search term to filter contacts by name, email, company, or phone.
    /// </summary>
    [Description("Optional search term to filter contacts by name, email, company, or phone.")]
    public string? Search { get; set; }
}
