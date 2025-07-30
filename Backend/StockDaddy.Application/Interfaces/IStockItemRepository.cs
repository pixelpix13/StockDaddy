using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IStockItemRepository
{
    Task<StockItem?> GetByProductIdAsync(Guid productId);
    Task<List<StockItem>> GetAllAsync();
    Task AddAsync(StockItem item);
    Task UpdateAsync(StockItem item);
}
