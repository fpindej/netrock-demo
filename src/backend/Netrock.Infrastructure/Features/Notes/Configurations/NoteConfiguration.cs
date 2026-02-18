using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Netrock.Domain.Entities;
using Netrock.Infrastructure.Persistence.Configurations;

namespace Netrock.Infrastructure.Features.Notes.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Note"/> entity.
/// </summary>
internal class NoteConfiguration : BaseEntityConfiguration<Note>
{
    /// <inheritdoc />
    protected override void ConfigureEntity(EntityTypeBuilder<Note> builder)
    {
        builder.ToTable("notes");

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Content)
            .IsRequired()
            .HasMaxLength(10000);

        builder.Property(n => n.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasComment("Enum: Personal, Work, Ideas");

        builder.Property(n => n.IsPinned)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(n => n.UserId)
            .IsRequired();

        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.Category);
    }
}
