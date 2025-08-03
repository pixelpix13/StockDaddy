using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IBundleSaleItemRepository
{
    Task<List<BundleSaleItem>> GetAllAsync();
    Task<BundleSaleItem?> GetByIdAsync(Guid id);
    Task AddAsync(BundleSaleItem item);
    Task UpdateAsync(BundleSaleItem item);
    Task DeleteAsync(Guid id);
}
