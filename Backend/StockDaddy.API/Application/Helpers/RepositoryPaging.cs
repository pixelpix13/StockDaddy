using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Helpers;

/// <summary>
/// Shared pagination helpers for EF Core repositories.
/// </summary>
public static class RepositoryPaging
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public static PagedQuery Normalize(PagedQuery query)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize switch
        {
            <= 0 => DefaultPageSize,
            > MaxPageSize => MaxPageSize,
            _ => query.PageSize
        };

        var sortDir = string.Equals(query.SortDir, "desc", StringComparison.OrdinalIgnoreCase)
            ? "desc"
            : "asc";

        var search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim();

        return new PagedQuery
        {
            Page = page,
            PageSize = pageSize,
            SortBy = query.SortBy,
            SortDir = sortDir,
            Search = search,
            CategoryId = query.CategoryId,
            Status = string.IsNullOrWhiteSpace(query.Status) ? null : query.Status.Trim(),
            RoleId = query.RoleId,
            PaymentMethod = string.IsNullOrWhiteSpace(query.PaymentMethod) ? null : query.PaymentMethod.Trim(),
            StockFilter = string.IsNullOrWhiteSpace(query.StockFilter)
                ? null
                : query.StockFilter.Trim().ToLowerInvariant(),
            SupplierId = query.SupplierId,
            SubcategoryId = query.SubcategoryId,
            TaxPercent = query.TaxPercent,
            Entity = string.IsNullOrWhiteSpace(query.Entity) ? null : query.Entity.Trim(),
            UserId = query.UserId
        };
    }

    public static string LikePattern(string search) => $"%{search.Trim()}%";

    /// <summary>Parses "#12" or "12" for ID-based search.</summary>
    public static bool TryParseSearchId(string? search, out int id)
    {
        id = 0;
        if (string.IsNullOrWhiteSpace(search))
        {
            return false;
        }

        var trimmed = search.Trim().TrimStart('#');
        return int.TryParse(trimmed, out id) && id > 0;
    }

    public static async Task<PagedResult<T>> ExecuteAsync<T>(
        IQueryable<T> query,
        PagedQuery paging,
        CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(paging);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((normalized.Page - 1) * normalized.PageSize)
            .Take(normalized.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            Page = normalized.Page,
            PageSize = normalized.PageSize,
            TotalCount = totalCount
        };
    }

    public static bool IsDescending(PagedQuery query) =>
        string.Equals(query.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
}
