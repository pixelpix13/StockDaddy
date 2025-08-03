using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class StockItemRepository : IStockItemRepository
{
    private readonly ApplicationDbContext _context;

    public StockItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockItem>> GetAllAsync()
    {
        return await _context.StockItems
            .Where(i => !i.IsDeleted)
            .ToListAsync();
    }

    public async Task<StockItem?> GetByIdAsync(Guid id)
    {
        return await _context.StockItems
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task AddAsync(StockItem item)
    {
        await _context.StockItems.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(StockItem item)
    {
        _context.StockItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await _context.StockItems.FindAsync(id);
        if (item == null) return;

        _context.StockItems.Update(item);
        await _context.SaveChangesAsync();
    }
}
