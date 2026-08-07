
using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class TaxRegionRepository : ITaxRegionRepository
{
    private readonly ApplicationDbContext _context;

    public TaxRegionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<TaxRegionDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.TaxRegions.Where(r => !r.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = RepositoryPaging.LikePattern(q.Search);
            var idMatch = RepositoryPaging.TryParseSearchId(q.Search, out var searchId);
            baseQuery = baseQuery.Where(r =>
                (idMatch && r.Id == searchId) ||
                EF.Functions.ILike(r.RegionName, pattern) ||
                EF.Functions.ILike(r.TaxPercent.ToString(), pattern));
        }

        if (q.TaxPercent.HasValue)
        {
            baseQuery = baseQuery.Where(r => r.TaxPercent == q.TaxPercent.Value);
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(r => new TaxRegionDto
        {
                Id = r.Id,
                TenantId = r.TenantId,
                StoreId = r.StoreId,
                RegionName = r.RegionName,
                TaxPercent = r.TaxPercent,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<TaxRegion> ApplySort(IQueryable<TaxRegion> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("name", true) => query.OrderByDescending(r => r.RegionName),
            ("name", false) => query.OrderBy(r => r.RegionName),
            ("createdat", true) => query.OrderByDescending(r => r.CreatedAt),
            ("createdat", false) => query.OrderBy(r => r.CreatedAt),
            (_, true) => query.OrderByDescending(r => r.Id),
            _ => query.OrderBy(r => r.Id),
        };

    public async Task<List<TaxRegionDto>> GetAllAsync()
    {
        return await _context.TaxRegions
            .Where(r => !r.IsDeleted)
            .Select(r => new TaxRegionDto
            {
                Id = r.Id,
                TenantId = r.TenantId,
                StoreId = r.StoreId,
                RegionName = r.RegionName,
                TaxPercent = r.TaxPercent,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<TaxRegionDto?> GetByIdAsync(int id)
    {
        return await _context.TaxRegions
            .Where(r => r.Id == id && !r.IsDeleted)
            .Select(r => new TaxRegionDto
            {
                Id = r.Id,
                TenantId = r.TenantId,
                StoreId = r.StoreId,
                RegionName = r.RegionName,
                TaxPercent = r.TaxPercent,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateTaxRegionRequest region)
    {
        var entity = new TaxRegion
        {
            TenantId = region.TenantId,
            StoreId = region.StoreId,
            RegionName = region.RegionName,
            TaxPercent = region.TaxPercent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.TaxRegions.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateTaxRegionRequest region)
    {
        var entity = await _context.TaxRegions.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        if (entity == null) return;
        entity.RegionName = region.RegionName;
        entity.TaxPercent = region.TaxPercent;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.TaxRegions.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.TaxRegions.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.TaxRegions.Update(entity);
        await _context.SaveChangesAsync();
    }
}
