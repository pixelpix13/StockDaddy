using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class SubcategoryRepository : ISubcategoryRepository
{
    private readonly ApplicationDbContext _context;

    public SubcategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubcategoryDto>> GetAllAsync()
    {
        return await _context.Subcategories
            .Where(s => !s.IsDeleted)
            .Select(s => new SubcategoryDto
            {
                Id = s.Id,
                StoreId = s.StoreId,
                TenantId = s.TenantId,
                CategoryId = s.CategoryId,
                Name = s.Name,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<SubcategoryDto?> GetByIdAsync(int id)
    {
        return await _context.Subcategories
            .Where(s => s.Id == id && !s.IsDeleted)
            .Select(s => new SubcategoryDto
            {
                Id = s.Id,
                StoreId = s.StoreId,
                TenantId = s.TenantId,
                CategoryId = s.CategoryId,
                Name = s.Name,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateSubcategoryRequest subcategory)
    {
        var entity = new Subcategory
        {
            StoreId = subcategory.StoreId,
            TenantId = subcategory.TenantId,
            CategoryId = subcategory.CategoryId,
            Name = subcategory.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Subcategories.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateSubcategoryRequest subcategory)
    {
        var entity = await _context.Subcategories.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (entity == null) return;
        entity.Name = subcategory.Name;
        entity.CategoryId = subcategory.CategoryId;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Subcategories.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Subcategories.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Subcategories.Update(entity);
        await _context.SaveChangesAsync();
    }
}
