using Netrock.Application.Features.Contacts.Dtos;
using Netrock.Shared;

namespace Netrock.Application.Features.Contacts;

/// <summary>
/// Provides CRUD and analytics operations for user-owned contacts in the sales pipeline.
/// </summary>
public interface IContactsService
{
    /// <summary>
    /// Gets all contacts for the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the list of contacts.</returns>
    Task<Result<List<ContactOutput>>> GetContactsAsync(Guid userId, CancellationToken ct);

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
}
