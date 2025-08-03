using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class ReturnRepository : IReturnRepository
{
    private readonly ApplicationDbContext _context;

    public ReturnRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Return>> GetAllAsync()
    {
        return await _context.Returns
            .Where(r => !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<Return?> GetByIdAsync(Guid id)
    {
        return await _context.Returns
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }

    public async Task AddAsync(Return r)
    {
        await _context.Returns.AddAsync(r);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Return r)
    {
        _context.Returns.Update(r);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var r = await _context.Returns.FindAsync(id);
        if (r == null) return;

        _context.Returns.Update(r); // The entity is already soft-deleted in the service
        await _context.SaveChangesAsync();
    }
}
