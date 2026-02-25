using Netrock.Shared;

namespace Netrock.Application.Features.Demo;

/// <summary>
/// Provides demo role elevation for the demo role switcher.
/// Allows any authenticated user to toggle themselves to/from Admin for realistic end-to-end demo previews.
/// SuperAdmin is intentionally excluded to protect PII.
/// </summary>
public interface IDemoService
{
    /// <summary>
    /// Sets the demo role for a user. Only <c>"Admin"</c> and <c>"User"</c> are accepted.
    /// <c>"SuperAdmin"</c> is always rejected.
    /// </summary>
    /// <param name="userId">The ID of the authenticated user requesting the role change.</param>
    /// <param name="role">The target role: <c>"Admin"</c> to elevate, <c>"User"</c> to de-elevate.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A success result, or a failure if the role is invalid or the operation fails.</returns>
    Task<Result> SetDemoRoleAsync(Guid userId, string role, CancellationToken ct = default);
}
