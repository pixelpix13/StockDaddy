using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IStoreRepository
{
    Task<List<Store>> GetAllAsync();
    Task<Store?> GetByIdAsync(Guid id);
    Task AddAsync(Store store);
    Task UpdateAsync(Store store);
    Task DeleteAsync(Guid id);
}
