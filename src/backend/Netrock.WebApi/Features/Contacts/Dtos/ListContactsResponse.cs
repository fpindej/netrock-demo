using JetBrains.Annotations;
using Netrock.WebApi.Shared;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Paginated response containing a list of contacts.
/// </summary>
public class ListContactsResponse : PaginatedResponse
{
    /// <summary>
    /// The contacts for the current page.
    /// </summary>
    [UsedImplicitly]
    public IReadOnlyList<ContactResponse> Items { get; init; } = [];
}
