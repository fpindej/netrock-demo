using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Netrock.Infrastructure.Features.Jobs.Models;

namespace Netrock.Infrastructure.Features.Jobs.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="JobExecution"/> entity in the <c>hangfire</c> schema.
/// </summary>
internal class JobExecutionConfiguration : IEntityTypeConfiguration<JobExecution>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<JobExecution> builder)
    {
        builder.ToTable("jobexecutions", "hangfire");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RecurringJobId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.HangfireJobId)
            .HasMaxLength(255);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.StartedAt)
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(4000);

        builder.Property(x => x.TriggeredBy)
            .HasMaxLength(20);

        builder.HasIndex(x => new { x.RecurringJobId, x.StartedAt });

        builder.HasMany(x => x.LogEntries)
            .WithOne()
            .HasForeignKey(x => x.JobExecutionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
