using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class ProductBundleRepository : IProductBundleRepository
{
    private readonly ApplicationDbContext _context;

    public ProductBundleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

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
