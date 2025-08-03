using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class StoreRepository : IStoreRepository
{
    private readonly ApplicationDbContext _context;

    public StoreRepository(ApplicationDbContext context)
    {
        _context = context; 
    }

    public async Task<List<Store>> GetAllAsync()
    {
        return await _context.Stores
            .Where(s => !s.IsDeleted)
            .ToListAsync();
    }

    public async Task<Store?> GetByIdAsync(Guid id)
    {
        return await _context.Stores
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }

    public async Task AddAsync(Store store)
    {
        await _context.Stores.AddAsync(store);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Store store)
    {
        _context.Stores.Update(store);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null) return;

        _context.Stores.Update(store);
        await _context.SaveChangesAsync();
    }
}
