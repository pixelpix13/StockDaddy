using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public InvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Invoice>> GetAllAsync()
    {
        return await _context.Invoices
            .Where(i => !i.IsDeleted)
            .ToListAsync();
    }

    public async Task<Invoice?> GetByIdAsync(Guid id)
    {
        return await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task AddAsync(Invoice invoice)
    {
        await _context.Invoices.AddAsync(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Invoice invoice)
    {
        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null) return;

        _context.Invoices.Update(invoice); // Soft delete props already set in service
        await _context.SaveChangesAsync();
    }
}
