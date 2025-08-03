using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly ApplicationDbContext _context;

    public PurchaseOrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PurchaseOrder>> GetAllAsync()
    {
        return await _context.PurchaseOrders
            .Where(o => !o.IsDeleted)
            .ToListAsync();
    }

    public async Task<PurchaseOrder?> GetByIdAsync(Guid id)
    {
        return await _context.PurchaseOrders
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
    }

    public async Task AddAsync(PurchaseOrder order)
    {
        await _context.PurchaseOrders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PurchaseOrder order)
    {
        _context.PurchaseOrders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var order = await _context.PurchaseOrders.FindAsync(id);
        if (order == null) return;

        _context.PurchaseOrders.Update(order);
        await _context.SaveChangesAsync();
    }
}
