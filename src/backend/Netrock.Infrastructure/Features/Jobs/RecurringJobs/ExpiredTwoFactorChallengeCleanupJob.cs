using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Netrock.Infrastructure.Persistence;

namespace Netrock.Infrastructure.Features.Jobs.RecurringJobs;

/// <summary>
/// Removes expired and consumed two-factor authentication challenges from the database.
/// Runs hourly to keep the TwoFactorChallenges table lean and prevent unbounded growth.
/// </summary>
internal sealed class ExpiredTwoFactorChallengeCleanupJob(
    NetrockDbContext dbContext,
    TimeProvider timeProvider,
    ILogger<ExpiredTwoFactorChallengeCleanupJob> logger) : IRecurringJobDefinition
{
    /// <inheritdoc />
    public string JobId => "expired-two-factor-challenge-cleanup";

    /// <inheritdoc />
    public string CronExpression => Cron.Hourly();

    /// <inheritdoc />
    public async Task ExecuteAsync()
    {
        var cutoff = timeProvider.GetUtcNow().UtcDateTime;

        var deletedCount = await dbContext.TwoFactorChallenges
            .Where(c => c.ExpiresAt < cutoff || c.IsUsed)
            .ExecuteDeleteAsync();

        logger.LogInformation("Deleted {Count} expired two-factor challenges", deletedCount);
    }
}
