using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Netrock.Infrastructure.Features.Authentication.Models;

namespace Netrock.Infrastructure.Features.Authentication.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="TwoFactorChallenge"/> entity in the <c>auth</c> schema.
/// </summary>
internal class TwoFactorChallengeConfiguration : IEntityTypeConfiguration<TwoFactorChallenge>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TwoFactorChallenge> builder)
    {
        builder.ToTable("TwoFactorChallenges", "auth");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.FailedAttempts)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.IsUsed)
            .HasColumnName("Used")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.RememberMe)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.TokenHash);
        builder.HasIndex(x => x.UserId);
    }
}
