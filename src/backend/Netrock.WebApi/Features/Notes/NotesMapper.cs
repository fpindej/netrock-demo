using Netrock.Application.Features.Notes.Dtos;
using Netrock.WebApi.Features.Notes.Dtos;

namespace Netrock.WebApi.Features.Notes;

/// <summary>
/// Maps between Notes Application layer DTOs and WebApi request/response DTOs.
/// </summary>
internal static class NotesMapper
{
    /// <summary>
    /// Maps a <see cref="NoteOutput"/> to a <see cref="NoteResponse"/>.
    /// </summary>
    public static NoteResponse ToResponse(this NoteOutput output) => new()
    {
        Id = output.Id,
        Title = output.Title,
        Content = output.Content,
        Category = output.Category.ToString(),
        IsPinned = output.IsPinned,
        CreatedAt = output.CreatedAt,
        UpdatedAt = output.UpdatedAt
    };

    /// <summary>
    /// Maps a <see cref="NotesStatsOutput"/> to a <see cref="NotesStatsResponse"/>.
    /// </summary>
    public static NotesStatsResponse ToResponse(this NotesStatsOutput output) => new()
    {
        TotalCount = output.TotalCount,
        PinnedCount = output.PinnedCount,
        ByCategory = output.ByCategory.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value),
        RecentNotes = output.RecentNotes.Select(n => n.ToResponse()).ToList()
    };

    /// <summary>
    /// Maps a <see cref="CreateNoteRequest"/> to a <see cref="CreateNoteInput"/>.
    /// </summary>
    public static CreateNoteInput ToInput(this CreateNoteRequest request) =>
        new(request.Title, request.Content, request.Category);

    /// <summary>
    /// Maps an <see cref="UpdateNoteRequest"/> to an <see cref="UpdateNoteInput"/>.
    /// </summary>
    public static UpdateNoteInput ToInput(this UpdateNoteRequest request) =>
        new(request.Title, request.Content, request.Category, request.IsPinned);
}
