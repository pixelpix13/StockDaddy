using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class RefundRepository : IRefundRepository
{
    private readonly ApplicationDbContext _context;

    public RefundRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Refund>> GetAllAsync()
    {
        return await _context.Refunds
            .Where(r => !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<Refund?> GetByIdAsync(Guid id)
    {
        return await _context.Refunds
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }

    public async Task AddAsync(Refund refund)
    {
        await _context.Refunds.AddAsync(refund);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Refund refund)
    {
        _context.Refunds.Update(refund);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var refund = await _context.Refunds.FindAsync(id);
        if (refund == null) return;

        _context.Refunds.Update(refund); // Soft delete already done in service
        await _context.SaveChangesAsync();
    }
}
