using StockDaddy.Application.DTOs;


namespace StockDaddy.Application.Interfaces;

public interface IProductBundleRepository
{
    Task<PagedResult<ProductBundleDto>> GetPagedAsync(PagedQuery query);
    Task<List<ProductBundleDto>> GetAllAsync();
    Task<ProductBundleDto?> GetByIdAsync(int id);
    Task AddAsync(CreateProductBundleRequest bundle);
    Task UpdateAsync(int id, UpdateProductBundleRequest bundle);
    Task DeleteAsync(int id);
}
