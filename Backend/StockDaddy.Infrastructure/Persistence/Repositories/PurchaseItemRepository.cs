using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class PurchaseItemRepository : IPurchaseItemRepository
{
    private readonly ApplicationDbContext _context;

    public PurchaseItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PurchaseItem>> GetAllAsync()
    {
        return await _context.PurchaseItems
            .Where(p => !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<PurchaseItem?> GetByIdAsync(Guid id)
    {
        return await _context.PurchaseItems
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task AddAsync(PurchaseItem item)
    {
        await _context.PurchaseItems.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PurchaseItem item)
    {
        _context.PurchaseItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await _context.PurchaseItems.FindAsync(id);
        if (item == null) return;

        _context.PurchaseItems.Update(item); // Soft delete already done in service
        await _context.SaveChangesAsync();
    }
}
