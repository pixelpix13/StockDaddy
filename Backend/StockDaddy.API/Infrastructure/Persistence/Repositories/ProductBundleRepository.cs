using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class ProductBundleRepository : IProductBundleRepository
{
    private readonly ApplicationDbContext _context;

    public ProductBundleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductBundleDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.ProductBundles.Where(b => !b.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(b => EF.Functions.ILike(b.Name, pattern) || EF.Functions.ILike(b.Description, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(b => new ProductBundleDto
        {
                Id = b.Id,
                TenantId = b.TenantId,
                Name = b.Name,
                Description = b.Description,
                Price = b.Price
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<ProductBundle> ApplySort(IQueryable<ProductBundle> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("name", true) => query.OrderByDescending(b => b.Name),
            ("name", false) => query.OrderBy(b => b.Name),
            ("createdat", true) => query.OrderByDescending(b => b.CreatedAt),
            ("createdat", false) => query.OrderBy(b => b.CreatedAt),
            (_, true) => query.OrderByDescending(b => b.Id),
            _ => query.OrderBy(b => b.Id),
        };

    public async Task<List<ProductBundleDto>> GetAllAsync()
    {
        return await _context.ProductBundles
            .Where(b => !b.IsDeleted)
            .Select(b => new ProductBundleDto
            {
                Id = b.Id,
                TenantId = b.TenantId,
                Name = b.Name,
                Description = b.Description,
                Price = b.Price
            })
            .ToListAsync();
    }

    public async Task<ProductBundleDto?> GetByIdAsync(int id)
    {
        var b = await _context.ProductBundles.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (b == null) return null;
        return new ProductBundleDto
        {
            Id = b.Id,
            TenantId = b.TenantId,
            Name = b.Name,
            Description = b.Description,
            Price = b.Price
        };
    }

    public async Task AddAsync(CreateProductBundleRequest bundle)
    {
        var entity = new ProductBundle
        {
            TenantId = bundle.TenantId,
            Name = bundle.Name,
            Description = bundle.Description,
            Price = bundle.Price,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.ProductBundles.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateProductBundleRequest bundle)
    {
        var entity = await _context.ProductBundles.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (entity == null) return;

        entity.Name = bundle.Name;
        entity.Description = bundle.Description;
        entity.Price = bundle.Price;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.ProductBundles.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ProductBundles.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.ProductBundles.Update(entity);
        await _context.SaveChangesAsync();
    }
}
