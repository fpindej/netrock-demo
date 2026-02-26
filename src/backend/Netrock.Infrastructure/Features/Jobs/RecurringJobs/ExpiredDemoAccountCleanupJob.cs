using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Netrock.Application.Caching;
using Netrock.Application.Caching.Constants;
using Netrock.Application.Features.FileStorage;
using Netrock.Application.Features.Jobs;
using Netrock.Infrastructure.Features.Authentication.Models;

namespace Netrock.Infrastructure.Features.Jobs.RecurringJobs;

/// <summary>
/// Removes expired demo accounts from the database.
/// Runs hourly as a safety net for abandoned sessions that were never explicitly logged out.
/// </summary>
internal sealed class ExpiredDemoAccountCleanupJob(
    UserManager<ApplicationUser> userManager,
    TimeProvider timeProvider,
    IFileStorageService fileStorageService,
    ICacheService cacheService,
    IJobExecutionContext executionContext,
    ILogger<ExpiredDemoAccountCleanupJob> logger) : IRecurringJobDefinition
{
    /// <inheritdoc />
    public string JobId => "expired-demo-account-cleanup";

    /// <inheritdoc />
    public string CronExpression => Cron.Hourly();

    /// <inheritdoc />
    public async Task ExecuteAsync()
    {
        var utcNow = timeProvider.GetUtcNow();

        var expiredUsers = await userManager.Users
            .Where(u => u.DemoExpiresAtUtc != null && u.DemoExpiresAtUtc < utcNow)
            .ToListAsync();

        executionContext.LogInfo($"Found {expiredUsers.Count} expired demo accounts", "Discovery");

        if (expiredUsers.Count == 0)
        {
            logger.LogDebug("No expired demo accounts to clean up");
            return;
        }

        var deletedCount = 0;

        foreach (var user in expiredUsers)
        {
            if (user.HasAvatar)
            {
                var avatarDeleteResult = await fileStorageService.DeleteAsync($"avatars/{user.Id}.webp", CancellationToken.None);
                if (!avatarDeleteResult.IsSuccess)
                {
                    executionContext.LogWarning($"Failed to delete avatar for user {user.Id}: {avatarDeleteResult.Error}", "Avatar");
                    logger.LogWarning("Failed to delete avatar for expired demo user {UserId}: {Error}",
                        user.Id, avatarDeleteResult.Error);
                }
                else
                {
                    executionContext.LogInfo($"Deleted avatar for user {user.Id}", "Avatar");
                }
            }

            var deleteResult = await userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
            {
                var errors = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
                executionContext.LogError($"Failed to delete user {user.Id}: {errors}", category: "Deletion");
                logger.LogError("Failed to delete expired demo account {UserId}: {Errors}",
                    user.Id, errors);
                continue;
            }

            await cacheService.RemoveAsync(CacheKeys.User(user.Id), CancellationToken.None);
            await cacheService.RemoveAsync(CacheKeys.SecurityStamp(user.Id), CancellationToken.None);

            executionContext.LogInfo($"Deleted demo account {user.Id}", "Deletion");
            deletedCount++;
        }

        executionContext.LogInfo($"Cleaned up {deletedCount} of {expiredUsers.Count} expired demo accounts", "Summary");
        logger.LogInformation("Cleaned up {Count} expired demo accounts", deletedCount);
    }
}
