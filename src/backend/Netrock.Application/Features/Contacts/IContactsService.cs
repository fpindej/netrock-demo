using Netrock.Application.Features.Contacts.Dtos;
using Netrock.Shared;

namespace Netrock.Application.Features.Contacts;

/// <summary>
/// Provides CRUD and analytics operations for user-owned contacts in the sales pipeline.
/// </summary>
public interface IContactsService
{
    /// <summary>
    /// Gets a paginated list of contacts for the specified user, optionally filtered and sorted.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="search">Optional search term to filter by name, email, company, or phone.</param>
    /// <param name="sortBy">Sort order for the results.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the paginated contacts and total count.</returns>
    Task<Result<(List<ContactOutput> Items, int TotalCount)>> GetContactsAsync(Guid userId, int pageNumber, int pageSize, string? search, string? sortBy, CancellationToken ct);

    /// <summary>
    /// Gets a single contact by ID, scoped to the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="contactId">The contact ID.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the contact, or a not-found failure.</returns>
    Task<Result<ContactOutput>> GetContactAsync(Guid userId, Guid contactId, CancellationToken ct);

    /// <summary>
    /// Creates a new contact for the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="input">The creation input.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the created contact.</returns>
    Task<Result<ContactOutput>> CreateContactAsync(Guid userId, CreateContactInput input, CancellationToken ct);

    /// <summary>
    /// Updates an existing contact, scoped to the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="contactId">The contact ID.</param>
    /// <param name="input">The update input.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the updated contact, or a not-found failure.</returns>
    Task<Result<ContactOutput>> UpdateContactAsync(Guid userId, Guid contactId, UpdateContactInput input, CancellationToken ct);

    /// <summary>
    /// Deletes a contact, scoped to the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="contactId">The contact ID.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result indicating success or a not-found failure.</returns>
    Task<Result> DeleteContactAsync(Guid userId, Guid contactId, CancellationToken ct);

    /// <summary>
    /// Gets aggregated pipeline statistics for the specified user's contacts.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the pipeline statistics.</returns>
    Task<Result<ContactsStatsOutput>> GetStatsAsync(Guid userId, CancellationToken ct);

    /// <summary>
    /// Deletes multiple contacts, scoped to the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="contactIds">The contact IDs to delete.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the number of deleted contacts.</returns>
    Task<Result<int>> BulkDeleteAsync(Guid userId, List<Guid> contactIds, CancellationToken ct);

    /// <summary>
    /// Deletes all contacts belonging to the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the number of deleted contacts.</returns>
    Task<Result<int>> DeleteAllAsync(Guid userId, CancellationToken ct);

    /// <summary>
    /// Toggles the favorite status of a contact.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="contactId">The contact ID.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the updated contact.</returns>
    Task<Result<ContactOutput>> ToggleFavoriteAsync(Guid userId, Guid contactId, CancellationToken ct);

    /// <summary>
    /// Seeds sample contacts for the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="count">The number of contacts to generate (1–50).</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the created contacts.</returns>
    Task<Result<List<ContactOutput>>> SeedAsync(Guid userId, int count, CancellationToken ct);
}
