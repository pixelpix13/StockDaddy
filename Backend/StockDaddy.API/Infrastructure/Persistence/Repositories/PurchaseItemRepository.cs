using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class PurchaseItemRepository : IPurchaseItemRepository
{
    private readonly ApplicationDbContext _context;

    public PurchaseItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<PurchaseItemDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.PurchaseItems.Where(p => !p.IsDeleted);

        if (q.PurchaseOrderId.HasValue)
        {
            baseQuery = baseQuery.Where(p => p.PurchaseOrderId == q.PurchaseOrderId.Value);
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(p => new PurchaseItemDto
        {
                Id = p.Id,
                PurchaseOrderId = p.PurchaseOrderId,
                ProductVariantId = p.ProductVariantId,
                Quantity = p.Quantity,
                QuantityReceived = p.QuantityReceived,
                UnitCost = p.UnitCost,
                TotalCost = p.TotalCost,
                ProductName = p.ProductVariant != null && p.ProductVariant.Product != null
                    ? p.ProductVariant.Product.Name
                    : null,
                SkuCode = p.ProductVariant != null ? p.ProductVariant.SkuCode : null,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<PurchaseItem> ApplySort(IQueryable<PurchaseItem> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(p => p.CreatedAt),
            ("createdat", false) => query.OrderBy(p => p.CreatedAt),
            (_, true) => query.OrderByDescending(p => p.Id),
            _ => query.OrderBy(p => p.Id),
        };

    public async Task<List<PurchaseItemDto>> GetAllAsync()
    {
        return await _context.PurchaseItems
            .Where(p => !p.IsDeleted)
            .Select(p => new PurchaseItemDto
            {
                Id = p.Id,
                PurchaseOrderId = p.PurchaseOrderId,
                ProductVariantId = p.ProductVariantId,
                Quantity = p.Quantity,
                UnitCost = p.UnitCost,
                TotalCost = p.TotalCost,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<PurchaseItemDto?> GetByIdAsync(int id)
    {
        return await _context.PurchaseItems
            .Where(p => p.Id == id && !p.IsDeleted)
            .Select(p => new PurchaseItemDto
            {
                Id = p.Id,
                PurchaseOrderId = p.PurchaseOrderId,
                ProductVariantId = p.ProductVariantId,
                Quantity = p.Quantity,
                UnitCost = p.UnitCost,
                TotalCost = p.TotalCost,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreatePurchaseItemRequest item)
    {
        var entity = new PurchaseItem
        {
            PurchaseOrderId = item.PurchaseOrderId,
            ProductVariantId = item.ProductVariantId,
            Quantity = item.Quantity,
            UnitCost = item.UnitCost,
            TotalCost = item.UnitCost * item.Quantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.PurchaseItems.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdatePurchaseItemRequest item)
    {
        var entity = await _context.PurchaseItems.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (entity == null) return;
        entity.Quantity = item.Quantity;
        entity.UnitCost = item.UnitCost;
        entity.TotalCost = item.UnitCost * item.Quantity;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.PurchaseItems.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.PurchaseItems.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.PurchaseItems.Update(entity);
        await _context.SaveChangesAsync();
    }
}
