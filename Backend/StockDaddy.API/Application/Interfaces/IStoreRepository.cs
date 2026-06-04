using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IStoreRepository
{
    Task<List<StoreDto>> GetAllAsync();
    Task<StoreDto?> GetByIdAsync(int id);
    Task AddAsync(CreateStoreRequest store);
    Task UpdateAsync(int id, UpdateStoreRequest store);
    Task DeleteAsync(int id);
}
