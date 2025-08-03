using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class ProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var products = await _repo.GetAllAsync();
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            TenantId = p.TenantId,
            StoreId = p.StoreId,
            SubcategoryId = p.SubcategoryId,
            Name = p.Name,
            Description = p.Description,
            Unit = p.Unit,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            IsDeleted = p.IsDeleted,
            DeletedAt = p.DeletedAt
        }).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return null;

        return new ProductDto
        {
            Id = product.Id,
            TenantId = product.TenantId,
            StoreId = product.StoreId,
            SubcategoryId = product.SubcategoryId,
            Name = product.Name,
            Description = product.Description,
            Unit = product.Unit,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            IsDeleted = product.IsDeleted,
            DeletedAt = product.DeletedAt
        };
    }

    public async Task AddAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            TenantId = request.TenantId,
            StoreId = request.StoreId,
            SubcategoryId = request.SubcategoryId,
            Name = request.Name,
            Description = request.Description,
            Unit = request.Unit,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(product);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return false;

        product.StoreId = request.StoreId;
        product.SubcategoryId = request.SubcategoryId;
        product.Name = request.Name;
        product.Description = request.Description;
        product.Unit = request.Unit;
        product.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(product);
        return true;
    }

    public async Task<bool> SoftDeleteAsync(Guid id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return false;

        await _repo.SoftDeleteAsync(id);
        return true;
    }
}
