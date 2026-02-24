using Netrock.Shared;

namespace Netrock.Application.Features.Contacts;

/// <summary>
/// Provides operations for managing CRM contacts.
/// All operations are scoped to the current authenticated user's contacts.
/// </summary>
public interface IContactService
{
    /// <summary>
    /// Gets a paginated list of contacts for the current user, with optional search and filtering.
    /// </summary>
    /// <param name="input">The query parameters including pagination, search, and filters.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A paginated list of contacts.</returns>
    Task<ContactListOutput> GetContactsAsync(GetContactsInput input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single contact by ID, scoped to the current user.
    /// </summary>
    /// <param name="id">The contact ID.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The contact details, or a failure if not found.</returns>
    Task<Result<ContactOutput>> GetContactByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new contact for the current user.
    /// </summary>
    /// <param name="input">The contact creation input.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created contact details.</returns>
    Task<Result<ContactOutput>> CreateContactAsync(CreateContactInput input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing contact owned by the current user.
    /// </summary>
    /// <param name="id">The contact ID.</param>
    /// <param name="input">The contact update input.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated contact details, or a failure if not found.</returns>
    Task<Result<ContactOutput>> UpdateContactAsync(Guid id, UpdateContactInput input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-deletes a contact owned by the current user.
    /// </summary>
    /// <param name="id">The contact ID.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Success or failure with an error message.</returns>
    Task<Result> DeleteContactAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a batch of realistic sample contacts for the current user using Bogus.
    /// </summary>
    /// <param name="count">The number of contacts to generate (capped at 100).</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of contacts generated.</returns>
    Task<Result<int>> GenerateSampleContactsAsync(int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets aggregate pipeline statistics for the current user's contacts.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The contact pipeline statistics.</returns>
    Task<ContactStatsOutput> GetStatsAsync(CancellationToken cancellationToken = default);
}
