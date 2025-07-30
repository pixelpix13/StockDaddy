using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface ISaleRepository
{
    Task<List<Sale>> GetAllAsync(Guid tenantId);
    Task<Sale?> GetByIdAsync(Guid id);
    Task AddAsync(Sale sale);
    Task UpdateAsync(Sale sale);
    Task DeleteAsync(Guid id);
}
