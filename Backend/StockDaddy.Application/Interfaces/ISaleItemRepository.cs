using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface ISaleItemRepository
{
    Task<List<SaleItem>> GetAllBySaleIdAsync(Guid saleId);
    Task<SaleItem?> GetByIdAsync(Guid id);
    Task AddAsync(SaleItem item);
    Task UpdateAsync(SaleItem item);
    Task DeleteAsync(Guid id);
}
