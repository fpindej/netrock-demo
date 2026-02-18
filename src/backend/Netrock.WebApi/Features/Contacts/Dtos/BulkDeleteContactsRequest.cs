namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Request to delete multiple contacts at once.
/// </summary>
public class BulkDeleteContactsRequest
{
    /// <summary>
    /// The IDs of the contacts to delete. Ignored when <see cref="All"/> is <c>true</c>.
    /// </summary>
    public required List<Guid> Ids { get; init; }

    /// <summary>
    /// When <c>true</c>, deletes all contacts belonging to the current user regardless of <see cref="Ids"/>.
    /// </summary>
    public bool All { get; init; }
}
