namespace Netrock.Application.Features.Contacts;

/// <summary>
/// Output for paginated contact list results.
/// </summary>
/// <param name="Contacts">The list of contacts for the current page.</param>
/// <param name="TotalCount">The total number of contacts matching the query.</param>
/// <param name="PageNumber">The current page number.</param>
/// <param name="PageSize">The number of items per page.</param>
public record ContactListOutput(
    IReadOnlyList<ContactOutput> Contacts,
    int TotalCount,
    int PageNumber,
    int PageSize
);
