using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class BundleItemRepository : IBundleItemRepository
{
    private readonly ApplicationDbContext _context;

    public BundleItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BundleItem>> GetAllAsync()
    {
        return await _context.BundleItems
            .Where(b => !b.IsDeleted)
            .ToListAsync();
    }

    public async Task<BundleItem?> GetByIdAsync(Guid id)
    {
        return await _context.BundleItems
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
    }

    public async Task AddAsync(BundleItem item)
    {
        await _context.BundleItems.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BundleItem item)
    {
        _context.BundleItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await _context.BundleItems.FindAsync(id);
        if (item == null) return;

        item.IsDeleted = true;
        item.DeletedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;

        _context.BundleItems.Update(item);
        await _context.SaveChangesAsync();
    }
}
