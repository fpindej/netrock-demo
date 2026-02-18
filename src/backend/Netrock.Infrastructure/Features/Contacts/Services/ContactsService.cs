using System.Text.Json;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Netrock.Application.Features.Audit;
using Netrock.Application.Features.Contacts;
using Netrock.Application.Features.Contacts.Dtos;
using Netrock.Domain.Entities;
using Netrock.Infrastructure.Persistence;
using Netrock.Infrastructure.Persistence.Extensions;
using Netrock.Shared;

namespace Netrock.Infrastructure.Features.Contacts.Services;

/// <summary>
/// EF Core implementation of <see cref="IContactsService"/>.
/// All queries are scoped to the requesting user for data isolation.
/// </summary>
internal sealed class ContactsService(NetrockDbContext dbContext, IAuditService auditService) : IContactsService
{
    /// <inheritdoc />
    public async Task<Result<(List<ContactOutput> Items, int TotalCount)>> GetContactsAsync(Guid userId, int pageNumber, int pageSize, string? search, CancellationToken ct)
    {
        var query = dbContext.Contacts
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.IsFavorite)
            .ThenByDescending(c => c.CreatedAt)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(c =>
                c.Name.ToLower().Contains(s) ||
                c.Name.Similarity(s) > 0.3 ||
                (c.Email != null && (c.Email.ToLower().Contains(s) || c.Email.Similarity(s) > 0.3)) ||
                (c.Company != null && (c.Company.ToLower().Contains(s) || c.Company.Similarity(s) > 0.3)) ||
                (c.Phone != null && c.Phone.ToLower().Contains(s)));
        }

        var totalCount = await query.CountAsync(ct);

        var contacts = await query
            .Paginate(pageNumber, pageSize)
            .Select(c => ToOutput(c))
            .ToListAsync(ct);

        return Result<(List<ContactOutput>, int)>.Success((contacts, totalCount));
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

        var changes = new Dictionary<string, object>();
        if (contact.Name != input.Name) changes["name"] = new { from = contact.Name, to = input.Name };
        if (contact.Email != input.Email) changes["email"] = new { from = contact.Email ?? "", to = input.Email ?? "" };
        if (contact.Company != input.Company) changes["company"] = new { from = contact.Company ?? "", to = input.Company ?? "" };
        if (contact.Phone != input.Phone) changes["phone"] = new { from = contact.Phone ?? "", to = input.Phone ?? "" };
        if (contact.Status != input.Status) changes["status"] = new { from = contact.Status.ToString(), to = input.Status.ToString() };
        if (contact.Source != input.Source) changes["source"] = new { from = contact.Source.ToString(), to = input.Source.ToString() };
        if (contact.Value != input.Value) changes["value"] = new { from = contact.Value?.ToString() ?? "", to = input.Value?.ToString() ?? "" };
        if (contact.Notes != input.Notes) changes["notes"] = new { from = contact.Notes ?? "", to = input.Notes ?? "" };
        if (contact.IsFavorite != input.IsFavorite) changes["isFavorite"] = new { from = contact.IsFavorite, to = input.IsFavorite };

        var metadata = changes.Count > 0 ? JsonSerializer.Serialize(new { changes }) : null;

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
            metadata: metadata,
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

    /// <inheritdoc />
    public async Task<Result<int>> BulkDeleteAsync(Guid userId, List<Guid> contactIds, CancellationToken ct)
    {
        var contacts = await dbContext.Contacts
            .Where(c => c.UserId == userId && contactIds.Contains(c.Id))
            .ToListAsync(ct);

        foreach (var contact in contacts)
        {
            contact.SoftDelete();
        }

        await dbContext.SaveChangesAsync(ct);

        foreach (var contact in contacts)
        {
            await auditService.LogAsync(
                AuditActions.ContactDelete,
                userId,
                targetEntityType: "Contact",
                targetEntityId: contact.Id,
                ct: ct);
        }

        return Result<int>.Success(contacts.Count);
    }

    /// <inheritdoc />
    public async Task<Result<int>> DeleteAllAsync(Guid userId, CancellationToken ct)
    {
        var contacts = await dbContext.Contacts
            .Where(c => c.UserId == userId)
            .ToListAsync(ct);

        foreach (var contact in contacts)
        {
            contact.SoftDelete();
        }

        await dbContext.SaveChangesAsync(ct);

        foreach (var contact in contacts)
        {
            await auditService.LogAsync(
                AuditActions.ContactDelete,
                userId,
                targetEntityType: "Contact",
                targetEntityId: contact.Id,
                ct: ct);
        }

        return Result<int>.Success(contacts.Count);
    }

    /// <inheritdoc />
    public async Task<Result<ContactOutput>> ToggleFavoriteAsync(Guid userId, Guid contactId, CancellationToken ct)
    {
        var contact = await dbContext.Contacts
            .Where(c => c.UserId == userId && c.Id == contactId)
            .FirstOrDefaultAsync(ct);

        if (contact is null)
        {
            return Result<ContactOutput>.Failure(ErrorMessages.Contacts.NotFound, ErrorType.NotFound);
        }

        var newFavorite = !contact.IsFavorite;
        var favoriteMetadata = JsonSerializer.Serialize(new { isFavorite = newFavorite });

        contact.Update(
            contact.Name,
            contact.Email,
            contact.Company,
            contact.Phone,
            contact.Status,
            contact.Source,
            contact.Value,
            contact.Notes,
            newFavorite);

        await dbContext.SaveChangesAsync(ct);

        await auditService.LogAsync(
            AuditActions.ContactFavorite,
            userId,
            targetEntityType: "Contact",
            targetEntityId: contact.Id,
            metadata: favoriteMetadata,
            ct: ct);

        return Result<ContactOutput>.Success(ToOutput(contact));
    }

    /// <inheritdoc />
    public async Task<Result<List<ContactOutput>>> SeedAsync(Guid userId, int count, CancellationToken ct)
    {
        var existing = await dbContext.Contacts.CountAsync(c => c.UserId == userId, ct);
        if (existing > 0)
        {
            return Result<List<ContactOutput>>.Failure("Cannot seed when contacts already exist.", ErrorType.Validation);
        }

        var contacts = GenerateSeedContacts(userId, count);

        dbContext.Contacts.AddRange(contacts);
        await dbContext.SaveChangesAsync(ct);

        foreach (var contact in contacts)
        {
            await auditService.LogAsync(
                AuditActions.ContactCreate,
                userId,
                targetEntityType: "Contact",
                targetEntityId: contact.Id,
                ct: ct);
        }

        return Result<List<ContactOutput>>.Success(contacts.Select(ToOutput).ToList());
    }

    private static List<Contact> GenerateSeedContacts(Guid userId, int count)
    {
        var statuses = Enum.GetValues<ContactStatus>();
        var sources = Enum.GetValues<ContactSource>();

        var faker = new Faker<Contact>()
            .CustomInstantiator(f =>
            {
                var status = f.PickRandom(statuses);
                var source = f.PickRandom(sources);
                var value = f.Random.Bool(0.8f) ? f.Finance.Amount(500, 50_000) : (decimal?)null;
                var notes = f.Random.Bool(0.6f) ? f.Lorem.Sentence(4, 8) : null;

                return new Contact(
                    userId,
                    f.Name.FullName(),
                    f.Random.Bool(0.9f) ? f.Internet.Email() : null,
                    f.Random.Bool(0.85f) ? f.Company.CompanyName() : null,
                    f.Random.Bool(0.7f) ? f.Phone.PhoneNumber("+# ###-###-####") : null,
                    status,
                    source,
                    value,
                    notes);
            });

        var contacts = faker.Generate(count);

        // Mark a couple as favorites
        foreach (var c in contacts.Take(2))
        {
            c.Update(c.Name, c.Email, c.Company, c.Phone, c.Status, c.Source, c.Value, c.Notes, true);
        }

        return contacts;
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
