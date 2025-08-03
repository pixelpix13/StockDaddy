using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IBundleItemRepository
{
    Task<List<BundleItem>> GetAllAsync();
    Task<BundleItem?> GetByIdAsync(Guid id);
    Task AddAsync(BundleItem item);
    Task UpdateAsync(BundleItem item);
    Task DeleteAsync(Guid id);
}
