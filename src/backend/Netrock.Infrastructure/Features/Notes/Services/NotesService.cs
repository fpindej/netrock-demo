using Microsoft.EntityFrameworkCore;
using Netrock.Application.Features.Notes;
using Netrock.Application.Features.Notes.Dtos;
using Netrock.Domain.Entities;
using Netrock.Infrastructure.Persistence;
using Netrock.Shared;

namespace Netrock.Infrastructure.Features.Notes.Services;

/// <summary>
/// EF Core implementation of <see cref="INotesService"/>.
/// All queries are scoped to the requesting user for data isolation.
/// </summary>
internal class NotesService(NetrockDbContext dbContext) : INotesService
{
    /// <inheritdoc />
    public async Task<Result<List<NoteOutput>>> GetNotesAsync(Guid userId, CancellationToken ct)
    {
        var notes = await dbContext.Notes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.IsPinned)
            .ThenByDescending(n => n.CreatedAt)
            .Select(n => ToOutput(n))
            .ToListAsync(ct);

        return Result<List<NoteOutput>>.Success(notes);
    }

    /// <inheritdoc />
    public async Task<Result<NoteOutput>> GetNoteAsync(Guid userId, Guid noteId, CancellationToken ct)
    {
        var note = await dbContext.Notes
            .Where(n => n.UserId == userId && n.Id == noteId)
            .FirstOrDefaultAsync(ct);

        if (note is null)
        {
            return Result<NoteOutput>.Failure("Note not found.", ErrorType.NotFound);
        }

        return Result<NoteOutput>.Success(ToOutput(note));
    }

    /// <inheritdoc />
    public async Task<Result<NoteOutput>> CreateNoteAsync(Guid userId, CreateNoteInput input, CancellationToken ct)
    {
        var note = new Note(userId, input.Title, input.Content, input.Category);

        dbContext.Notes.Add(note);
        await dbContext.SaveChangesAsync(ct);

        return Result<NoteOutput>.Success(ToOutput(note));
    }

    /// <inheritdoc />
    public async Task<Result<NoteOutput>> UpdateNoteAsync(Guid userId, Guid noteId, UpdateNoteInput input, CancellationToken ct)
    {
        var note = await dbContext.Notes
            .Where(n => n.UserId == userId && n.Id == noteId)
            .FirstOrDefaultAsync(ct);

        if (note is null)
        {
            return Result<NoteOutput>.Failure("Note not found.", ErrorType.NotFound);
        }

        note.Update(input.Title, input.Content, input.Category, input.IsPinned);
        await dbContext.SaveChangesAsync(ct);

        return Result<NoteOutput>.Success(ToOutput(note));
    }

    /// <inheritdoc />
    public async Task<Result> DeleteNoteAsync(Guid userId, Guid noteId, CancellationToken ct)
    {
        var note = await dbContext.Notes
            .Where(n => n.UserId == userId && n.Id == noteId)
            .FirstOrDefaultAsync(ct);

        if (note is null)
        {
            return Result.Failure("Note not found.", ErrorType.NotFound);
        }

        note.SoftDelete();
        await dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result<NotesStatsOutput>> GetStatsAsync(Guid userId, CancellationToken ct)
    {
        var notes = await dbContext.Notes
            .Where(n => n.UserId == userId)
            .ToListAsync(ct);

        var totalCount = notes.Count;
        var pinnedCount = notes.Count(n => n.IsPinned);

        var byCategory = Enum.GetValues<NoteCategory>()
            .ToDictionary(c => c, c => notes.Count(n => n.Category == c));

        var recentNotes = notes
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .Select(n => ToOutput(n))
            .ToList();

        var stats = new NotesStatsOutput(totalCount, pinnedCount, byCategory, recentNotes);
        return Result<NotesStatsOutput>.Success(stats);
    }

    private static NoteOutput ToOutput(Note note) => new(
        note.Id,
        note.Title,
        note.Content,
        note.Category,
        note.IsPinned,
        note.CreatedAt,
        note.UpdatedAt);
}
