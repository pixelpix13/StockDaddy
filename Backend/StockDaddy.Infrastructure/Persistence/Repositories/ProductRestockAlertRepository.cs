using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class ProductRestockAlertRepository : IProductRestockAlertRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRestockAlertRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductRestockAlert>> GetAllAsync()
    {
        return await _context.ProductRestockAlerts.ToListAsync();
    }

    public async Task<ProductRestockAlert?> GetByIdAsync(Guid id)
    {
        return await _context.ProductRestockAlerts.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAsync(ProductRestockAlert alert)
    {
        await _context.ProductRestockAlerts.AddAsync(alert);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductRestockAlert alert)
    {
        _context.ProductRestockAlerts.Update(alert);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
{
    var alert = await _context.ProductRestockAlerts.FindAsync(id);
    if (alert == null) return;

    _context.ProductRestockAlerts.Update(alert);
    await _context.SaveChangesAsync();
}

}
