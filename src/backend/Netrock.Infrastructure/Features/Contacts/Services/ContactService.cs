using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Netrock.Application.Features.Contacts;
using Netrock.Application.Identity;
using Netrock.Domain.Entities;
using Netrock.Infrastructure.Persistence;
using Netrock.Infrastructure.Persistence.Extensions;
using Netrock.Shared;

namespace Netrock.Infrastructure.Features.Contacts.Services;

/// <summary>
/// EF Core–backed implementation of <see cref="IContactService"/> for managing CRM contacts.
/// All queries are filtered by the current authenticated user's ID to enforce data isolation.
/// </summary>
internal class ContactService(
    NetrockDbContext dbContext,
    IUserContext userContext,
    ILogger<ContactService> logger) : IContactService
{
    private const int MaxSampleContacts = 100;

    /// <inheritdoc />
    public async Task<ContactListOutput> GetContactsAsync(GetContactsInput input,
        CancellationToken cancellationToken = default)
    {
        var userId = userContext.AuthenticatedUserId;

        var query = dbContext.Contacts
            .AsNoTracking()
            .Where(c => c.OwnerId == userId);

        if (!string.IsNullOrWhiteSpace(input.Search))
        {
            var searchLower = input.Search.ToLowerInvariant();
            query = query.Where(c =>
                c.FirstName.ToLower().Contains(searchLower) ||
                c.LastName.ToLower().Contains(searchLower) ||
                c.Email.ToLower().Contains(searchLower) ||
                (c.Company != null && c.Company.ToLower().Contains(searchLower)));
        }

        if (input.Status.HasValue)
        {
            query = query.Where(c => c.Status == input.Status.Value);
        }

        if (input.Source.HasValue)
        {
            query = query.Where(c => c.Source == input.Source.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var contacts = await query
            .OrderByDescending(c => c.CreatedAt)
            .Paginate(input.PageNumber, input.PageSize)
            .ToListAsync(cancellationToken);

        var outputs = contacts.Select(MapToOutput).ToList();

        return new ContactListOutput(outputs, totalCount, input.PageNumber, input.PageSize);
    }

    /// <inheritdoc />
    public async Task<Result<ContactOutput>> GetContactByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = userContext.AuthenticatedUserId;

        var contact = await dbContext.Contacts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.OwnerId == userId, cancellationToken);

        if (contact is null)
        {
            return Result<ContactOutput>.Failure(ErrorMessages.Entity.NotFound, ErrorType.NotFound);
        }

        return Result<ContactOutput>.Success(MapToOutput(contact));
    }

    /// <inheritdoc />
    public async Task<Result<ContactOutput>> CreateContactAsync(CreateContactInput input,
        CancellationToken cancellationToken = default)
    {
        var userId = userContext.AuthenticatedUserId;

        var contact = new Contact
        {
            FirstName = input.FirstName,
            LastName = input.LastName,
            Email = input.Email,
            Company = input.Company,
            Status = input.Status,
            Source = input.Source,
            Value = input.Value,
            Notes = input.Notes,
            Phone = input.Phone,
            OwnerId = userId
        };

        dbContext.Contacts.Add(contact);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Contact '{ContactId}' created by user '{UserId}'", contact.Id, userId);

        return Result<ContactOutput>.Success(MapToOutput(contact));
    }

    /// <inheritdoc />
    public async Task<Result<ContactOutput>> UpdateContactAsync(Guid id, UpdateContactInput input,
        CancellationToken cancellationToken = default)
    {
        var userId = userContext.AuthenticatedUserId;

        var contact = await dbContext.Contacts
            .FirstOrDefaultAsync(c => c.Id == id && c.OwnerId == userId, cancellationToken);

        if (contact is null)
        {
            return Result<ContactOutput>.Failure(ErrorMessages.Entity.NotFound, ErrorType.NotFound);
        }

        contact.FirstName = input.FirstName;
        contact.LastName = input.LastName;
        contact.Email = input.Email;
        contact.Company = input.Company;
        contact.Status = input.Status;
        contact.Source = input.Source;
        contact.Value = input.Value;
        contact.Notes = input.Notes;
        contact.Phone = input.Phone;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Contact '{ContactId}' updated by user '{UserId}'", id, userId);

        return Result<ContactOutput>.Success(MapToOutput(contact));
    }

    /// <inheritdoc />
    public async Task<Result> DeleteContactAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = userContext.AuthenticatedUserId;

        var contact = await dbContext.Contacts
            .FirstOrDefaultAsync(c => c.Id == id && c.OwnerId == userId, cancellationToken);

        if (contact is null)
        {
            return Result.Failure(ErrorMessages.Entity.NotFound, ErrorType.NotFound);
        }

        contact.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Contact '{ContactId}' soft-deleted by user '{UserId}'", id, userId);

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result<int>> GenerateSampleContactsAsync(int count,
        CancellationToken cancellationToken = default)
    {
        var userId = userContext.AuthenticatedUserId;

        var existingCount = await dbContext.Contacts
            .CountAsync(c => c.OwnerId == userId, cancellationToken);

        if (existingCount > 0)
        {
            return Result<int>.Failure(ErrorMessages.Contacts.SampleContactsAlreadyGenerated);
        }

        var effectiveCount = Math.Min(count, MaxSampleContacts);

        if (effectiveCount <= 0)
        {
            return Result<int>.Failure("Count must be a positive number.");
        }

        var faker = new Faker<Contact>()
            .RuleFor(c => c.FirstName, f => f.Name.FirstName())
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
            .RuleFor(c => c.Company, f => f.Company.CompanyName())
            .RuleFor(c => c.Status, f => f.PickRandom<ContactStatus>())
            .RuleFor(c => c.Source, f => f.PickRandom<ContactSource>())
            .RuleFor(c => c.Value, f => f.Finance.Amount(100, 50000))
            .RuleFor(c => c.Notes, f => f.Lorem.Sentence())
            .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber("+## ### ### ###"))
            .RuleFor(c => c.OwnerId, _ => userId);

        var contacts = faker.Generate(effectiveCount);

        dbContext.Contacts.AddRange(contacts);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("{Count} sample contacts generated for user '{UserId}'", effectiveCount, userId);

        return Result<int>.Success(effectiveCount);
    }

    /// <inheritdoc />
    public async Task<ContactStatsOutput> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var userId = userContext.AuthenticatedUserId;

        var contacts = await dbContext.Contacts
            .AsNoTracking()
            .Where(c => c.OwnerId == userId)
            .ToListAsync(cancellationToken);

        var totalContacts = contacts.Count;
        var totalValue = contacts.Sum(c => c.Value);
        var leadCount = contacts.Count(c => c.Status == ContactStatus.Lead);
        var prospectCount = contacts.Count(c => c.Status == ContactStatus.Prospect);
        var customerCount = contacts.Count(c => c.Status == ContactStatus.Customer);
        var churningCount = contacts.Count(c => c.Status == ContactStatus.Churning);
        var averageValue = totalContacts > 0 ? totalValue / totalContacts : 0;

        var sourceBreakdown = contacts
            .GroupBy(c => c.Source)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        var recentContacts = contacts
            .OrderByDescending(c => c.CreatedAt)
            .Take(5)
            .Select(MapToOutput)
            .ToList();

        return new ContactStatsOutput(
            totalContacts,
            totalValue,
            leadCount,
            prospectCount,
            customerCount,
            churningCount,
            averageValue,
            sourceBreakdown,
            recentContacts
        );
    }

    /// <summary>
    /// Maps a <see cref="Contact"/> entity to a <see cref="ContactOutput"/> DTO.
    /// </summary>
    private static ContactOutput MapToOutput(Contact contact) => new(
        Id: contact.Id,
        FirstName: contact.FirstName,
        LastName: contact.LastName,
        Email: contact.Email,
        Company: contact.Company,
        Status: contact.Status,
        Source: contact.Source,
        Value: contact.Value,
        Notes: contact.Notes,
        Phone: contact.Phone,
        OwnerId: contact.OwnerId,
        CreatedAt: contact.CreatedAt,
        UpdatedAt: contact.UpdatedAt
    );
}
