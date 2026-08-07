using StockDaddy.Application.DTOs;


namespace StockDaddy.Application.Interfaces;

public interface IBundleItemRepository
{
    Task<PagedResult<BundleItemDto>> GetPagedAsync(PagedQuery query);
    Task<List<BundleItemDto>> GetAllAsync();
    Task<BundleItemDto?> GetByIdAsync(int id);
    Task AddAsync(CreateBundleItemRequest item);
    Task UpdateAsync(int id, UpdateBundleItemRequest item);
    Task DeleteAsync(int id);
}
