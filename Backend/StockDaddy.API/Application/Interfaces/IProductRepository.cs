using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IProductRepository
{
    Task<List<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task AddAsync(CreateProductRequest product);
    Task UpdateAsync(int id, UpdateProductRequest product);
    Task SoftDeleteAsync(int id);
}
