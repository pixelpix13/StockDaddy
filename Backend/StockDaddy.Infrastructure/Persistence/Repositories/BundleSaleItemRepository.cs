using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class BundleSaleItemRepository : IBundleSaleItemRepository
{
    private readonly ApplicationDbContext _context;

    public BundleSaleItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BundleSaleItem>> GetAllAsync()
    {
        return await _context.BundleSaleItems
            .Where(i => !i.IsDeleted)
            .ToListAsync();
    }

    public async Task<BundleSaleItem?> GetByIdAsync(Guid id)
    {
        return await _context.BundleSaleItems
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task AddAsync(BundleSaleItem item)
    {
        await _context.BundleSaleItems.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BundleSaleItem item)
    {
        _context.BundleSaleItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await _context.BundleSaleItems.FindAsync(id);
        if (item == null) return;

        item.IsDeleted = true;
        item.DeletedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;

        _context.BundleSaleItems.Update(item);
        await _context.SaveChangesAsync();
    }
}
