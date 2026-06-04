using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly ApplicationDbContext _context;

    public ProductImageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductImageDto>> GetAllAsync()
    {
        return await _context.ProductImages
            .Where(i => !i.IsDeleted)
            .Select(i => new ProductImageDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ImageUrl = i.ImageUrl,
                IsPrimary = i.IsPrimary,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<ProductImageDto?> GetByIdAsync(int id)
    {
        var i = await _context.ProductImages.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (i == null) return null;
        return new ProductImageDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ImageUrl = i.ImageUrl,
            IsPrimary = i.IsPrimary,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        };
    }

    public async Task AddAsync(CreateProductImageRequest image)
    {
        var entity = new ProductImage
        {
            ProductId = image.ProductId,
            ImageUrl = image.ImageUrl,
            IsPrimary = image.IsPrimary,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.ProductImages.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateProductImageRequest image)
    {
        var entity = await _context.ProductImages.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (entity == null) return;

        entity.ImageUrl = image.ImageUrl;
        entity.IsPrimary = image.IsPrimary;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.ProductImages.Update(entity);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ProductImages.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (entity == null) return;

        _context.ProductImages.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
