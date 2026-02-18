using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Netrock.Application.Caching;
using Netrock.Application.Caching.Constants;
using Netrock.Application.Identity;
using Netrock.Application.Identity.Constants;
using Netrock.Infrastructure.Features.Authentication.Models;
using Netrock.WebApi.Features.Demo.Dtos;
using Netrock.WebApi.Options;
using Netrock.WebApi.Shared;

namespace Netrock.WebApi.Features.Demo;

/// <summary>
/// Demo-only endpoints for role switching. Disabled when <see cref="DemoOptions.Enabled"/> is false.
/// </summary>
[Tags("Demo")]
public class DemoController(
    UserManager<ApplicationUser> userManager,
    IUserContext userContext,
    IOptions<DemoOptions> demoOptions,
    ICacheService cacheService) : ApiController
{
    /// <summary>
    /// Switches the current user's role. Removes all existing roles and assigns the requested one.
    /// Rotates the security stamp so the frontend's next request triggers a token refresh.
    /// </summary>
    /// <param name="request">The role to switch to.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <response code="200">Role switched successfully.</response>
    /// <response code="400">If the role is invalid.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If demo mode is disabled.</response>
    [HttpPost("switch-role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SwitchRole(
        [FromBody] SwitchRoleRequest request,
        CancellationToken cancellationToken)
    {
        if (!demoOptions.Value.Enabled)
        {
            return NotFound();
        }

        var userId = userContext.AuthenticatedUserId;
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return ProblemFactory.Create("User not found.", Netrock.Shared.ErrorType.NotFound);
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Count > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return ProblemFactory.Create("Failed to remove existing roles.");
            }
        }

        var addResult = await userManager.AddToRoleAsync(user, request.Role);
        if (!addResult.Succeeded)
        {
            return ProblemFactory.Create("Failed to assign role.");
        }

        await userManager.UpdateSecurityStampAsync(user);

        await cacheService.RemoveAsync(CacheKeys.SecurityStamp(user.Id), cancellationToken);
        await cacheService.RemoveAsync(CacheKeys.User(user.Id), cancellationToken);

        return Ok();
    }
}
