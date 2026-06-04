using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IStockItemRepository
{
    Task<List<StockItemDto>> GetAllAsync();
    Task<StockItemDto?> GetByIdAsync(int id);
    Task AddAsync(CreateStockItemRequest item);
    Task UpdateAsync(int id, UpdateStockItemRequest item);
    Task DeleteAsync(int id);
}
