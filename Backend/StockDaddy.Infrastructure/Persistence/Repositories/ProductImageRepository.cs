using Microsoft.EntityFrameworkCore;
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

    public async Task<List<ProductImage>> GetAllAsync()
    {
        return await _context.ProductImages
            .Where(i => !i.IsDeleted) // if soft delete is added
            .ToListAsync();
    }

    public async Task<ProductImage?> GetByIdAsync(Guid id)
    {
        return await _context.ProductImages
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task AddAsync(ProductImage image)
    {
        await _context.ProductImages.AddAsync(image);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductImage image)
    {
        _context.ProductImages.Update(image);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var image = await _context.ProductImages.FindAsync(id);
        if (image == null) return;

        image.IsDeleted = true;
        image.DeletedAt = DateTime.UtcNow;
        image.UpdatedAt = DateTime.UtcNow;

        _context.ProductImages.Update(image);
        await _context.SaveChangesAsync();
    }
}
