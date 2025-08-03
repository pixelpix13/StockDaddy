using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IProductAttributeRepository
{
    Task<List<ProductAttribute>> GetAllAsync();
    Task<ProductAttribute?> GetByIdAsync(Guid id);
    Task AddAsync(ProductAttribute attribute);
    Task UpdateAsync(ProductAttribute attribute);
    Task DeleteAsync(Guid id);
}
