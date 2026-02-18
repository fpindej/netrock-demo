using Microsoft.EntityFrameworkCore;
using Netrock.Application.Features.Audit;
using Netrock.Application.Features.Contacts;
using Netrock.Application.Features.Contacts.Dtos;
using Netrock.Domain.Entities;
using Netrock.Infrastructure.Persistence;
using Netrock.Shared;

namespace Netrock.Infrastructure.Features.Contacts.Services;

/// <summary>
/// EF Core implementation of <see cref="IContactsService"/>.
/// All queries are scoped to the requesting user for data isolation.
/// </summary>
internal sealed class ContactsService(NetrockDbContext dbContext, IAuditService auditService) : IContactsService
{
    /// <inheritdoc />
    public async Task<Result<List<ContactOutput>>> GetContactsAsync(Guid userId, CancellationToken ct)
    {
        var contacts = await dbContext.Contacts
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.IsFavorite)
            .ThenByDescending(c => c.CreatedAt)
            .Select(c => ToOutput(c))
            .ToListAsync(ct);

        return Result<List<ContactOutput>>.Success(contacts);
    }

    /// <inheritdoc />
    public async Task<Result<ContactOutput>> GetContactAsync(Guid userId, Guid contactId, CancellationToken ct)
    {
        var contact = await dbContext.Contacts
            .Where(c => c.UserId == userId && c.Id == contactId)
            .FirstOrDefaultAsync(ct);

        if (contact is null)
        {
            return Result<ContactOutput>.Failure(ErrorMessages.Contacts.NotFound, ErrorType.NotFound);
        }

        return Result<ContactOutput>.Success(ToOutput(contact));
    }

    /// <inheritdoc />
    public async Task<Result<ContactOutput>> CreateContactAsync(Guid userId, CreateContactInput input, CancellationToken ct)
    {
        var contact = new Contact(
            userId,
            input.Name,
            input.Email,
            input.Company,
            input.Phone,
            input.Status,
            input.Source,
            input.Value,
            input.Notes);

        dbContext.Contacts.Add(contact);
        await dbContext.SaveChangesAsync(ct);

        await auditService.LogAsync(
            AuditActions.ContactCreate,
            userId,
            targetEntityType: "Contact",
            targetEntityId: contact.Id,
            ct: ct);

        return Result<ContactOutput>.Success(ToOutput(contact));
    }

    /// <inheritdoc />
    public async Task<Result<ContactOutput>> UpdateContactAsync(Guid userId, Guid contactId, UpdateContactInput input, CancellationToken ct)
    {
        var contact = await dbContext.Contacts
            .Where(c => c.UserId == userId && c.Id == contactId)
            .FirstOrDefaultAsync(ct);

        if (contact is null)
        {
            return Result<ContactOutput>.Failure(ErrorMessages.Contacts.NotFound, ErrorType.NotFound);
        }

        contact.Update(
            input.Name,
            input.Email,
            input.Company,
            input.Phone,
            input.Status,
            input.Source,
            input.Value,
            input.Notes,
            input.IsFavorite);

        await dbContext.SaveChangesAsync(ct);

        await auditService.LogAsync(
            AuditActions.ContactUpdate,
            userId,
            targetEntityType: "Contact",
            targetEntityId: contact.Id,
            ct: ct);

        return Result<ContactOutput>.Success(ToOutput(contact));
    }

    /// <inheritdoc />
    public async Task<Result> DeleteContactAsync(Guid userId, Guid contactId, CancellationToken ct)
    {
        var contact = await dbContext.Contacts
            .Where(c => c.UserId == userId && c.Id == contactId)
            .FirstOrDefaultAsync(ct);

        if (contact is null)
        {
            return Result.Failure(ErrorMessages.Contacts.NotFound, ErrorType.NotFound);
        }

        contact.SoftDelete();
        await dbContext.SaveChangesAsync(ct);

        await auditService.LogAsync(
            AuditActions.ContactDelete,
            userId,
            targetEntityType: "Contact",
            targetEntityId: contact.Id,
            ct: ct);

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result<ContactsStatsOutput>> GetStatsAsync(Guid userId, CancellationToken ct)
    {
        var contacts = await dbContext.Contacts
            .Where(c => c.UserId == userId)
            .ToListAsync(ct);

        var totalCount = contacts.Count;
        var customerCount = contacts.Count(c => c.Status == ContactStatus.Customer);
        var totalPipelineValue = contacts
            .Where(c => c.Status != ContactStatus.Churned)
            .Sum(c => c.Value ?? 0m);

        var byStatus = Enum.GetValues<ContactStatus>()
            .ToDictionary(s => s, s => contacts.Count(c => c.Status == s));

        var bySource = Enum.GetValues<ContactSource>()
            .ToDictionary(s => s, s => contacts.Count(c => c.Source == s));

        var pipelineValue = Enum.GetValues<ContactStatus>()
            .ToDictionary(s => s, s => contacts.Where(c => c.Status == s).Sum(c => c.Value ?? 0m));

        var recentContacts = contacts
            .OrderByDescending(c => c.CreatedAt)
            .Take(5)
            .Select(c => ToOutput(c))
            .ToList();

        var stats = new ContactsStatsOutput(
            totalCount,
            customerCount,
            totalPipelineValue,
            byStatus,
            bySource,
            pipelineValue,
            recentContacts);

        return Result<ContactsStatsOutput>.Success(stats);
    }

    private static ContactOutput ToOutput(Contact contact) => new(
        contact.Id,
        contact.Name,
        contact.Email,
        contact.Company,
        contact.Phone,
        contact.Status,
        contact.Source,
        contact.Value,
        contact.Notes,
        contact.IsFavorite,
        contact.CreatedAt,
        contact.UpdatedAt);
}
