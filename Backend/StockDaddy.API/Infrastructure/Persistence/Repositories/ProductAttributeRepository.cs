using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class ProductAttributeRepository : IProductAttributeRepository
{
    private readonly ApplicationDbContext _context;

    public ProductAttributeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductAttributeDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.ProductAttributes.Where(a => !a.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(a => EF.Functions.ILike(a.AttributeName, pattern) || EF.Functions.ILike(a.AttributeValue, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(a => new ProductAttributeDto
        {
                Id = a.Id,
                ProductId = a.ProductId,
                AttributeName = a.AttributeName,
                AttributeValue = a.AttributeValue,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<ProductAttribute> ApplySort(IQueryable<ProductAttribute> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(a => a.CreatedAt),
            ("createdat", false) => query.OrderBy(a => a.CreatedAt),
            (_, true) => query.OrderByDescending(a => a.Id),
            _ => query.OrderBy(a => a.Id),
        };

    public async Task<List<ProductAttributeDto>> GetAllAsync()
    {
        return await _context.ProductAttributes
            .Where(a => !a.IsDeleted)
            .Select(a => new ProductAttributeDto
            {
                Id = a.Id,
                ProductId = a.ProductId,
                AttributeName = a.AttributeName,
                AttributeValue = a.AttributeValue,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<ProductAttributeDto?> GetByIdAsync(int id)
    {
        var a = await _context.ProductAttributes.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (a == null) return null;
        return new ProductAttributeDto
        {
            Id = a.Id,
            ProductId = a.ProductId,
            AttributeName = a.AttributeName,
            AttributeValue = a.AttributeValue,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        };
    }

    public async Task AddAsync(CreateProductAttributeRequest attribute)
    {
        var entity = new ProductAttribute
        {
            ProductId = attribute.ProductId,
            AttributeName = attribute.AttributeName,
            AttributeValue = attribute.AttributeValue,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.ProductAttributes.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateProductAttributeRequest attribute)
    {
        var entity = await _context.ProductAttributes.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (entity == null) return;

        entity.AttributeName = attribute.AttributeName;
        entity.AttributeValue = attribute.AttributeValue;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.ProductAttributes.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ProductAttributes.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.ProductAttributes.Update(entity);
        await _context.SaveChangesAsync();
    }
}
