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

    public async Task<ProductAttributeDto?> GetByIdAsync(Guid id)
    {
        var attr = await _repo.GetByIdAsync(id);
        return attr == null ? null : new ProductAttributeDto
        {
            Id = attr.Id,
            ProductId = attr.ProductId,
            AttributeName = attr.AttributeName,
            AttributeValue = attr.AttributeValue
        };
    }

    public async Task<List<ProductAttributeDto>> GetByProductIdAsync(Guid productId)
    {
        var list = await _repo.GetByProductIdAsync(productId);
        return list.Select(attr => new ProductAttributeDto
        {
            Id = attr.Id,
            ProductId = attr.ProductId,
            AttributeName = attr.AttributeName,
            AttributeValue = attr.AttributeValue
        }).ToList();
    }

    public async Task<Guid> CreateAsync(CreateProductAttributeRequest req)
    {
        var attr = new ProductAttribute
        {
            ProductId = req.ProductId,
            AttributeName = req.AttributeName,
            AttributeValue = req.AttributeValue
        };

        await _repo.AddAsync(attr);
        return attr.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductAttributeRequest req)
    {
        var attr = await _repo.GetByIdAsync(id);
        if (attr == null) return false;

        if (req.AttributeName is not null)
            attr.AttributeName = req.AttributeName;

        if (req.AttributeValue is not null)
            attr.AttributeValue = req.AttributeValue;

        await _repo.UpdateAsync(attr);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var attr = await _repo.GetByIdAsync(id);
        if (attr == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
