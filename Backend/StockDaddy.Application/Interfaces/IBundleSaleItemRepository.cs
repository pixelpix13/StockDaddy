using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IBundleSaleItemRepository
{
    Task<BundleSaleItem?> GetByIdAsync(Guid id);
    Task<List<BundleSaleItem>> GetBySaleIdAsync(Guid saleId);
    Task AddAsync(BundleSaleItem item);
    Task UpdateAsync(BundleSaleItem item);
    Task DeleteAsync(Guid id);
}
