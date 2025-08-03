using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class TaxRegionRepository : ITaxRegionRepository
{
    private readonly ApplicationDbContext _context;

    public TaxRegionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaxRegion>> GetAllAsync()
    {
        return await _context.TaxRegions
            .Where(r => !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<TaxRegion?> GetByIdAsync(Guid id)
    {
        return await _context.TaxRegions
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }

    public async Task AddAsync(TaxRegion region)
    {
        await _context.TaxRegions.AddAsync(region);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TaxRegion region)
    {
        _context.TaxRegions.Update(region);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var region = await _context.TaxRegions.FindAsync(id);
        if (region == null) return;

        _context.TaxRegions.Update(region);
        await _context.SaveChangesAsync();
    }
}
