using Microsoft.EntityFrameworkCore;
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

    public async Task<List<ProductBundle>> GetAllAsync()
    {
        return await _context.ProductBundles
            .Where(b => !b.IsDeleted)
            .ToListAsync();
    }

    public async Task<ProductBundle?> GetByIdAsync(Guid id)
    {
        return await _context.ProductBundles
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
    }

    public async Task AddAsync(ProductBundle bundle)
    {
        await _context.ProductBundles.AddAsync(bundle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductBundle bundle)
    {
        _context.ProductBundles.Update(bundle);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var bundle = await _context.ProductBundles.FindAsync(id);
        if (bundle == null) return;

        // IsDeleted already set in service layer
        _context.ProductBundles.Update(bundle);
        await _context.SaveChangesAsync();
    }
}
