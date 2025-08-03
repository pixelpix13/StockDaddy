using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class HsnMasterRepository : IHsnMasterRepository
{
    private readonly ApplicationDbContext _context;

    public HsnMasterRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HsnMaster>> GetAllAsync()
    {
        return await _context.HsnMaster
            .Where(h => !h.IsDeleted)
            .ToListAsync();
    }

    public async Task<HsnMaster?> GetByIdAsync(Guid id)
    {
        return await _context.HsnMaster
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted);
    }

    public async Task AddAsync(HsnMaster hsn)
    {
        await _context.HsnMaster.AddAsync(hsn);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(HsnMaster hsn)
    {
        _context.HsnMaster.Update(hsn);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var hsn = await _context.HsnMaster.FindAsync(id);
        if (hsn == null) return;

        _context.HsnMaster.Update(hsn); // already marked deleted in service
        await _context.SaveChangesAsync();
    }
}
