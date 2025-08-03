using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence ;

namespace StockDaddy.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly ApplicationDbContext _context;

    public PermissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Permission>> GetAllAsync()
    {
        return await _context.Permissions
            .Where(p => !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<Permission?> GetByIdAsync(Guid id)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task AddAsync(Permission permission)
    {
        await _context.Permissions.AddAsync(permission);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Permission permission)
    {
        _context.Permissions.Update(permission);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var permission = await _context.Permissions.FindAsync(id);
        if (permission == null) return;

        _context.Permissions.Update(permission); // assumes soft delete is already set in service
        await _context.SaveChangesAsync();
    }
}
