namespace StockDaddy.Application.Interfaces;    
using StockDaddy.Domain.Entities;


public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id);
    Task<List<Category>> GetByTenantAsync(Guid tenantId);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Guid id);
}
