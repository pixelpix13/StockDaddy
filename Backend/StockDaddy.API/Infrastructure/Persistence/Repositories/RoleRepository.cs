using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<RoleDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Roles.Where(r => !r.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(r => EF.Functions.ILike(r.Name, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(r => new RoleDto
        {
                Id = r.Id,
                Name = r.Name,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<Role> ApplySort(IQueryable<Role> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("name", true) => query.OrderByDescending(r => r.Name),
            ("name", false) => query.OrderBy(r => r.Name),
            ("createdat", true) => query.OrderByDescending(r => r.CreatedAt),
            ("createdat", false) => query.OrderBy(r => r.CreatedAt),
            (_, true) => query.OrderByDescending(r => r.Id),
            _ => query.OrderBy(r => r.Id),
        };

    public async Task<List<RoleDto>> GetAllAsync()
    {
        return await _context.Roles
            .Where(r => !r.IsDeleted)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<RoleDto?> GetByIdAsync(int id)
    {
        return await _context.Roles
            .Where(r => r.Id == id && !r.IsDeleted)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateRoleRequest role)
    {
        var entity = new Role
        {
            Name = role.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Roles.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateRoleRequest role)
    {
        var entity = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        if (entity == null) return;
        entity.Name = role.Name;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Roles.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Roles.Update(entity);
        await _context.SaveChangesAsync();
    }
}
