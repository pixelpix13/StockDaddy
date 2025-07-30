using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IBundleItemRepository
{
    Task<BundleItem?> GetByIdAsync(Guid id);
    Task<List<BundleItem>> GetByBundleIdAsync(Guid bundleId);
    Task AddAsync(BundleItem item);
    Task UpdateAsync(BundleItem item);
    Task DeleteAsync(Guid id);
}
