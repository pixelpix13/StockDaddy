
using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

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

    public async Task AddAsync(CreateProductRequest product)
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
