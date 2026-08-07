using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IProductRepository
{
    Task<PagedResult<ProductDto>> GetPagedAsync(PagedQuery query);
    Task<List<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<int> AddAsync(CreateProductRequest product);
    Task UpdateAsync(int id, UpdateProductRequest product);
    Task SoftDeleteAsync(int id);
}
