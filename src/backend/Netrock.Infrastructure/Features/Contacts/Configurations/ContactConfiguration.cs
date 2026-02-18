using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Netrock.Domain.Entities;
using Netrock.Infrastructure.Persistence.Configurations;

namespace Netrock.Infrastructure.Features.Contacts.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Contact"/> entity.
/// </summary>
internal class ContactConfiguration : BaseEntityConfiguration<Contact>
{
    /// <inheritdoc />
    protected override void ConfigureEntity(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("contacts");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Email)
            .HasMaxLength(256);

        builder.Property(c => c.Company)
            .HasMaxLength(200);

        builder.Property(c => c.Phone)
            .HasMaxLength(50);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasComment("Enum: Lead, Prospect, Customer, Churned");

        builder.Property(c => c.Source)
            .IsRequired()
            .HasConversion<int>()
            .HasComment("Enum: Website, Referral, Social, Email, Phone, Other");

        builder.Property(c => c.Value)
            .HasPrecision(18, 2);

        builder.Property(c => c.Notes)
            .HasMaxLength(10000);

        builder.Property(c => c.IsFavorite)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.Source);
    }
}
