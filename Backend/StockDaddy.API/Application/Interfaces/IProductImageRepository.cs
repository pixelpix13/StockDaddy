using StockDaddy.Application.DTOs;


namespace StockDaddy.Application.Interfaces;

public interface IProductImageRepository
{
    Task<List<ProductImageDto>> GetAllAsync();
    Task<ProductImageDto?> GetByIdAsync(int id);
    Task AddAsync(CreateProductImageRequest image);
    Task UpdateAsync(int id, UpdateProductImageRequest image);
    Task DeleteAsync(int id);
}
