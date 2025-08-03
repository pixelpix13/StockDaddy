using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class ProductVariantRepository : IProductVariantRepository
{
    private readonly ApplicationDbContext _context;

    public ProductVariantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductVariant>> GetAllAsync()
    {
        return await _context.ProductVariants
            .Where(v => !v.IsDeleted)
            .ToListAsync();
    }

    public async Task<ProductVariant?> GetByIdAsync(Guid id)
    {
        return await _context.ProductVariants
            .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
    }

    public async Task AddAsync(ProductVariant variant)
    {
        await _context.ProductVariants.AddAsync(variant);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductVariant variant)
    {
        _context.ProductVariants.Update(variant);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var variant = await _context.ProductVariants.FindAsync(id);
        if (variant == null) return;

        _context.ProductVariants.Update(variant);
        await _context.SaveChangesAsync();
    }
}
