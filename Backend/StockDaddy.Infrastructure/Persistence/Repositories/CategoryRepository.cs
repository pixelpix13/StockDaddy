using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.DTOs;

namespace StockDaddy.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<List<CategoryDto>> GetAllAsync()
    {
        return await _context.Categories
            .Where(c => !c.IsDeleted)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                StoreId = c.StoreId,
                TenantId = c.TenantId,
                Name = c.Name,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();
    }


    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var c = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (c == null) return null;
        return new CategoryDto
        {
            Id = c.Id,
            StoreId = c.StoreId,
            TenantId = c.TenantId,
            Name = c.Name,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };
    }


    public async Task AddAsync(CreateCategoryRequest category)
    {
        var entity = new Category
        {
            StoreId = category.StoreId,
            TenantId = category.TenantId,
            Name = category.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Categories.AddAsync(entity);
        await _context.SaveChangesAsync();
    }


    public async Task UpdateAsync(int id, UpdateCategoryRequest category)
    {
        var entity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (entity == null) return;

        entity.Name = category.Name;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Categories.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Categories.Update(entity);
        await _context.SaveChangesAsync();
    }
}
