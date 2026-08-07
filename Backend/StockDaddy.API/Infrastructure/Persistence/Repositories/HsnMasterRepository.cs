using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class HsnMasterRepository : IHsnMasterRepository
{
    private readonly ApplicationDbContext _context;

    public HsnMasterRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<HsnMasterDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.HsnMaster.Where(h => !h.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = RepositoryPaging.LikePattern(q.Search);
            var idMatch = RepositoryPaging.TryParseSearchId(q.Search, out var searchId);
            baseQuery = baseQuery.Where(h =>
                (idMatch && h.Id == searchId) ||
                EF.Functions.ILike(h.HSNCode, pattern) ||
                EF.Functions.ILike(h.Description, pattern) ||
                EF.Functions.ILike(h.CGSTPercent.ToString(), pattern) ||
                EF.Functions.ILike(h.SGSTPercent.ToString(), pattern));
        }

        if (q.TaxPercent.HasValue)
        {
            baseQuery = baseQuery.Where(h => h.CGSTPercent == q.TaxPercent.Value);
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(h => new HsnMasterDto
        {
                Id = h.Id,
                HSNCode = h.HSNCode,
                Description = h.Description,
                CGSTPercent = h.CGSTPercent,
                SGSTPercent = h.SGSTPercent,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<HsnMaster> ApplySort(IQueryable<HsnMaster> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(h => h.CreatedAt),
            ("createdat", false) => query.OrderBy(h => h.CreatedAt),
            (_, true) => query.OrderByDescending(h => h.Id),
            _ => query.OrderBy(h => h.Id),
        };

    public async Task<List<HsnMasterDto>> GetAllAsync()
    {
        return await _context.HsnMaster
            .Where(h => !h.IsDeleted)
            .Select(h => new HsnMasterDto
            {
                Id = h.Id,
                HSNCode = h.HSNCode,
                Description = h.Description,
                CGSTPercent = h.CGSTPercent,
                SGSTPercent = h.SGSTPercent,
                CreatedAt = h.CreatedAt,
                UpdatedAt = h.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<HsnMasterDto?> GetByIdAsync(int id)
    {
        var h = await _context.HsnMaster.FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
        if (h == null) return null;
        return new HsnMasterDto
        {
            Id = h.Id,
            HSNCode = h.HSNCode,
            Description = h.Description,
            CGSTPercent = h.CGSTPercent,
            SGSTPercent = h.SGSTPercent,
            CreatedAt = h.CreatedAt,
            UpdatedAt = h.UpdatedAt
        };
    }

    public async Task AddAsync(CreateHsnMasterRequest hsn)
    {
        var entity = new HsnMaster
        {
            HSNCode = hsn.HSNCode,
            Description = hsn.Description,
            CGSTPercent = hsn.CGSTPercent,
            SGSTPercent = hsn.SGSTPercent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.HsnMaster.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateHsnMasterRequest hsn)
    {
        var entity = await _context.HsnMaster.FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
        if (entity == null) return;

        entity.Description = hsn.Description;
        entity.CGSTPercent = hsn.CGSTPercent;
        entity.SGSTPercent = hsn.SGSTPercent;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.HsnMaster.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.HsnMaster.FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.HsnMaster.Update(entity);
        await _context.SaveChangesAsync();
    }
}
