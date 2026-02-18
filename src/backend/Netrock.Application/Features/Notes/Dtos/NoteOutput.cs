using Netrock.Domain.Entities;

namespace Netrock.Application.Features.Notes.Dtos;

/// <summary>
/// Output representing a single note.
/// </summary>
/// <param name="Id">The note's unique identifier.</param>
/// <param name="Title">The note title.</param>
/// <param name="Content">The note content.</param>
/// <param name="Category">The note category.</param>
/// <param name="IsPinned">Whether the note is pinned.</param>
/// <param name="CreatedAt">When the note was created.</param>
/// <param name="UpdatedAt">When the note was last updated, or <c>null</c> if never updated.</param>
public record NoteOutput(
    Guid Id,
    string Title,
    string Content,
    NoteCategory Category,
    bool IsPinned,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
