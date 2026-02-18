using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Notes.Dtos;

/// <summary>
/// Represents aggregated notes statistics in API responses.
/// </summary>
public class NotesStatsResponse
{
    /// <summary>
    /// Total number of notes.
    /// </summary>
    public int TotalCount { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>
    /// Number of pinned notes.
    /// </summary>
    public int PinnedCount { [UsedImplicitly] get; [UsedImplicitly] init; }

    /// <summary>
    /// Note count per category.
    /// </summary>
    public Dictionary<string, int> ByCategory { [UsedImplicitly] get; [UsedImplicitly] init; } = new();

    /// <summary>
    /// The last 5 notes ordered by creation date.
    /// </summary>
    public List<NoteResponse> RecentNotes { [UsedImplicitly] get; [UsedImplicitly] init; } = [];
}
