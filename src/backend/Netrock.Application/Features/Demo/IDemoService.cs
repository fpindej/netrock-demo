using Netrock.Application.Features.Demo.Dtos;
using Netrock.Shared;

namespace Netrock.Application.Features.Demo;

/// <summary>
/// Provides demo role elevation and short-lived demo account management.
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

    /// <summary>
    /// Creates a short-lived demo account with a generated email and password.
    /// The account expires after 24 hours and is cleaned up on logout or by the background job.
    /// </summary>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A result containing the demo account credentials for auto-login.</returns>
    Task<Result<DemoAccountOutput>> CreateDemoAccountAsync(CancellationToken ct = default);

    /// <summary>
    /// Cleans up a demo account by deleting the user and associated data.
    /// No-ops for non-demo users (where <c>DemoExpiresAtUtc</c> is <c>null</c>).
    /// </summary>
    /// <param name="userId">The ID of the user to clean up.</param>
    /// <param name="ct">A cancellation token.</param>
    Task CleanupDemoAccountAsync(Guid userId, CancellationToken ct = default);
}
