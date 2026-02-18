using Netrock.Application.Features.Notes.Dtos;
using Netrock.Shared;

namespace Netrock.Application.Features.Notes;

/// <summary>
/// Provides CRUD and analytics operations for user-owned notes.
/// </summary>
public interface INotesService
{
    /// <summary>
    /// Gets all notes for the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the list of notes.</returns>
    Task<Result<List<NoteOutput>>> GetNotesAsync(Guid userId, CancellationToken ct);

    /// <summary>
    /// Gets a single note by ID, scoped to the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="noteId">The note ID.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the note, or a not-found failure.</returns>
    Task<Result<NoteOutput>> GetNoteAsync(Guid userId, Guid noteId, CancellationToken ct);

    /// <summary>
    /// Creates a new note for the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="input">The creation input.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the created note.</returns>
    Task<Result<NoteOutput>> CreateNoteAsync(Guid userId, CreateNoteInput input, CancellationToken ct);

    /// <summary>
    /// Updates an existing note, scoped to the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="noteId">The note ID.</param>
    /// <param name="input">The update input.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the updated note, or a not-found failure.</returns>
    Task<Result<NoteOutput>> UpdateNoteAsync(Guid userId, Guid noteId, UpdateNoteInput input, CancellationToken ct);

    /// <summary>
    /// Deletes a note, scoped to the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="noteId">The note ID.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result indicating success or a not-found failure.</returns>
    Task<Result> DeleteNoteAsync(Guid userId, Guid noteId, CancellationToken ct);

    /// <summary>
    /// Gets aggregated statistics for the specified user's notes.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the notes statistics.</returns>
    Task<Result<NotesStatsOutput>> GetStatsAsync(Guid userId, CancellationToken ct);
}
