using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class GiftOptionRepository : IGiftOptionRepository
{
    private readonly ApplicationDbContext _context;

    public GiftOptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<GiftOption>> GetAllAsync()
    {
        return await _context.GiftOptions
            .Where(g => !g.IsDeleted)
            .ToListAsync();
    }

    public async Task<GiftOption?> GetByIdAsync(Guid id)
    {
        return await _context.GiftOptions
            .FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted);
    }

    public async Task AddAsync(GiftOption option)
    {
        await _context.GiftOptions.AddAsync(option);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(GiftOption option)
    {
        _context.GiftOptions.Update(option);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var giftOption = await _context.GiftOptions.FindAsync(id);
        if (giftOption == null) return;

        // Entity is already soft-marked by service
        _context.GiftOptions.Update(giftOption);
        await _context.SaveChangesAsync();
    }
}
