using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class CategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _repo.GetAllAsync();
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            StoreId = c.StoreId,
            TenantId = c.TenantId,
            Name = c.Name,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return null;

        return new CategoryDto
        {
            Id = category.Id,
            StoreId = category.StoreId,
            TenantId = category.TenantId,
            Name = category.Name,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public async Task AddAsync(CreateCategoryRequest request)
    {
        var category = new Category
        {
            StoreId = request.StoreId,
            TenantId = request.TenantId,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(category);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateCategoryRequest request)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return false;

        category.Name = request.Name;
        category.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(category);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
