using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IPurchaseItemRepository
{
    Task<List<PurchaseItem>> GetAllAsync();
    Task<PurchaseItem?> GetByIdAsync(Guid id);
    Task AddAsync(PurchaseItem item);
    Task UpdateAsync(PurchaseItem item);
    Task DeleteAsync(Guid id);
}
