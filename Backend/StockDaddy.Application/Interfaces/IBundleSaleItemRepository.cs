using StockDaddy.Application.DTOs;


namespace StockDaddy.Application.Interfaces;

public interface IBundleSaleItemRepository
{
    Task<List<BundleSaleItemDto>> GetAllAsync();
    Task<BundleSaleItemDto?> GetByIdAsync(int id);
    Task AddAsync(CreateBundleSaleItemRequest item);
    Task UpdateAsync(int id, UpdateBundleSaleItemRequest item);
    Task DeleteAsync(int id);
}
