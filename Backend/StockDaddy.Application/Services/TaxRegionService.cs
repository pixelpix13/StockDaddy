using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class TaxRegionService
{
    private readonly ITaxRegionRepository _repo;

    public TaxRegionService(ITaxRegionRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TaxRegionDto>> GetAllAsync()
    {
        var regions = await _repo.GetAllAsync();
        return regions.Select(r => new TaxRegionDto
        {
            Id = r.Id,
            TenantId = r.TenantId,
            StoreId = r.StoreId,
            RegionName = r.RegionName,
            TaxPercent = r.TaxPercent,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList();
    }

    public async Task<TaxRegionDto?> GetByIdAsync(Guid id)
    {
        var region = await _repo.GetByIdAsync(id);
        if (region == null) return null;

        return new TaxRegionDto
        {
            Id = region.Id,
            TenantId = region.TenantId,
            StoreId = region.StoreId,
            RegionName = region.RegionName,
            TaxPercent = region.TaxPercent,
            CreatedAt = region.CreatedAt,
            UpdatedAt = region.UpdatedAt
        };
    }

    public async Task AddAsync(CreateTaxRegionRequest request)
    {
        var region = new TaxRegion
        {
            TenantId = request.TenantId,
            StoreId = request.StoreId,
            RegionName = request.RegionName,
            TaxPercent = request.TaxPercent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(region);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTaxRegionRequest request)
    {
        var region = await _repo.GetByIdAsync(id);
        if (region == null) return false;

        region.RegionName = request.RegionName;
        region.TaxPercent = request.TaxPercent;
        region.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(region);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var region = await _repo.GetByIdAsync(id);
        if (region == null) return false;

        // Soft delete logic
        region.IsDeleted = true;
        region.DeletedAt = DateTime.UtcNow;
        region.UpdatedAt = DateTime.UtcNow;
        

        await _repo.DeleteAsync(id);
        return true;
    }
}
