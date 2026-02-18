using Netrock.Application.Features.Contacts.Dtos;
using Netrock.WebApi.Features.Contacts.Dtos;

namespace Netrock.WebApi.Features.Contacts;

/// <summary>
/// Maps between Contacts Application layer DTOs and WebApi request/response DTOs.
/// </summary>
internal static class ContactsMapper
{
    /// <summary>
    /// Maps a <see cref="ContactOutput"/> to a <see cref="ContactResponse"/>.
    /// </summary>
    public static ContactResponse ToResponse(this ContactOutput output) => new()
    {
        Id = output.Id,
        Name = output.Name,
        Email = output.Email,
        Company = output.Company,
        Phone = output.Phone,
        Status = output.Status.ToString(),
        Source = output.Source.ToString(),
        Value = output.Value,
        Notes = output.Notes,
        IsFavorite = output.IsFavorite,
        CreatedAt = output.CreatedAt,
        UpdatedAt = output.UpdatedAt
    };

    /// <summary>
    /// Maps a <see cref="ContactsStatsOutput"/> to a <see cref="ContactsStatsResponse"/>.
    /// </summary>
    public static ContactsStatsResponse ToResponse(this ContactsStatsOutput output) => new()
    {
        TotalCount = output.TotalCount,
        CustomerCount = output.CustomerCount,
        TotalPipelineValue = output.TotalPipelineValue,
        ByStatus = output.ByStatus.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value),
        BySource = output.BySource.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value),
        PipelineValue = output.PipelineValue.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value),
        RecentContacts = output.RecentContacts.Select(c => c.ToResponse()).ToList()
    };

    /// <summary>
    /// Maps a <see cref="CreateContactRequest"/> to a <see cref="CreateContactInput"/>.
    /// </summary>
    public static CreateContactInput ToInput(this CreateContactRequest request) =>
        new(request.Name, request.Email, request.Company, request.Phone,
            request.Status, request.Source, request.Value, request.Notes);

    /// <summary>
    /// Maps an <see cref="UpdateContactRequest"/> to an <see cref="UpdateContactInput"/>.
    /// </summary>
    public static UpdateContactInput ToInput(this UpdateContactRequest request) =>
        new(request.Name, request.Email, request.Company, request.Phone,
            request.Status, request.Source, request.Value, request.Notes, request.IsFavorite);
}
