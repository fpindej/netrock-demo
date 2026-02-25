using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Demo.Dtos;

/// <summary>
/// Request to set the demo role for the current user.
/// </summary>
public class ElevateRequest
{
    /// <summary>
    /// The target role: <c>"Admin"</c> to elevate or <c>"User"</c> to de-elevate.
    /// </summary>
    [Required]
    public string Role { get; [UsedImplicitly] init; } = string.Empty;
}
