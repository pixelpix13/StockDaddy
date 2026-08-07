using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class StoreRepository : IStoreRepository
{
    private readonly ApplicationDbContext _context;

    public StoreRepository(ApplicationDbContext context)
    {
        _context = context; 
    }

    public async Task<PagedResult<StoreDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Stores.Where(s => !s.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(s => EF.Functions.ILike(s.Name, pattern) || EF.Functions.ILike(s.Location, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(s => new StoreDto
        {
                Id = s.Id,
                TenantId = s.TenantId,
                Name = s.Name,
                Location = s.Location,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<Store> ApplySort(IQueryable<Store> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("name", true) => query.OrderByDescending(s => s.Name),
            ("name", false) => query.OrderBy(s => s.Name),
            ("createdat", true) => query.OrderByDescending(s => s.CreatedAt),
            ("createdat", false) => query.OrderBy(s => s.CreatedAt),
            (_, true) => query.OrderByDescending(s => s.Id),
            _ => query.OrderBy(s => s.Id),
        };

    public async Task<List<StoreDto>> GetAllAsync()
    {
        return await _context.Stores
            .Where(s => !s.IsDeleted)
            .Select(s => new StoreDto
            {
                Id = s.Id,
                TenantId = s.TenantId,
                Name = s.Name,
                Location = s.Location,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<StoreDto?> GetByIdAsync(int id)
    {
        return await _context.Stores
            .Where(s => s.Id == id && !s.IsDeleted)
            .Select(s => new StoreDto
            {
                Id = s.Id,
                TenantId = s.TenantId,
                Name = s.Name,
                Location = s.Location,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateStoreRequest store)
    {
        var entity = new Store
        {
            TenantId = store.TenantId,
            Name = store.Name,
            Location = store.Location,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Stores.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateStoreRequest store)
    {
        var entity = await _context.Stores.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (entity == null) return;
        entity.Name = store.Name;
        entity.Location = store.Location;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Stores.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Stores.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Stores.Update(entity);
        await _context.SaveChangesAsync();
    }
}
