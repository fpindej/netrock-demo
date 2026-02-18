using System.ComponentModel;
using System.Text.Json.Serialization;
using Netrock.WebApi.Shared;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Request parameters for listing contacts with pagination, search, and sorting.
/// </summary>
public class ListContactsRequest : PaginatedRequest
{
    /// <summary>
    /// Optional search term to filter contacts by name, email, company, or phone.
    /// </summary>
    [Description("Optional search term to filter contacts by name, email, company, or phone.")]
    public string? Search { get; set; }

    /// <summary>
    /// Sort order for the contact list. Defaults to <see cref="ContactSortBy.Newest"/>.
    /// </summary>
    [Description("Sort order: Newest, NameAsc, NameDesc, Favorites.")]
    public ContactSortBy SortBy { get; set; } = ContactSortBy.Newest;
}

/// <summary>
/// Defines the available sort orders for the contact list.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ContactSortBy
{
    /// <summary>Most recently created first.</summary>
    Newest,

    /// <summary>Name A → Z.</summary>
    NameAsc,

    /// <summary>Name Z → A.</summary>
    NameDesc,

    /// <summary>Favorites first, then by newest.</summary>
    Favorites
}
