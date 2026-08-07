using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly ApplicationDbContext _context;

    public PermissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<PermissionDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Permissions.Where(p => !p.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(p => EF.Functions.ILike(p.Module, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(p => new PermissionDto
        {
                Id = p.Id,
                Module = p.Module,
                Action = p.Action,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<Permission> ApplySort(IQueryable<Permission> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(p => p.CreatedAt),
            ("createdat", false) => query.OrderBy(p => p.CreatedAt),
            (_, true) => query.OrderByDescending(p => p.Id),
            _ => query.OrderBy(p => p.Id),
        };

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
