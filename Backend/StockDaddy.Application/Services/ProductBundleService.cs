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

    public async Task<List<ProductBundleDto>> GetAllAsync()
    {
        var bundles = await _repo.GetAllAsync();
        return bundles.Select(b => new ProductBundleDto
        {
            Id = b.Id,
            TenantId = b.TenantId,
            Name = b.Name,
            Description = b.Description,
            Price = b.Price
        }).ToList();
    }

    public async Task<ProductBundleDto?> GetByIdAsync(Guid id)
    {
        var bundle = await _repo.GetByIdAsync(id);
        if (bundle == null) return null;

        return new ProductBundleDto
        {
            Id = bundle.Id,
            TenantId = bundle.TenantId,
            Name = bundle.Name,
            Description = bundle.Description,
            Price = bundle.Price
        };
    }

    public async Task AddAsync(CreateProductBundleRequest request)
    {
        var bundle = new ProductBundle
        {
            TenantId = request.TenantId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(bundle);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductBundleRequest request)
    {
        var bundle = await _repo.GetByIdAsync(id);
        if (bundle == null) return false;

        bundle.Name = request.Name;
        bundle.Description = request.Description;
        bundle.Price = request.Price;
        bundle.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(bundle);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var bundle = await _repo.GetByIdAsync(id);
        if (bundle == null) return false;
        bundle.IsDeleted = true;
        bundle.DeletedAt = DateTime.UtcNow;
        bundle.UpdatedAt = DateTime.UtcNow;

        await _repo.DeleteAsync(id);
        return true;
    }
}
