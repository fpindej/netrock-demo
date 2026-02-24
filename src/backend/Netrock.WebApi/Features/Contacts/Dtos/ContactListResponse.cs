using Netrock.WebApi.Shared;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Paginated response containing a list of contact records.
/// </summary>
public class ContactListResponse : PaginatedResponse
{
    /// <summary>
    /// The contacts for the current page.
    /// </summary>
    public IReadOnlyList<ContactResponse> Items { get; init; } = [];
}
