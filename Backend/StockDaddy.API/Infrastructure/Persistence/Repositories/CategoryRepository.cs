using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Authorization;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Helpers;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IRequestContext _requestContext;

    public CategoryRepository(ApplicationDbContext context, IRequestContext requestContext)
    {
        _context = context;
        _requestContext = requestContext;
    }

    public async Task<PagedResult<CategoryDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Categories.Where(c => !c.IsDeleted);

        if (_requestContext.TenantId.HasValue)
        {
            baseQuery = baseQuery.Where(c => c.TenantId == _requestContext.TenantId.Value);
        }

        var storeFilter = QueryScope.ResolveStoreFilter(q, _requestContext);
        if (storeFilter.HasValue)
        {
            baseQuery = baseQuery.Where(c => c.StoreId == storeFilter.Value);
        }

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = RepositoryPaging.LikePattern(q.Search);
            var idMatch = RepositoryPaging.TryParseSearchId(q.Search, out var searchId);
            baseQuery = baseQuery.Where(c =>
                (idMatch && c.Id == searchId) ||
                EF.Functions.ILike(c.Name, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(c => new CategoryDto
        {
            Id = c.Id,
            StoreId = c.StoreId,
            TenantId = c.TenantId,
            Name = c.Name,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<Category> ApplySort(IQueryable<Category> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("name", true) => query.OrderByDescending(c => c.Name),
            ("name", false) => query.OrderBy(c => c.Name),
            ("createdat", true) => query.OrderByDescending(c => c.CreatedAt),
            ("createdat", false) => query.OrderBy(c => c.CreatedAt),
            (_, true) => query.OrderByDescending(c => c.Id),
            _ => query.OrderBy(c => c.Id),
        };

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
