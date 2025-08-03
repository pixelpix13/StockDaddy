using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await _context.Roles
            .Where(r => !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }

    public async Task AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Role role)
    {
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return;

        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
    }
}
