
using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class TaxRegionRepository : ITaxRegionRepository
{
    private readonly ApplicationDbContext _context;

    public TaxRegionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

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
