using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
namespace StockDaddy.Infrastructure.Repositories;

public class ProductTagRepository : IProductTagRepository
{
    private readonly ApplicationDbContext _context;

    public ProductTagRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductTag>> GetAllAsync()
    {
        return await _context.ProductTags
            .Where(t => !t.IsDeleted)
            .ToListAsync();
    }

    public async Task<ProductTag?> GetByIdAsync(Guid id)
    {
        return await _context.ProductTags
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
    }

    public async Task AddAsync(ProductTag tag)
    {
        await _context.ProductTags.AddAsync(tag);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductTag tag)
    {
        _context.ProductTags.Update(tag);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var tag = await _context.ProductTags.FindAsync(id);
        if (tag == null) return;
        _context.ProductTags.Update(tag);
        await _context.SaveChangesAsync();
    }
}
