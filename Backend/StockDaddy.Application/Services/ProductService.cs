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
            Name = p.Name,
            Description = p.Description,
            Quantity = p.Quantity,
            CostPrice = p.CostPrice,
            SellingPrice = p.SellingPrice
        }).ToList();
    }

    public async Task AddAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Quantity = request.Quantity,
            CostPrice = request.CostPrice,
            SellingPrice = request.SellingPrice
        };
        await _repo.AddAsync(product);
    }
    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Quantity = product.Quantity,
            CostPrice = product.CostPrice,
            SellingPrice = product.SellingPrice
        };
    }
    public async Task<bool> UpdateAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return false;

        product.Name = request.Name;
        product.Description = request.Description;
        product.Quantity = request.Quantity;
        product.CostPrice = request.CostPrice;
        product.SellingPrice = request.SellingPrice;

        await _repo.UpdateAsync(product);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }

}
