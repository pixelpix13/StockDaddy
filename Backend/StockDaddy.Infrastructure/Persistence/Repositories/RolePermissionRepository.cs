using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly ApplicationDbContext _context;

    public RolePermissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RolePermission>> GetAllAsync()
    {
        return await _context.RolePermissions
            .Where(rp => !rp.IsDeleted)
            .ToListAsync();
    }

    public async Task<RolePermission?> GetByIdAsync(Guid id)
    {
        return await _context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.Id == id && !rp.IsDeleted);
    }

    public async Task AddAsync(RolePermission rolePermission)
    {
        await _context.RolePermissions.AddAsync(rolePermission);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RolePermission rolePermission)
    {
        _context.RolePermissions.Update(rolePermission);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var rp = await _context.RolePermissions.FindAsync(id);
        if (rp == null) return;

        _context.RolePermissions.Update(rp);
        await _context.SaveChangesAsync();
    }
}
