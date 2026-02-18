using Netrock.Domain.Entities;

namespace Netrock.Application.Features.Notes.Dtos;

/// <summary>
/// Input for updating an existing note.
/// </summary>
/// <param name="Title">The new title.</param>
/// <param name="Content">The new content.</param>
/// <param name="Category">The new category.</param>
/// <param name="IsPinned">Whether the note should be pinned.</param>
public record UpdateNoteInput(string Title, string Content, NoteCategory Category, bool IsPinned);
