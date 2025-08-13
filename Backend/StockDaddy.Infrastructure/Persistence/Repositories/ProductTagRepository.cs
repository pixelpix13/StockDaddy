using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
namespace StockDaddy.Infrastructure.Repositories;

public class ProductTagRepository : IProductTagRepository
{
    private readonly ApplicationDbContext _context;

    public ProductTagRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductTagDto>> GetAllAsync()
    {
        return await _context.ProductTags
            .Where(t => !t.IsDeleted)
            .Select(t => new ProductTagDto
            {
                Id = t.Id,
                ProductId = t.ProductId,
                Tag = t.Tag,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<ProductTagDto?> GetByIdAsync(int id)
    {
        return await _context.ProductTags
            .Where(t => t.Id == id && !t.IsDeleted)
            .Select(t => new ProductTagDto
            {
                Id = t.Id,
                ProductId = t.ProductId,
                Tag = t.Tag,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateProductTagRequest tag)
    {
        var entity = new ProductTag
        {
            ProductId = tag.ProductId,
            Tag = tag.Tag,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.ProductTags.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateProductTagRequest tag)
    {
        var entity = await _context.ProductTags.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (entity == null) return;
        entity.Tag = tag.Tag;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.ProductTags.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ProductTags.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.ProductTags.Update(entity);
        await _context.SaveChangesAsync();
    }
}
