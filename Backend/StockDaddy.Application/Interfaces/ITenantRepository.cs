using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface ITenantRepository
{
    Task<List<Tenant>> GetAllAsync();
    Task<Tenant?> GetByIdAsync(Guid id);
    Task AddAsync(Tenant tenant);
    Task UpdateAsync(Tenant tenant);
    Task DeleteAsync(Guid id);
}
