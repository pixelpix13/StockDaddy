using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface ICategoryRepository
{
    Task<PagedResult<CategoryDto>> GetPagedAsync(PagedQuery query);
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task AddAsync(CreateCategoryRequest category);
    Task UpdateAsync(int id, UpdateCategoryRequest category);
    Task DeleteAsync(int id);
}
