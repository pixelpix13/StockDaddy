using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class AdjustedInvoiceRepository : IAdjustedInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public AdjustedInvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdjustedInvoice>> GetAllAsync()
    {
        return await _context.AdjustedInvoices
            .Where(ai => !ai.IsDeleted)
            .ToListAsync();
    }

    public async Task<AdjustedInvoice?> GetByIdAsync(Guid id)
    {
        return await _context.AdjustedInvoices
            .FirstOrDefaultAsync(ai => ai.Id == id && !ai.IsDeleted);
    }

    public async Task AddAsync(AdjustedInvoice invoice)
    {
        await _context.AdjustedInvoices.AddAsync(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AdjustedInvoice invoice)
    {
        _context.AdjustedInvoices.Update(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var invoice = await _context.AdjustedInvoices.FindAsync(id);
        if (invoice == null) return;

        invoice.IsDeleted = true;
        invoice.DeletedAt = DateTime.UtcNow;
        invoice.UpdatedAt = DateTime.UtcNow;

        _context.AdjustedInvoices.Update(invoice);
        await _context.SaveChangesAsync();
    }
}
