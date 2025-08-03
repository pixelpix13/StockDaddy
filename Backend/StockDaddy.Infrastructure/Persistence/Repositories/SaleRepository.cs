using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly ApplicationDbContext _context;

    public SaleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Sale>> GetAllAsync()
    {
        return await _context.Sales
            .Where(s => !s.IsDeleted)
            .ToListAsync();
    }

    public async Task<Sale?> GetByIdAsync(Guid id)
    {
        return await _context.Sales
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }

    public async Task AddAsync(Sale sale)
    {
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Sale sale)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var sale = await _context.Sales.FindAsync(id);
        if (sale == null) return;
        
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync();
    }
}
