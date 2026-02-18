namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Request to delete multiple contacts at once.
/// </summary>
public class BulkDeleteContactsRequest
{
    /// <summary>
    /// The IDs of the contacts to delete.
    /// </summary>
    public required List<Guid> Ids { get; init; }
}
