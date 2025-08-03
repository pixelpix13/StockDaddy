using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IStockItemRepository
{
    Task<List<StockItem>> GetAllAsync();
    Task<StockItem?> GetByIdAsync(Guid id);
    Task AddAsync(StockItem item);
    Task UpdateAsync(StockItem item);
    Task DeleteAsync(Guid id);
}
