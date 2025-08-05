using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IPurchaseOrderRepository
{
    Task<List<PurchaseOrderDto>> GetAllAsync();
    Task<PurchaseOrderDto?> GetByIdAsync(int id);
    Task AddAsync(CreatePurchaseOrderRequest order);
    Task UpdateAsync(int id, UpdatePurchaseOrderRequest order);
    Task DeleteAsync(int id);
}
