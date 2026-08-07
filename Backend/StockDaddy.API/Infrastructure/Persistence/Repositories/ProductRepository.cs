
using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Products.Where(p => !p.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(p => EF.Functions.ILike(p.Name, pattern) || EF.Functions.ILike(p.Description, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(p => new ProductDto
        {
                Id = p.Id,
                TenantId = p.TenantId,
                StoreId = p.StoreId,
                SubcategoryId = p.SubcategoryId,
                Name = p.Name,
                Description = p.Description,
                Unit = p.Unit,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                IsDeleted = p.IsDeleted,
                DeletedAt = p.DeletedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<Product> ApplySort(IQueryable<Product> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("name", true) => query.OrderByDescending(p => p.Name),
            ("name", false) => query.OrderBy(p => p.Name),
            ("createdat", true) => query.OrderByDescending(p => p.CreatedAt),
            ("createdat", false) => query.OrderBy(p => p.CreatedAt),
            (_, true) => query.OrderByDescending(p => p.Id),
            _ => query.OrderBy(p => p.Id),
        };

    public async Task<List<ProductDto>> GetAllAsync()
    {
        return await _context.Products
            .Where(p => !p.IsDeleted)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                TenantId = p.TenantId,
                StoreId = p.StoreId,
                SubcategoryId = p.SubcategoryId,
                Name = p.Name,
                Description = p.Description,
                Unit = p.Unit,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                IsDeleted = p.IsDeleted,
                DeletedAt = p.DeletedAt
            })
            .ToListAsync();
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var p = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (p == null) return null;
        return new ProductDto
        {
            Id = p.Id,
            TenantId = p.TenantId,
            StoreId = p.StoreId,
            SubcategoryId = p.SubcategoryId,
            Name = p.Name,
            Description = p.Description,
            Unit = p.Unit,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            IsDeleted = p.IsDeleted,
            DeletedAt = p.DeletedAt
        };
    }

    public async Task<int> AddAsync(CreateProductRequest product)
    {
        var entity = new Product
        {
            TenantId = product.TenantId,
            StoreId = product.StoreId,
            SubcategoryId = product.SubcategoryId,
            Name = product.Name,
            Description = product.Description,
            Unit = product.Unit,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Products.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(int id, UpdateProductRequest product)
    {
        var entity = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (entity == null) return;

        entity.StoreId = product.StoreId;
        entity.SubcategoryId = product.SubcategoryId;
        entity.Name = product.Name;
        entity.Description = product.Description;
        entity.Unit = product.Unit;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Products.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var entity = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Products.Update(entity);
        await _context.SaveChangesAsync();
    }
}
