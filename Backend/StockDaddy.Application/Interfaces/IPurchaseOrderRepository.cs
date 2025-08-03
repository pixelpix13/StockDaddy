using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IPurchaseOrderRepository
{
    Task<List<PurchaseOrder>> GetAllAsync();
    Task<PurchaseOrder?> GetByIdAsync(Guid id);
    Task AddAsync(PurchaseOrder order);
    Task UpdateAsync(PurchaseOrder order);
    Task DeleteAsync(Guid id);
}
