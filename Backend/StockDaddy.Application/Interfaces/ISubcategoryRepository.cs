namespace StockDaddy.Application.Interfaces;    
using StockDaddy.Domain.Entities;

public interface ISubcategoryRepository
{
    Task<Subcategory?> GetByIdAsync(Guid id);
    Task<List<Subcategory>> GetByTenantAsync(Guid tenantId);
    Task AddAsync(Subcategory subcategory);
    Task UpdateAsync(Subcategory subcategory);
    Task DeleteAsync(Guid id);
}
