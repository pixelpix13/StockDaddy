using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly ApplicationDbContext _context;

    public RolePermissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<RolePermissionDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.RolePermissions.Where(rp => !rp.IsDeleted);

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(rp => new RolePermissionDto
        {
                Id = rp.Id,
                RoleId = rp.RoleId,
                PermissionId = rp.PermissionId,
                CreatedAt = rp.CreatedAt,
                UpdatedAt = rp.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<RolePermission> ApplySort(IQueryable<RolePermission> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(rp => rp.CreatedAt),
            ("createdat", false) => query.OrderBy(rp => rp.CreatedAt),
            (_, true) => query.OrderByDescending(rp => rp.Id),
            _ => query.OrderBy(rp => rp.Id),
        };

    public async Task<List<RolePermissionDto>> GetAllAsync()
    {
        return await _context.RolePermissions
            .Where(rp => !rp.IsDeleted)
            .Select(rp => new RolePermissionDto
            {
                Id = rp.Id,
                RoleId = rp.RoleId,
                PermissionId = rp.PermissionId,
                CreatedAt = rp.CreatedAt,
                UpdatedAt = rp.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<RolePermissionDto?> GetByIdAsync(int id)
    {
        return await _context.RolePermissions
            .Where(rp => rp.Id == id && !rp.IsDeleted)
            .Select(rp => new RolePermissionDto
            {
                Id = rp.Id,
                RoleId = rp.RoleId,
                PermissionId = rp.PermissionId,
                CreatedAt = rp.CreatedAt,
                UpdatedAt = rp.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateRolePermissionRequest rolePermission)
    {
        var entity = new RolePermission
        {
            RoleId = rolePermission.RoleId,
            PermissionId = rolePermission.PermissionId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.RolePermissions.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateRolePermissionRequest rolePermission)
    {
        var entity = await _context.RolePermissions.FirstOrDefaultAsync(rp => rp.Id == id && !rp.IsDeleted);
        if (entity == null) return;
        entity.RoleId = rolePermission.RoleId;
        entity.PermissionId = rolePermission.PermissionId;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.RolePermissions.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.RolePermissions.FirstOrDefaultAsync(rp => rp.Id == id && !rp.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.RolePermissions.Update(entity);
        await _context.SaveChangesAsync();
    }
}
