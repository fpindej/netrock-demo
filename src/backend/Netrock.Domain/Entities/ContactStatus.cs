namespace Netrock.Domain.Entities;

/// <summary>
/// Defines the pipeline stages for a contact.
/// </summary>
public enum ContactStatus
{
    /// <summary>Initial lead — not yet qualified.</summary>
    Lead = 0,

    /// <summary>Qualified prospect in active discussions.</summary>
    Prospect = 1,

    /// <summary>Paying customer.</summary>
    Customer = 2,

    /// <summary>Former customer who has left.</summary>
    Churned = 3
}
