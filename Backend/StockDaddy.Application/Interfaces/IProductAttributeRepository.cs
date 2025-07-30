using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IProductAttributeRepository
{
    Task<ProductAttribute?> GetByIdAsync(Guid id);
    Task<List<ProductAttribute>> GetByProductIdAsync(Guid productId);
    Task AddAsync(ProductAttribute attr);
    Task UpdateAsync(ProductAttribute attr);
    Task DeleteAsync(Guid id);
}
