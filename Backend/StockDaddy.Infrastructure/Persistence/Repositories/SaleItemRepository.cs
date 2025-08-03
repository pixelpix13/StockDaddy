using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class SaleItemRepository : ISaleItemRepository
{
    private readonly ApplicationDbContext _context;

    public SaleItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SaleItem>> GetAllBySaleIdAsync(Guid saleId)
    {
        return await _context.SaleItems
            .Where(i => i.SaleId == saleId && !i.IsDeleted)
            .ToListAsync();
    }

    public async Task<SaleItem?> GetByIdAsync(Guid id)
    {
        return await _context.SaleItems
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task AddAsync(SaleItem item)
    {
        await _context.SaleItems.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SaleItem item)
    {
        _context.SaleItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await _context.SaleItems.FindAsync(id);
        if (item == null) return;

        _context.SaleItems.Update(item);
        await _context.SaveChangesAsync();
    }
}
