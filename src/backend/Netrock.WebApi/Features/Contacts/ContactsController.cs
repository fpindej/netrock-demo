using Microsoft.AspNetCore.Mvc;
using Netrock.Application.Features.Contacts;
using Netrock.WebApi.Features.Contacts.Dtos;
using Netrock.WebApi.Shared;

namespace Netrock.WebApi.Features.Contacts;

/// <summary>
/// Endpoints for managing CRM contacts.
/// All endpoints require authentication and operate on the current user's contacts only.
/// </summary>
[Tags("Contacts")]
public class ContactsController(IContactService contactService) : ApiController
{
    /// <summary>
    /// Gets a paginated list of the current user's contacts with optional search and filtering.
    /// </summary>
    /// <param name="request">Pagination, search, and filter parameters</param>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>A paginated list of contacts</returns>
    /// <response code="200">Returns the paginated contact list</response>
    /// <response code="400">If the pagination parameters are invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpGet]
    [ProducesResponseType(typeof(ContactListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ContactListResponse>> GetContacts(
        [FromQuery] GetContactsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await contactService.GetContactsAsync(request.ToInput(), cancellationToken);
        return Ok(result.ToResponse());
    }

    /// <summary>
    /// Gets a single contact by ID.
    /// </summary>
    /// <param name="id">The contact ID</param>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>The contact details</returns>
    /// <response code="200">Returns the contact</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the contact was not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactResponse>> GetContact(Guid id, CancellationToken cancellationToken)
    {
        var result = await contactService.GetContactByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return Ok(result.Value.ToResponse());
    }

    /// <summary>
    /// Creates a new contact for the current user.
    /// </summary>
    /// <param name="request">The contact creation request</param>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>The created contact</returns>
    /// <response code="201">Contact created successfully</response>
    /// <response code="400">If the request validation fails</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpPost]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ContactResponse>> CreateContact(
        [FromBody] CreateContactRequest request,
        CancellationToken cancellationToken)
    {
        var result = await contactService.CreateContactAsync(request.ToInput(), cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return CreatedAtAction(nameof(GetContact), new { id = result.Value.Id }, result.Value.ToResponse());
    }

    /// <summary>
    /// Updates an existing contact owned by the current user.
    /// </summary>
    /// <param name="id">The contact ID</param>
    /// <param name="request">The contact update request</param>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>The updated contact</returns>
    /// <response code="200">Contact updated successfully</response>
    /// <response code="400">If the request validation fails</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the contact was not found</response>
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
        var result = await contactService.UpdateContactAsync(id, request.ToInput(), cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return Ok(result.Value.ToResponse());
    }

    /// <summary>
    /// Soft-deletes a contact owned by the current user.
    /// </summary>
    /// <param name="id">The contact ID</param>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Contact deleted successfully</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the contact was not found</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteContact(Guid id, CancellationToken cancellationToken)
    {
        var result = await contactService.DeleteContactAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return NoContent();
    }

    /// <summary>
    /// Generates a batch of realistic sample contacts using Bogus for demo purposes.
    /// </summary>
    /// <param name="request">The generation request specifying the number of contacts</param>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>The number of contacts generated</returns>
    /// <response code="200">Sample contacts generated successfully</response>
    /// <response code="400">If the count is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpPost("generate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> GenerateSampleContacts(
        [FromBody] GenerateContactsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await contactService.GenerateSampleContactsAsync(request.Count, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return Ok(new { generated = result.Value });
    }

    /// <summary>
    /// Gets aggregate pipeline statistics for the current user's contacts.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>Pipeline statistics including status counts, value totals, and source breakdown</returns>
    /// <response code="200">Returns the contact statistics</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(ContactStatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ContactStatsResponse>> GetStats(CancellationToken cancellationToken)
    {
        var result = await contactService.GetStatsAsync(cancellationToken);
        return Ok(result.ToResponse());
    }
}
