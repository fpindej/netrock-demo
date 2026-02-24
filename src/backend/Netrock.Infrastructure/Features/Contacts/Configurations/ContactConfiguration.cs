using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Netrock.Domain.Entities;
using Netrock.Infrastructure.Persistence.Configurations;

namespace Netrock.Infrastructure.Features.Contacts.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Contact"/> entity.
/// Maps to the "contacts" table with appropriate column constraints and indexes.
/// </summary>
internal class ContactConfiguration : BaseEntityConfiguration<Contact>
{
    /// <inheritdoc />
    protected override void ConfigureEntity(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("contacts");

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.Company)
            .HasMaxLength(200);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Source)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Value)
            .HasPrecision(18, 2);

        builder.Property(c => c.Notes)
            .HasMaxLength(2000);

        builder.Property(c => c.Phone)
            .HasMaxLength(20);

        builder.Property(c => c.OwnerId)
            .IsRequired();

        builder.HasIndex(c => c.OwnerId);
        builder.HasIndex(c => c.Email);
        builder.HasIndex(c => c.Status);
    }
}
