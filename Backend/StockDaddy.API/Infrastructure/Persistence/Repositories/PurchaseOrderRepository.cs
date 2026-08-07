using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Domain.Enums;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly ApplicationDbContext _context;

    public PurchaseOrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<PurchaseOrderDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.PurchaseOrders.Where(o => !o.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = RepositoryPaging.LikePattern(q.Search);
            var idMatch = RepositoryPaging.TryParseSearchId(q.Search, out var searchId);
            baseQuery = baseQuery.Where(o =>
                (idMatch && o.Id == searchId) ||
                (idMatch && o.SupplierId == searchId) ||
                EF.Functions.ILike(o.Notes ?? string.Empty, pattern) ||
                EF.Functions.ILike(o.Status.ToString(), pattern));
        }

        if (!string.IsNullOrEmpty(q.Status) &&
            Enum.TryParse<PurchaseOrderStatus>(q.Status, true, out var statusFilter))
        {
            baseQuery = baseQuery.Where(o => o.Status == statusFilter);
        }

        if (q.SupplierId.HasValue)
        {
            baseQuery = baseQuery.Where(o => o.SupplierId == q.SupplierId.Value);
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(o => new PurchaseOrderDto
        {
                Id = o.Id,
                TenantId = o.TenantId,
                SupplierId = o.SupplierId,
                StoreId = o.StoreId,
                OrderDate = o.OrderDate,
                ExpectedDelivery = o.ExpectedDelivery,
                Status = o.Status,
                Notes = o.Notes,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<PurchaseOrder> ApplySort(IQueryable<PurchaseOrder> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(o => o.CreatedAt),
            ("createdat", false) => query.OrderBy(o => o.CreatedAt),
            (_, true) => query.OrderByDescending(o => o.Id),
            _ => query.OrderBy(o => o.Id),
        };

    public async Task<List<PurchaseOrderDto>> GetAllAsync()
    {
        return await _context.PurchaseOrders
            .Where(o => !o.IsDeleted)
            .Select(o => new PurchaseOrderDto
            {
                Id = o.Id,
                TenantId = o.TenantId,
                SupplierId = o.SupplierId,
                StoreId = o.StoreId,
                OrderDate = o.OrderDate,
                ExpectedDelivery = o.ExpectedDelivery,
                Status = o.Status,
                Notes = o.Notes,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<PurchaseOrderDto?> GetByIdAsync(int id)
    {
        return await _context.PurchaseOrders
            .Where(o => o.Id == id && !o.IsDeleted)
            .Select(o => new PurchaseOrderDto
            {
                Id = o.Id,
                TenantId = o.TenantId,
                SupplierId = o.SupplierId,
                StoreId = o.StoreId,
                OrderDate = o.OrderDate,
                ExpectedDelivery = o.ExpectedDelivery,
                Status = o.Status,
                Notes = o.Notes,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreatePurchaseOrderRequest order)
    {
        var entity = new PurchaseOrder
        {
            TenantId = order.TenantId,
            SupplierId = order.SupplierId,
            StoreId = order.StoreId,
            OrderDate = order.OrderDate,
            ExpectedDelivery = order.ExpectedDelivery,
            Status = order.Status,
            Notes = order.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.PurchaseOrders.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdatePurchaseOrderRequest order)
    {
        var entity = await _context.PurchaseOrders.FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        if (entity == null) return;
        entity.ExpectedDelivery = order.ExpectedDelivery;
        entity.Status = order.Status;
        entity.Notes = order.Notes;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.PurchaseOrders.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.PurchaseOrders.FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.PurchaseOrders.Update(entity);
        await _context.SaveChangesAsync();
    }
}
