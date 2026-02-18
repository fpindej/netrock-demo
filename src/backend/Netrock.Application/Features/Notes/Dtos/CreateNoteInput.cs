using Netrock.Domain.Entities;

namespace Netrock.Application.Features.Notes.Dtos;

/// <summary>
/// Input for creating a new note.
/// </summary>
/// <param name="Title">The note title.</param>
/// <param name="Content">The note content.</param>
/// <param name="Category">The note category.</param>
public record CreateNoteInput(string Title, string Content, NoteCategory Category);
