using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IProductVariantRepository
{
    Task<List<ProductVariant>> GetAllByProductIdAsync(Guid productId);
    Task<ProductVariant?> GetByIdAsync(Guid id);
    Task AddAsync(ProductVariant variant);
    Task UpdateAsync(ProductVariant variant);
    Task DeleteAsync(Guid id);
}
