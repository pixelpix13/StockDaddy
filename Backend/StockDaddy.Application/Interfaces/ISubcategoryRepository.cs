namespace StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

public interface ISubcategoryRepository
{
    Task<List<SubcategoryDto>> GetAllAsync();
    Task<SubcategoryDto?> GetByIdAsync(int id);
    Task AddAsync(CreateSubcategoryRequest subcategory);
    Task UpdateAsync(int id, UpdateSubcategoryRequest subcategory);
    Task DeleteAsync(int id);
}
