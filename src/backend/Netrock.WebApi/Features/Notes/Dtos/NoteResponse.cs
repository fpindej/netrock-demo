using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Notes.Dtos;

/// <summary>
/// Represents a note in API responses.
/// </summary>
public class NoteResponse
{
    /// <summary>
    /// The unique identifier of the note.
    /// </summary>
    public Guid Id { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>
    /// The title of the note.
    /// </summary>
    public string Title { [UsedImplicitly] get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// The content of the note.
    /// </summary>
    public string Content { [UsedImplicitly] get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// The category of the note.
    /// </summary>
    public string Category { [UsedImplicitly] get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// Whether the note is pinned.
    /// </summary>
    public bool IsPinned { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>
    /// When the note was created.
    /// </summary>
    public DateTime CreatedAt { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>
    /// When the note was last updated, or null if never updated.
    /// </summary>
    public DateTime? UpdatedAt { [UsedImplicitly] get; [UsedImplicitly] init; }
}
