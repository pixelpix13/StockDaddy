using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class ProductRestockAlertRepository : IProductRestockAlertRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRestockAlertRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductRestockAlertDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.ProductRestockAlerts.Where(a => !a.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = RepositoryPaging.LikePattern(q.Search);
            var idMatch = RepositoryPaging.TryParseSearchId(q.Search, out var searchId);
            baseQuery = baseQuery.Where(a =>
                (idMatch && a.Id == searchId) ||
                (idMatch && a.ProductId == searchId) ||
                (idMatch && a.VariantId == searchId) ||
                EF.Functions.ILike(a.Status, pattern));
        }

        if (!string.IsNullOrEmpty(q.Status))
        {
            baseQuery = baseQuery.Where(a => a.Status == q.Status);
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(a => new ProductRestockAlertDto
        {
                Id = a.Id,
                ProductId = a.ProductId,
                StoreId = a.StoreId,
                VariantId = a.VariantId,
                TriggeredAt = a.TriggeredAt,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<ProductRestockAlert> ApplySort(IQueryable<ProductRestockAlert> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(a => a.CreatedAt),
            ("createdat", false) => query.OrderBy(a => a.CreatedAt),
            (_, true) => query.OrderByDescending(a => a.Id),
            _ => query.OrderBy(a => a.Id),
        };

    public async Task<List<ProductRestockAlertDto>> GetAllAsync()
    {
        return await _context.ProductRestockAlerts
            .Where(a => !a.IsDeleted)
            .Select(a => new ProductRestockAlertDto
            {
                Id = a.Id,
                ProductId = a.ProductId,
                StoreId = a.StoreId,
                VariantId = a.VariantId,
                TriggeredAt = a.TriggeredAt,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<ProductRestockAlertDto?> GetByIdAsync(int id)
    {
        return await _context.ProductRestockAlerts
            .Where(a => a.Id == id && !a.IsDeleted)
            .Select(a => new ProductRestockAlertDto
            {
                Id = a.Id,
                ProductId = a.ProductId,
                StoreId = a.StoreId,
                VariantId = a.VariantId,
                TriggeredAt = a.TriggeredAt,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateProductRestockAlertRequest alert)
    {
        var entity = new ProductRestockAlert
        {
            ProductId = alert.ProductId,
            StoreId = alert.StoreId,
            VariantId = alert.VariantId,
            Status = alert.Status,
            TriggeredAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.ProductRestockAlerts.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateProductRestockAlertRequest alert)
    {
        var entity = await _context.ProductRestockAlerts.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (entity == null) return;
        entity.Status = alert.Status;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.ProductRestockAlerts.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ProductRestockAlerts.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.ProductRestockAlerts.Update(entity);
        await _context.SaveChangesAsync();
    }

}
