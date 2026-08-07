using StockDaddy.Application.DTOs;
namespace StockDaddy.Application.Interfaces;

public interface IProductTagRepository
{
    Task<PagedResult<ProductTagDto>> GetPagedAsync(PagedQuery query);
    Task<List<ProductTagDto>> GetAllAsync();
    Task<ProductTagDto?> GetByIdAsync(int id);
    Task AddAsync(CreateProductTagRequest tag);
    Task UpdateAsync(int id, UpdateProductTagRequest tag);
    Task DeleteAsync(int id);
}
