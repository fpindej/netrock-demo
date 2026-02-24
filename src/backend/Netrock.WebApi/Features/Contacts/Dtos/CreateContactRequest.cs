using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Netrock.Domain.Entities;

namespace Netrock.WebApi.Features.Contacts.Dtos;

/// <summary>
/// Request to create a new CRM contact.
/// </summary>
public class CreateContactRequest
{
    /// <summary>
    /// The contact's first name.
    /// </summary>
    [Required]
    public string FirstName { get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// The contact's last name.
    /// </summary>
    [Required]
    public string LastName { get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// The contact's email address.
    /// </summary>
    [Required]
    public string Email { get; [UsedImplicitly] init; } = string.Empty;

    /// <summary>
    /// The company the contact is associated with.
    /// </summary>
    public string? Company { get; [UsedImplicitly] init; }

    /// <summary>
    /// The pipeline status of the contact.
    /// </summary>
    [Required]
    public ContactStatus Status { get; [UsedImplicitly] init; }

    /// <summary>
    /// The acquisition source of the contact.
    /// </summary>
    [Required]
    public ContactSource Source { get; [UsedImplicitly] init; }

    /// <summary>
    /// The estimated monetary value of the contact.
    /// </summary>
    public decimal Value { get; [UsedImplicitly] init; }

    /// <summary>
    /// Free-text notes about the contact.
    /// </summary>
    public string? Notes { get; [UsedImplicitly] init; }

    /// <summary>
    /// The contact's phone number.
    /// </summary>
    public string? Phone { get; [UsedImplicitly] init; }
}
