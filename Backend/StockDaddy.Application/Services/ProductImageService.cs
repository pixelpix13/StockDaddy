using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class ProductImageService
{
    private readonly IProductImageRepository _repo;

    public ProductImageService(IProductImageRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductImageDto>> GetAllByProductIdAsync(Guid productId)
    {
        var images = await _repo.GetAllByProductIdAsync(productId);
        return images.Select(i => new ProductImageDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ImageUrl = i.ImageUrl,
            IsPrimary = i.IsPrimary
        }).ToList();
    }

    public async Task<ProductImageDto?> GetByIdAsync(Guid id)
    {
        var image = await _repo.GetByIdAsync(id);
        if (image == null) return null;

        return new ProductImageDto
        {
            Id = image.Id,
            ProductId = image.ProductId,
            ImageUrl = image.ImageUrl,
            IsPrimary = image.IsPrimary
        };
    }

    public async Task AddAsync(CreateProductImageRequest request)
    {
        var image = new ProductImage
        {
            ProductId = request.ProductId,
            ImageUrl = request.ImageUrl,
            IsPrimary = request.IsPrimary
        };

        await _repo.AddAsync(image);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductImageRequest request)
    {
        var image = await _repo.GetByIdAsync(id);
        if (image == null) return false;

        image.ImageUrl = request.ImageUrl;
        image.IsPrimary = request.IsPrimary;

        await _repo.UpdateAsync(image);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var image = await _repo.GetByIdAsync(id);
        if (image == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
