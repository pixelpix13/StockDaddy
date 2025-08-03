using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class ProductAttributeService
{
    private readonly IProductAttributeRepository _repo;

    public ProductAttributeService(IProductAttributeRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductAttributeDto>> GetAllAsync()
    {
        var attributes = await _repo.GetAllAsync();
        return attributes.Select(a => new ProductAttributeDto
        {
            Id = a.Id,
            ProductId = a.ProductId,
            AttributeName = a.AttributeName,
            AttributeValue = a.AttributeValue,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        }).ToList();
    }

    public async Task<ProductAttributeDto?> GetByIdAsync(Guid id)
    {
        var a = await _repo.GetByIdAsync(id);
        if (a == null) return null;

        return new ProductAttributeDto
        {
            Id = a.Id,
            ProductId = a.ProductId,
            AttributeName = a.AttributeName,
            AttributeValue = a.AttributeValue,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        };
    }

    public async Task AddAsync(CreateProductAttributeRequest request)
    {
        var attr = new ProductAttribute
        {
            ProductId = request.ProductId,
            AttributeName = request.AttributeName,
            AttributeValue = request.AttributeValue,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(attr);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductAttributeRequest request)
    {
        var attr = await _repo.GetByIdAsync(id);
        if (attr == null) return false;

        attr.AttributeName = request.AttributeName;
        attr.AttributeValue = request.AttributeValue;
        attr.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(attr);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var attr = await _repo.GetByIdAsync(id);
        if (attr == null) return false;

        // Soft delete logic
        attr.IsDeleted = true;
        attr.DeletedAt = DateTime.UtcNow;
        attr.UpdatedAt = DateTime.UtcNow;

        await _repo.DeleteAsync(id);
        return true;
    }
}
