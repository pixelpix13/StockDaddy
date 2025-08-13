
using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
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

    public async Task<List<TenantDto>> GetAllAsync()
    {
        return await _context.Tenants
            .Where(t => !t.IsDeleted)
            .Select(t => new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<TenantDto?> GetByIdAsync(int id)
    {
        return await _context.Tenants
            .Where(t => t.Id == id && !t.IsDeleted)
            .Select(t => new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateTenantRequest tenant)
    {
        var entity = new Tenant
        {
            Name = tenant.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Tenants.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateTenantRequest tenant)
    {
        var entity = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (entity == null) return;
        entity.Name = tenant.Name;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Tenants.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Tenants.Update(entity);
        await _context.SaveChangesAsync();
    }
}
