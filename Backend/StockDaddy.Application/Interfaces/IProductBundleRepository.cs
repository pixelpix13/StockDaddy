using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IProductBundleRepository
{
    Task<ProductBundle?> GetByIdAsync(Guid id);
    Task<List<ProductBundle>> GetByTenantIdAsync(Guid tenantId);
    Task AddAsync(ProductBundle bundle);
    Task UpdateAsync(ProductBundle bundle);
    Task DeleteAsync(Guid id);
}
