using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Netrock.Infrastructure.Features.Jobs.Models;

namespace Netrock.Infrastructure.Features.Jobs.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="JobExecutionLogEntry"/> entity in the <c>hangfire</c> schema.
/// </summary>
internal class JobExecutionLogEntryConfiguration : IEntityTypeConfiguration<JobExecutionLogEntry>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<JobExecutionLogEntry> builder)
    {
        builder.ToTable("jobexecutionlogentries", "hangfire");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.JobExecutionId)
            .IsRequired();

        builder.Property(x => x.Timestamp)
            .IsRequired();

        builder.Property(x => x.Level)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.Category)
            .HasMaxLength(100);

        builder.HasIndex(x => x.JobExecutionId);
    }
}
