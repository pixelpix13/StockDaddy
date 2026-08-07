using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class BundleItemRepository : IBundleItemRepository
{
    private readonly ApplicationDbContext _context;

    public BundleItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<BundleItemDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.BundleItems.Where(b => !b.IsDeleted);

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(b => new BundleItemDto
        {
                Id = b.Id,
                BundleId = b.BundleId,
                ProductId = b.ProductId,
                Quantity = b.Quantity,
                EffectiveUnitPrice = b.EffectiveUnitPrice,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<BundleItem> ApplySort(IQueryable<BundleItem> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(b => b.CreatedAt),
            ("createdat", false) => query.OrderBy(b => b.CreatedAt),
            (_, true) => query.OrderByDescending(b => b.Id),
            _ => query.OrderBy(b => b.Id),
        };

    public async Task<List<BundleItemDto>> GetAllAsync()
    {
        return await _context.BundleItems
            .Where(b => !b.IsDeleted)
            .Select(b => new BundleItemDto
            {
                Id = b.Id,
                BundleId = b.BundleId,
                ProductId = b.ProductId,
                Quantity = b.Quantity,
                EffectiveUnitPrice = b.EffectiveUnitPrice,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<BundleItemDto?> GetByIdAsync(int id)
    {
        var b = await _context.BundleItems.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (b == null) return null;
        return new BundleItemDto
        {
            Id = b.Id,
            BundleId = b.BundleId,
            ProductId = b.ProductId,
            Quantity = b.Quantity,
            EffectiveUnitPrice = b.EffectiveUnitPrice,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        };
    }

    public async Task AddAsync(CreateBundleItemRequest item)
    {
        var entity = new BundleItem
        {
            BundleId = item.BundleId,
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            EffectiveUnitPrice = item.EffectiveUnitPrice,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.BundleItems.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateBundleItemRequest item)
    {
        var entity = await _context.BundleItems.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (entity == null) return;

        entity.Quantity = item.Quantity;
        entity.EffectiveUnitPrice = item.EffectiveUnitPrice;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.BundleItems.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.BundleItems.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.BundleItems.Update(entity);
        await _context.SaveChangesAsync();
    }
}
