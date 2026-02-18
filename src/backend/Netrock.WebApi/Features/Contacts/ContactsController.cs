using Microsoft.AspNetCore.Mvc;
using Netrock.Application.Features.Audit;
using Netrock.Application.Features.Contacts;
using Netrock.Application.Identity;
using Netrock.WebApi.Features.Contacts.Dtos;
using Netrock.WebApi.Shared;

namespace Netrock.WebApi.Features.Contacts;

/// <summary>
/// CRUD endpoints for user-owned contacts, pipeline analytics, and per-contact audit trail.
/// </summary>
[Tags("Contacts")]
public class ContactsController(
    IContactsService contactsService,
    IAuditService auditService,
    IUserContext userContext) : ApiController
{
    /// <summary>
    /// Gets all contacts for the current user.
    /// </summary>
    /// <returns>A list of contacts ordered by favorite status and creation date.</returns>
    /// <response code="200">Returns the user's contacts.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ContactResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ContactResponse>>> GetContacts(CancellationToken cancellationToken)
    {
        var userId = userContext.AuthenticatedUserId;
        var result = await contactsService.GetContactsAsync(userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return Ok(result.Value.Select(c => c.ToResponse()).ToList());
    }

    /// <summary>
    /// Gets a single contact by ID.
    /// </summary>
    /// <param name="id">The contact ID.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The requested contact.</returns>
    /// <response code="200">Returns the contact.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If the contact was not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactResponse>> GetContact(Guid id, CancellationToken cancellationToken)
    {
        var userId = userContext.AuthenticatedUserId;
        var result = await contactsService.GetContactAsync(userId, id, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return Ok(result.Value.ToResponse());
    }

    /// <summary>
    /// Creates a new contact.
    /// </summary>
    /// <param name="request">The contact creation request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created contact.</returns>
    /// <response code="201">Contact created successfully.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ContactResponse>> CreateContact(
        [FromBody] CreateContactRequest request,
        CancellationToken cancellationToken)
    {
        var userId = userContext.AuthenticatedUserId;
        var result = await contactsService.CreateContactAsync(userId, request.ToInput(), cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return CreatedAtAction(nameof(GetContact), new { id = result.Value.Id }, result.Value.ToResponse());
    }

    /// <summary>
    /// Updates an existing contact.
    /// </summary>
    /// <param name="id">The contact ID.</param>
    /// <param name="request">The contact update request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated contact.</returns>
    /// <response code="200">Contact updated successfully.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If the contact was not found.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactResponse>> UpdateContact(
        Guid id,
        [FromBody] UpdateContactRequest request,
        CancellationToken cancellationToken)
    {
        var userId = userContext.AuthenticatedUserId;
        var result = await contactsService.UpdateContactAsync(userId, id, request.ToInput(), cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return Ok(result.Value.ToResponse());
    }

    /// <summary>
    /// Deletes a contact.
    /// </summary>
    /// <param name="id">The contact ID.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <response code="204">Contact deleted successfully.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If the contact was not found.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteContact(Guid id, CancellationToken cancellationToken)
    {
        var userId = userContext.AuthenticatedUserId;
        var result = await contactsService.DeleteContactAsync(userId, id, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return NoContent();
    }

    /// <summary>
    /// Gets aggregated pipeline statistics for the current user's contacts.
    /// </summary>
    /// <returns>Pipeline statistics including counts, values, and recent contacts.</returns>
    /// <response code="200">Returns the statistics.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(ContactsStatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ContactsStatsResponse>> GetStats(CancellationToken cancellationToken)
    {
        var userId = userContext.AuthenticatedUserId;
        var result = await contactsService.GetStatsAsync(userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return Ok(result.Value.ToResponse());
    }

    /// <summary>
    /// Gets the audit trail for a specific contact.
    /// </summary>
    /// <param name="id">The contact ID.</param>
    /// <param name="page">The page number (1-based). Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A paginated list of audit events for the contact.</returns>
    /// <response code="200">Returns the contact audit trail.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If the contact was not found.</response>
    [HttpGet("{id:guid}/audit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetContactAudit(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = userContext.AuthenticatedUserId;

        // Verify the contact belongs to the current user
        var contactResult = await contactsService.GetContactAsync(userId, id, cancellationToken);
        if (!contactResult.IsSuccess)
        {
            return ProblemFactory.Create(contactResult.Error, contactResult.ErrorType);
        }

        var auditResult = await auditService.GetEntityAuditEventsAsync("Contact", id, page, pageSize, cancellationToken);
        return Ok(auditResult);
    }
}
