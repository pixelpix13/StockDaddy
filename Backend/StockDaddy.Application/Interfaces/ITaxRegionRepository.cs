using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface ITaxRegionRepository
{
    Task<List<TaxRegion>> GetAllAsync();
    Task<TaxRegion?> GetByIdAsync(Guid id);
    Task AddAsync(TaxRegion region);
    Task UpdateAsync(TaxRegion region);
    Task DeleteAsync(Guid id);
}
