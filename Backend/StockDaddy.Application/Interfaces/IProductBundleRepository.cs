using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IProductBundleRepository
{
    Task<List<ProductBundle>> GetAllAsync();
    Task<ProductBundle?> GetByIdAsync(Guid id);
    Task AddAsync(ProductBundle bundle);
    Task UpdateAsync(ProductBundle bundle);
    Task DeleteAsync(Guid id);
}
