using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class ProductBundleService
{
    private readonly IProductBundleRepository _repo;

    public ProductBundleService(IProductBundleRepository repo)
    {
        _repo = repo;
    }

    public async Task<ProductBundleDto?> GetByIdAsync(Guid id)
    {
        var b = await _repo.GetByIdAsync(id);
        return b == null ? null : new ProductBundleDto
        {
            Id = b.Id,
            TenantId = b.TenantId,
            Name = b.Name,
            Description = b.Description,
            Price = b.Price
        };
    }

    public async Task<List<ProductBundleDto>> GetByTenantIdAsync(Guid tenantId)
    {
        var list = await _repo.GetByTenantIdAsync(tenantId);
        return list.Select(b => new ProductBundleDto
        {
            Id = b.Id,
            TenantId = b.TenantId,
            Name = b.Name,
            Description = b.Description,
            Price = b.Price
        }).ToList();
    }

    public async Task<Guid> CreateAsync(CreateProductBundleRequest req)
    {
        var b = new ProductBundle
        {
            TenantId = req.TenantId,
            Name = req.Name,
            Description = req.Description,
            Price = req.Price
        };

        await _repo.AddAsync(b);
        return b.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductBundleRequest req)
    {
        var b = await _repo.GetByIdAsync(id);
        if (b == null) return false;

        if (!string.IsNullOrWhiteSpace(req.Name)) b.Name = req.Name;
        if (!string.IsNullOrWhiteSpace(req.Description)) b.Description = req.Description;
        if (req.Price.HasValue) b.Price = req.Price.Value;

        await _repo.UpdateAsync(b);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var b = await _repo.GetByIdAsync(id);
        if (b == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
