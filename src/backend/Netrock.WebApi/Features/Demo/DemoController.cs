using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Netrock.Application.Features.Demo;
using Netrock.Application.Identity;
using Netrock.WebApi.Features.Demo.Dtos;
using Netrock.WebApi.Shared;

namespace Netrock.WebApi.Features.Demo;

/// <summary>
/// Demo endpoints for role elevation. Allows any authenticated user to toggle
/// themselves to/from Admin for realistic end-to-end demo previews.
/// SuperAdmin is always rejected at the service layer.
/// </summary>
[Tags("Demo")]
public class DemoController(IDemoService demoService, IUserContext userContext) : ApiController
{
    /// <summary>
    /// Sets the demo role for the current user. Accepts <c>"Admin"</c> or <c>"User"</c>.
    /// SuperAdmin is always rejected.
    /// </summary>
    /// <param name="request">The target role</param>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Role changed successfully</response>
    /// <response code="400">If the role is invalid or the operation fails</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the user was not found</response>
    /// <response code="429">If the rate limit is exceeded</response>
    [HttpPost("elevate")]
    [EnableRateLimiting(RateLimitPolicies.AdminMutations)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult> Elevate(
        [FromBody] ElevateRequest request,
        CancellationToken cancellationToken)
    {
        var userId = userContext.AuthenticatedUserId;
        var result = await demoService.SetDemoRoleAsync(userId, request.Role, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemFactory.Create(result.Error, result.ErrorType);
        }

        return NoContent();
    }
}
