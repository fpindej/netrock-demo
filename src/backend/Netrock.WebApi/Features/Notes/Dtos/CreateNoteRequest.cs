using JetBrains.Annotations;
using Netrock.Domain.Entities;

namespace Netrock.WebApi.Features.Notes.Dtos;

/// <summary>
/// Represents a request to create a new note.
/// </summary>
[UsedImplicitly]
public class CreateNoteRequest
{
    /// <summary>
    /// The title of the note.
    /// </summary>
    public string Title { get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// The content of the note.
    /// </summary>
    public string Content { get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// The category of the note.
    /// </summary>
    public NoteCategory Category { get; [UsedImplicitly] init; }
}
