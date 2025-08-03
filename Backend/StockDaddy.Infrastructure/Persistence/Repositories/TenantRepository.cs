using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly ApplicationDbContext _context;

    public TenantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tenant>> GetAllAsync()
    {
        return await _context.Tenants
            .Where(t => !t.IsDeleted)
            .ToListAsync();
    }

    public async Task<Tenant?> GetByIdAsync(Guid id)
    {
        return await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
    }

    public async Task AddAsync(Tenant tenant)
    {
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Tenant tenant)
    {
        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        if (tenant == null) return;

        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync();
    }
}
