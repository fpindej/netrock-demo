using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Netrock.Application.Caching;
using Netrock.Application.Caching.Constants;
using Netrock.Application.Features.Demo;
using Netrock.Application.Identity.Constants;
using Netrock.Infrastructure.Features.Authentication.Models;
using Netrock.Shared;

namespace Netrock.Infrastructure.Features.Demo.Services;

/// <summary>
/// Demo role elevation service that allows any authenticated user to toggle the Admin role
/// for realistic end-to-end demo previews. SuperAdmin is always rejected.
/// </summary>
internal class DemoService(
    UserManager<ApplicationUser> userManager,
    ICacheService cacheService,
    ILogger<DemoService> logger) : IDemoService
{
    /// <inheritdoc />
    public async Task<Result> SetDemoRoleAsync(Guid userId, string role, CancellationToken ct = default)
    {
        if (role == AppRoles.SuperAdmin)
        {
            logger.LogWarning("Demo role elevation rejected: SuperAdmin requested by user {UserId}", userId);
            return Result.Failure(ErrorMessages.Demo.InvalidRole);
        }

        if (!AppRoles.All.Contains(role) || (role != AppRoles.Admin && role != AppRoles.User))
        {
            logger.LogWarning("Demo role elevation rejected: unknown role '{Role}' requested by user {UserId}", role, userId);
            return Result.Failure(ErrorMessages.Demo.InvalidRole);
        }

        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result.Failure(ErrorMessages.Demo.UserNotFound, ErrorType.NotFound);
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        var hasAdmin = currentRoles.Contains(AppRoles.Admin);

        if (role == AppRoles.Admin && !hasAdmin)
        {
            var addResult = await userManager.AddToRoleAsync(user, AppRoles.Admin);
            if (!addResult.Succeeded)
            {
                logger.LogError("Failed to add Admin role for demo user {UserId}: {Errors}",
                    userId, string.Join(", ", addResult.Errors.Select(e => e.Description)));
                return Result.Failure(ErrorMessages.Demo.RoleChangeFailed);
            }

            logger.LogInformation("Demo role elevated to Admin for user {UserId}", userId);
        }
        else if (role == AppRoles.User && hasAdmin)
        {
            var removeResult = await userManager.RemoveFromRoleAsync(user, AppRoles.Admin);
            if (!removeResult.Succeeded)
            {
                logger.LogError("Failed to remove Admin role for demo user {UserId}: {Errors}",
                    userId, string.Join(", ", removeResult.Errors.Select(e => e.Description)));
                return Result.Failure(ErrorMessages.Demo.RoleChangeFailed);
            }

            logger.LogInformation("Demo role de-elevated to User for user {UserId}", userId);
        }
        else
        {
            logger.LogDebug("Demo role unchanged for user {UserId} (already {Role})", userId, role);
        }

        await userManager.UpdateSecurityStampAsync(user);
        await cacheService.RemoveAsync(CacheKeys.SecurityStamp(userId), ct);
        await cacheService.RemoveAsync(CacheKeys.User(userId), ct);

        return Result.Success();
    }
}
