using Netrock.Domain.Entities;

namespace Netrock.Application.Features.Contacts;

/// <summary>
/// Input for querying contacts with pagination, search, and filtering.
/// </summary>
/// <param name="PageNumber">The page number (1-based).</param>
/// <param name="PageSize">The number of items per page.</param>
/// <param name="Search">Optional search term to filter by name, email, or company.</param>
/// <param name="Status">Optional status filter.</param>
/// <param name="Source">Optional source filter.</param>
public record GetContactsInput(
    int PageNumber,
    int PageSize,
    string? Search,
    ContactStatus? Status,
    ContactSource? Source
);
