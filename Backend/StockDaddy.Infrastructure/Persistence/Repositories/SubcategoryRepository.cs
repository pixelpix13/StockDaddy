using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class SubcategoryRepository : ISubcategoryRepository
{
    private readonly ApplicationDbContext _context;

    public SubcategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Subcategory>> GetAllAsync()
    {
        return await _context.Subcategories
            .Where(s => !s.IsDeleted)
            .ToListAsync();
    }

    public async Task<Subcategory?> GetByIdAsync(Guid id)
    {
        return await _context.Subcategories
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }

    public async Task AddAsync(Subcategory subcategory)
    {
        await _context.Subcategories.AddAsync(subcategory);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Subcategory subcategory)
    {
        _context.Subcategories.Update(subcategory);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var subcategory = await _context.Subcategories.FindAsync(id);
        if (subcategory == null) return;

        _context.Subcategories.Update(subcategory);
        await _context.SaveChangesAsync();
    }
}
