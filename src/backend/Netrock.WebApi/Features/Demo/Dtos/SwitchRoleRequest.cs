using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Demo.Dtos;

/// <summary>
/// Represents a request to switch the current user's role in demo mode.
/// </summary>
[UsedImplicitly]
public class SwitchRoleRequest
{
    /// <summary>
    /// The role to switch to (User, Admin, or SuperAdmin).
    /// </summary>
    public string Role { get; [UsedImplicitly] init; } = string.Empty;
}
