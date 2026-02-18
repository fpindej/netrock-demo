using JetBrains.Annotations;
using Netrock.Domain.Entities;

namespace Netrock.WebApi.Features.Notes.Dtos;

/// <summary>
/// Represents a request to update an existing note.
/// </summary>
[UsedImplicitly]
public class UpdateNoteRequest
{
    /// <summary>
    /// The new title of the note.
    /// </summary>
    public string Title { get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// The new content of the note.
    /// </summary>
    public string Content { get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// The new category of the note.
    /// </summary>
    public NoteCategory Category { get; [UsedImplicitly] init; }

    /// <summary>
    /// Whether the note should be pinned.
    /// </summary>
    public bool IsPinned { get; [UsedImplicitly] init; }
}
