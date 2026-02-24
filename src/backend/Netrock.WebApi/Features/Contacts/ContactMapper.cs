using Netrock.Application.Features.Contacts;
using Netrock.WebApi.Features.Contacts.Dtos;

namespace Netrock.WebApi.Features.Contacts;

/// <summary>
/// Maps between contact Application layer DTOs and WebApi request/response DTOs.
/// </summary>
internal static class ContactMapper
{
    /// <summary>
    /// Maps a <see cref="ContactOutput"/> to a <see cref="ContactResponse"/>.
    /// </summary>
    public static ContactResponse ToResponse(this ContactOutput output) => new()
    {
        Id = output.Id,
        FirstName = output.FirstName,
        LastName = output.LastName,
        Email = output.Email,
        Company = output.Company,
        Status = output.Status.ToString(),
        Source = output.Source.ToString(),
        Value = output.Value,
        Notes = output.Notes,
        Phone = output.Phone,
        OwnerId = output.OwnerId,
        CreatedAt = output.CreatedAt,
        UpdatedAt = output.UpdatedAt
    };

    /// <summary>
    /// Maps a <see cref="ContactListOutput"/> to a <see cref="ContactListResponse"/>.
    /// </summary>
    public static ContactListResponse ToResponse(this ContactListOutput output) => new()
    {
        Items = output.Contacts.Select(c => c.ToResponse()).ToList(),
        TotalCount = output.TotalCount,
        PageNumber = output.PageNumber,
        PageSize = output.PageSize
    };

    /// <summary>
    /// Maps a <see cref="ContactStatsOutput"/> to a <see cref="ContactStatsResponse"/>.
    /// </summary>
    public static ContactStatsResponse ToResponse(this ContactStatsOutput output) => new()
    {
        TotalContacts = output.TotalContacts,
        TotalValue = output.TotalValue,
        LeadCount = output.LeadCount,
        ProspectCount = output.ProspectCount,
        CustomerCount = output.CustomerCount,
        ChurningCount = output.ChurningCount,
        AverageValue = output.AverageValue,
        SourceBreakdown = output.SourceBreakdown,
        RecentContacts = output.RecentContacts.Select(c => c.ToResponse()).ToList()
    };

    /// <summary>
    /// Maps a <see cref="CreateContactRequest"/> to a <see cref="CreateContactInput"/>.
    /// </summary>
    public static CreateContactInput ToInput(this CreateContactRequest request) => new(
        request.FirstName,
        request.LastName,
        request.Email,
        request.Company,
        request.Status,
        request.Source,
        request.Value,
        request.Notes,
        request.Phone
    );

    /// <summary>
    /// Maps an <see cref="UpdateContactRequest"/> to an <see cref="UpdateContactInput"/>.
    /// </summary>
    public static UpdateContactInput ToInput(this UpdateContactRequest request) => new(
        request.FirstName,
        request.LastName,
        request.Email,
        request.Company,
        request.Status,
        request.Source,
        request.Value,
        request.Notes,
        request.Phone
    );

    /// <summary>
    /// Maps a <see cref="GetContactsRequest"/> to a <see cref="GetContactsInput"/>.
    /// </summary>
    public static GetContactsInput ToInput(this GetContactsRequest request) => new(
        request.PageNumber,
        request.PageSize,
        request.Search,
        request.Status,
        request.Source
    );
}
