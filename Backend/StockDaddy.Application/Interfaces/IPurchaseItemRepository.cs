using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IPurchaseItemRepository
{
    Task<List<PurchaseItemDto>> GetAllAsync();
    Task<PurchaseItemDto?> GetByIdAsync(int id);
    Task AddAsync(CreatePurchaseItemRequest item);
    Task UpdateAsync(int id, UpdatePurchaseItemRequest item);
    Task DeleteAsync(int id);
}
