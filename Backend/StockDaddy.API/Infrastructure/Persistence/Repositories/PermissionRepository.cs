using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly ApplicationDbContext _context;

    public PermissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PermissionDto>> GetAllAsync()
    {
        return await _context.Permissions
            .Where(p => !p.IsDeleted)
            .Select(p => new PermissionDto
            {
                Id = p.Id,
                Module = p.Module,
                Action = p.Action,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<PermissionDto?> GetByIdAsync(int id)
    {
        var p = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (p == null) return null;
        return new PermissionDto
        {
            Id = p.Id,
            Module = p.Module,
            Action = p.Action,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }

    public async Task AddAsync(CreatePermissionRequest permission)
    {
        var entity = new Permission
        {
            Module = permission.Module,
            Action = permission.Action,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Permissions.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdatePermissionRequest permission)
    {
        var entity = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (entity == null) return;

        entity.Module = permission.Module;
        entity.Action = permission.Action;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Permissions.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Permissions.Update(entity);
        await _context.SaveChangesAsync();
    }
}
