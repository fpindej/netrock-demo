using Netrock.Domain.Entities;

namespace Netrock.Application.Features.Notes.Dtos;

/// <summary>
/// Output containing aggregated statistics about a user's notes.
/// </summary>
/// <param name="TotalCount">Total number of notes.</param>
/// <param name="PinnedCount">Number of pinned notes.</param>
/// <param name="ByCategory">Note count per category.</param>
/// <param name="RecentNotes">The last 5 notes ordered by creation date.</param>
public record NotesStatsOutput(
    int TotalCount,
    int PinnedCount,
    Dictionary<NoteCategory, int> ByCategory,
    List<NoteOutput> RecentNotes);
