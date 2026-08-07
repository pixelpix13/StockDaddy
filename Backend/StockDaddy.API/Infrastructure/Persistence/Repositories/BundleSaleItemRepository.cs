using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class BundleSaleItemRepository : IBundleSaleItemRepository
{
    private readonly ApplicationDbContext _context;

    public BundleSaleItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<BundleSaleItemDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.BundleSaleItems.Where(i => !i.IsDeleted);

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(i => new BundleSaleItemDto
        {
                Id = i.Id,
                SaleId = i.SaleId,
                BundleId = i.BundleId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<BundleSaleItem> ApplySort(IQueryable<BundleSaleItem> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(i => i.CreatedAt),
            ("createdat", false) => query.OrderBy(i => i.CreatedAt),
            (_, true) => query.OrderByDescending(i => i.Id),
            _ => query.OrderBy(i => i.Id),
        };

    public async Task<List<BundleSaleItemDto>> GetAllAsync()
    {
        return await _context.BundleSaleItems
            .Where(i => !i.IsDeleted)
            .Select(i => new BundleSaleItemDto
            {
                Id = i.Id,
                SaleId = i.SaleId,
                BundleId = i.BundleId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<BundleSaleItemDto?> GetByIdAsync(int id)
    {
        var i = await _context.BundleSaleItems.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (i == null) return null;
        return new BundleSaleItemDto
        {
            Id = i.Id,
            SaleId = i.SaleId,
            BundleId = i.BundleId,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            TotalPrice = i.TotalPrice,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        };
    }

    public async Task AddAsync(CreateBundleSaleItemRequest item)
    {
        var entity = new BundleSaleItem
        {
            SaleId = item.SaleId,
            BundleId = item.BundleId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.BundleSaleItems.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateBundleSaleItemRequest item)
    {
        var entity = await _context.BundleSaleItems.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (entity == null) return;

        entity.Quantity = item.Quantity;
        entity.UnitPrice = item.UnitPrice;
        entity.TotalPrice = item.TotalPrice;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.BundleSaleItems.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.BundleSaleItems.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.BundleSaleItems.Update(entity);
        await _context.SaveChangesAsync();
    }
}
