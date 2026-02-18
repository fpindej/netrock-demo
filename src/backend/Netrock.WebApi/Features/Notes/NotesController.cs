using Microsoft.AspNetCore.Mvc;
using Netrock.Application.Features.Notes;
using Netrock.Application.Identity;
using Netrock.WebApi.Features.Notes.Dtos;
using Netrock.WebApi.Shared;

namespace Netrock.WebApi.Features.Notes;

/// <summary>
/// CRUD endpoints for user-owned notes and note analytics.
/// </summary>
[Tags("Notes")]
public class NotesController(INotesService notesService, IUserContext userContext) : ApiController
{
    /// <summary>
    /// Gets all notes for the current user.
    /// </summary>
    /// <returns>A list of notes ordered by pinned status and creation date.</returns>
    /// <response code="200">Returns the user's notes.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<NoteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<NoteResponse>>> GetNotes(CancellationToken cancellationToken)
    {
        var userId = userContext.AuthenticatedUserId;
        var result = await notesService.GetNotesAsync(userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return Ok(result.Value.Select(n => n.ToResponse()).ToList());
    }

    /// <summary>
    /// Gets a single note by ID.
    /// </summary>
    /// <param name="id">The note ID.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The requested note.</returns>
    /// <response code="200">Returns the note.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If the note was not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NoteResponse>> GetNote(Guid id, CancellationToken cancellationToken)
    {
        var userId = userContext.AuthenticatedUserId;
        var result = await notesService.GetNoteAsync(userId, id, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return Ok(result.Value.ToResponse());
    }

    /// <summary>
    /// Creates a new note.
    /// </summary>
    /// <param name="request">The note creation request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created note.</returns>
    /// <response code="201">Note created successfully.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<NoteResponse>> CreateNote(
        [FromBody] CreateNoteRequest request,
        CancellationToken cancellationToken)
    {
        var userId = userContext.AuthenticatedUserId;
        var result = await notesService.CreateNoteAsync(userId, request.ToInput(), cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return CreatedAtAction(nameof(GetNote), new { id = result.Value.Id }, result.Value.ToResponse());
    }

    /// <summary>
    /// Updates an existing note.
    /// </summary>
    /// <param name="id">The note ID.</param>
    /// <param name="request">The note update request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated note.</returns>
    /// <response code="200">Note updated successfully.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If the note was not found.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NoteResponse>> UpdateNote(
        Guid id,
        [FromBody] UpdateNoteRequest request,
        CancellationToken cancellationToken)
    {
        var userId = userContext.AuthenticatedUserId;
        var result = await notesService.UpdateNoteAsync(userId, id, request.ToInput(), cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return Ok(result.Value.ToResponse());
    }

    /// <summary>
    /// Deletes a note.
    /// </summary>
    /// <param name="id">The note ID.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <response code="204">Note deleted successfully.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If the note was not found.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteNote(Guid id, CancellationToken cancellationToken)
    {
        var userId = userContext.AuthenticatedUserId;
        var result = await notesService.DeleteNoteAsync(userId, id, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return NoContent();
    }

    /// <summary>
    /// Gets aggregated statistics for the current user's notes.
    /// </summary>
    /// <returns>Notes statistics including counts and recent notes.</returns>
    /// <response code="200">Returns the statistics.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(NotesStatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<NotesStatsResponse>> GetStats(CancellationToken cancellationToken)
    {
        var userId = userContext.AuthenticatedUserId;
        var result = await notesService.GetStatsAsync(userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return Ok(result.Value.ToResponse());
    }
}
