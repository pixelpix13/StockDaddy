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

    // ✅ Get all categories by tenant
    public async Task<List<CategoryDto>> GetByTenantAsync(Guid tenantId)
    {
        var categories = await _repo.GetByTenantAsync(tenantId);

        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();
    }

    // ✅ Get a category by ID
    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    // ✅ Create a new category for a tenant
    public async Task AddAsync(CreateCategoryRequest request)
    {
        var category = new Category
        {
            TenantId = request.TenantId,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(category);
    }

    // ✅ Update existing category
    public async Task<bool> UpdateAsync(Guid id, UpdateCategoryRequest request)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return false;

        category.Name = request.Name;
        await _repo.UpdateAsync(category);
        return true;
    }

    // ✅ Delete
    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
