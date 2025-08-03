using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class SubcategoryService
{
    private readonly ISubcategoryRepository _repo;

    public SubcategoryService(ISubcategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<SubcategoryDto>> GetAllAsync()
    {
        var subcategories = await _repo.GetAllAsync();
        return subcategories.Select(s => new SubcategoryDto
        {
            Id = s.Id,
            StoreId = s.StoreId,
            TenantId = s.TenantId,
            CategoryId = s.CategoryId,
            Name = s.Name,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        }).ToList();
    }

    public async Task<SubcategoryDto?> GetByIdAsync(Guid id)
    {
        var sub = await _repo.GetByIdAsync(id);
        if (sub == null) return null;

        return new SubcategoryDto
        {
            Id = sub.Id,
            StoreId = sub.StoreId,
            TenantId = sub.TenantId,
            CategoryId = sub.CategoryId,
            Name = sub.Name,
            CreatedAt = sub.CreatedAt,
            UpdatedAt = sub.UpdatedAt
        };
    }

    public async Task AddAsync(CreateSubcategoryRequest request)
    {
        var sub = new Subcategory
        {
            StoreId = request.StoreId,
            TenantId = request.TenantId,
            CategoryId = request.CategoryId,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(sub);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateSubcategoryRequest request)
    {
        var sub = await _repo.GetByIdAsync(id);
        if (sub == null) return false;

        sub.Name = request.Name;
        sub.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(sub);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var sub = await _repo.GetByIdAsync(id);
        if (sub == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
