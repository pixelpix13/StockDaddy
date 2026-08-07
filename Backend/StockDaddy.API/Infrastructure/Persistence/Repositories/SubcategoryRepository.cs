using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class SubcategoryRepository : ISubcategoryRepository
{
    private readonly ApplicationDbContext _context;

    public SubcategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<SubcategoryDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Subcategories.Where(s => !s.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = RepositoryPaging.LikePattern(q.Search);
            var idMatch = RepositoryPaging.TryParseSearchId(q.Search, out var searchId);
            baseQuery = baseQuery.Where(s =>
                (idMatch && s.Id == searchId) ||
                (idMatch && s.CategoryId == searchId) ||
                EF.Functions.ILike(s.Name, pattern) ||
                _context.Categories.Any(c =>
                    c.Id == s.CategoryId &&
                    !c.IsDeleted &&
                    EF.Functions.ILike(c.Name, pattern)));
        }

        if (q.CategoryId.HasValue)
        {
            baseQuery = baseQuery.Where(s => s.CategoryId == q.CategoryId.Value);
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(s => new SubcategoryDto
        {
                Id = s.Id,
                StoreId = s.StoreId,
                TenantId = s.TenantId,
                CategoryId = s.CategoryId,
                Name = s.Name,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<Subcategory> ApplySort(IQueryable<Subcategory> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("name", true) => query.OrderByDescending(s => s.Name),
            ("name", false) => query.OrderBy(s => s.Name),
            ("createdat", true) => query.OrderByDescending(s => s.CreatedAt),
            ("createdat", false) => query.OrderBy(s => s.CreatedAt),
            (_, true) => query.OrderByDescending(s => s.Id),
            _ => query.OrderBy(s => s.Id),
        };

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
