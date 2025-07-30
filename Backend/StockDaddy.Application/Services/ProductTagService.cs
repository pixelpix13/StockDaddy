using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class ProductTagService
{
    private readonly IProductTagRepository _repo;

    public ProductTagService(IProductTagRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductTagDto>> GetAllByProductIdAsync(Guid productId)
    {
        var tags = await _repo.GetAllByProductIdAsync(productId);
        return tags.Select(t => new ProductTagDto
        {
            Id = t.Id,
            ProductId = t.ProductId,
            Tag = t.Tag
        }).ToList();
    }

    public async Task<ProductTagDto?> GetByIdAsync(Guid id)
    {
        var tag = await _repo.GetByIdAsync(id);
        if (tag == null) return null;

        return new ProductTagDto
        {
            Id = tag.Id,
            ProductId = tag.ProductId,
            Tag = tag.Tag
        };
    }

    public async Task AddAsync(CreateProductTagRequest request)
    {
        var tag = new ProductTag
        {
            ProductId = request.ProductId,
            Tag = request.Tag
        };

        await _repo.AddAsync(tag);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductTagRequest request)
    {
        var tag = await _repo.GetByIdAsync(id);
        if (tag == null) return false;

        tag.Tag = request.Tag;
        await _repo.UpdateAsync(tag);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var tag = await _repo.GetByIdAsync(id);
        if (tag == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
