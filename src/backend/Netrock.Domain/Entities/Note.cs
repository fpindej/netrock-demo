namespace Netrock.Domain.Entities;

/// <summary>
/// Represents a user-owned note with a title, content, category, and pinned state.
/// </summary>
public class Note : BaseEntity
{
    /// <summary>
    /// Gets the title of the note.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the content body of the note.
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the category of the note.
    /// </summary>
    public NoteCategory Category { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the note is pinned.
    /// </summary>
    public bool IsPinned { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who owns this note.
    /// </summary>
    public Guid UserId { get; private init; }

    /// <summary>
    /// Protected constructor for EF Core.
    /// </summary>
    protected Note()
    {
    }

    /// <summary>
    /// Creates a new note for the specified user.
    /// </summary>
    /// <param name="userId">The owner's user ID.</param>
    /// <param name="title">The note title.</param>
    /// <param name="content">The note content.</param>
    /// <param name="category">The note category.</param>
    public Note(Guid userId, string title, string content, NoteCategory category)
    {
        UserId = userId;
        Title = title;
        Content = content;
        Category = category;
    }

    /// <summary>
    /// Updates the note's mutable properties.
    /// </summary>
    /// <param name="title">The new title.</param>
    /// <param name="content">The new content.</param>
    /// <param name="category">The new category.</param>
    /// <param name="isPinned">Whether the note should be pinned.</param>
    public void Update(string title, string content, NoteCategory category, bool isPinned)
    {
        Title = title;
        Content = content;
        Category = category;
        IsPinned = isPinned;
    }
}
