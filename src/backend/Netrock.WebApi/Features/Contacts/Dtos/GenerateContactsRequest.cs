using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Request to generate sample contacts with Bogus.
/// </summary>
public class GenerateContactsRequest
{
    /// <summary>
    /// The number of sample contacts to generate (1–100).
    /// </summary>
    [Range(1, 100)]
    public int Count { get; [UsedImplicitly] init; } = 25;
}
