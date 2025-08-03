using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IProductImageRepository
{
    Task<List<ProductImage>> GetAllAsync();
    Task<ProductImage?> GetByIdAsync(Guid id);
    Task AddAsync(ProductImage image);
    Task UpdateAsync(ProductImage image);
    Task DeleteAsync(Guid id);
}
