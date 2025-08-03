using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class ProductAttributeRepository : IProductAttributeRepository
{
    private readonly ApplicationDbContext _context;

    public ProductAttributeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductAttribute>> GetAllAsync()
    {
        return await _context.ProductAttributes
            .Where(a => !a.IsDeleted)
            .ToListAsync();
    }

    public async Task<ProductAttribute?> GetByIdAsync(Guid id)
    {
        return await _context.ProductAttributes
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
    }

    public async Task AddAsync(ProductAttribute attribute)
    {
        await _context.ProductAttributes.AddAsync(attribute);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductAttribute attribute)
    {
        _context.ProductAttributes.Update(attribute);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var attr = await _context.ProductAttributes.FindAsync(id);
        if (attr == null) return;

        _context.ProductAttributes.Update(attr); // Soft delete fields already updated in service
        await _context.SaveChangesAsync();
    }
}
