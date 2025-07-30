using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class SubcategoryService
{
    private readonly ISubcategoryRepository _repo;
    private readonly ICategoryRepository _categoryRepo;

    public SubcategoryService(ISubcategoryRepository repo, ICategoryRepository categoryRepo)
    {
        _repo = repo;
        _categoryRepo = categoryRepo;
    }

    // ✅ Get all subcategories for a tenant
    public async Task<List<SubcategoryDto>> GetByTenantAsync(Guid tenantId)
    {
        var data = await _repo.GetByTenantAsync(tenantId);

        var result = new List<SubcategoryDto>();
        foreach (var sub in data)
        {
            var category = await _categoryRepo.GetByIdAsync(sub.CategoryId);
            result.Add(new SubcategoryDto
            {
                Id = sub.Id,
                Name = sub.Name,
                CategoryId = sub.CategoryId,
                TenantId = sub.TenantId,
                CategoryName = category?.Name ?? "Unknown"
            });
        }

        return result;
    }

    // ✅ Get one by ID
    public async Task<SubcategoryDto?> GetByIdAsync(Guid id)
    {
        var sub = await _repo.GetByIdAsync(id);
        if (sub == null) return null;

        var category = await _categoryRepo.GetByIdAsync(sub.CategoryId);
        return new SubcategoryDto
        {
            Id = sub.Id,
            Name = sub.Name,
            CategoryId = sub.CategoryId,
            TenantId = sub.TenantId,
            CategoryName = category?.Name ?? "Unknown"
        };
    }

    // ✅ Create
    public async Task AddAsync(CreateSubcategoryRequest request)
    {
        var entity = new Subcategory
        {
            Name = request.Name,
            CategoryId = request.CategoryId,
            TenantId = request.TenantId
        };
        await _repo.AddAsync(entity);
    }

    // ✅ Update
    public async Task<bool> UpdateAsync(Guid id, UpdateSubcategoryRequest request)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return false;

        entity.Name = request.Name;
        entity.CategoryId = request.CategoryId;

        await _repo.UpdateAsync(entity);
        return true;
    }

    // ✅ Delete
    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
