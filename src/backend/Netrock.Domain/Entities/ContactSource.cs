namespace Netrock.Domain.Entities;

/// <summary>
/// Defines the acquisition channel through which a contact was obtained.
/// </summary>
public enum ContactSource
{
    /// <summary>Inbound from company website.</summary>
    Website = 0,

    /// <summary>Referred by an existing contact.</summary>
    Referral = 1,

    /// <summary>Social media channel.</summary>
    Social = 2,

    /// <summary>Email outreach or inbound email.</summary>
    Email = 3,

    /// <summary>Phone call or cold call.</summary>
    Phone = 4,

    /// <summary>Other or unspecified source.</summary>
    Other = 5
}
