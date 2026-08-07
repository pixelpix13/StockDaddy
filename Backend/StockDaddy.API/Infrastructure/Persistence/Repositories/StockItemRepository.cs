using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Domain.Enums;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class StockItemRepository : IStockItemRepository
{
    private readonly ApplicationDbContext _context;

    public StockItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<StockItemDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.StockItems.Where(i => !i.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(i => EF.Functions.ILike(i.Status.ToString(), pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(i => new StockItemDto
        {
                Id = i.Id,
                ProductId = i.ProductId,
                StoreId = i.StoreId,
                Quantity = i.Quantity,
                Status = i.Status,
                LastUpdated = i.LastUpdated,
                UpdatedBy = i.UpdatedBy,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<StockItem> ApplySort(IQueryable<StockItem> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(i => i.CreatedAt),
            ("createdat", false) => query.OrderBy(i => i.CreatedAt),
            (_, true) => query.OrderByDescending(i => i.Id),
            _ => query.OrderBy(i => i.Id),
        };

    public async Task<List<StockItemDto>> GetAllAsync()
    {
        return await _context.StockItems
            .Where(i => !i.IsDeleted)
            .Select(i => new StockItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                StoreId = i.StoreId,
                Quantity = i.Quantity,
                Status = i.Status,
                LastUpdated = i.LastUpdated,
                UpdatedBy = i.UpdatedBy,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<StockItemDto?> GetByIdAsync(int id)
    {
        return await _context.StockItems
            .Where(i => i.Id == id && !i.IsDeleted)
            .Select(i => new StockItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                StoreId = i.StoreId,
                Quantity = i.Quantity,
                Status = i.Status,
                LastUpdated = i.LastUpdated,
                UpdatedBy = i.UpdatedBy,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateStockItemRequest item)
    {
        var entity = new StockItem
        {
            ProductId = item.ProductId,
            StoreId = item.StoreId,
            Quantity = item.Quantity,
            Status = item.Status,
            LastUpdated = DateTime.UtcNow,
            UpdatedBy = item.UpdatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.StockItems.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateStockItemRequest item)
    {
        var entity = await _context.StockItems.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (entity == null) return;
        entity.Quantity = item.Quantity;
        entity.Status = item.Status;
        entity.UpdatedBy = item.UpdatedBy;
        entity.LastUpdated = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.StockItems.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.StockItems.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.StockItems.Update(entity);
        await _context.SaveChangesAsync();
    }
}
