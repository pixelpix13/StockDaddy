using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class RolePermissionService
{
    private readonly IRolePermissionRepository _repo;

    public RolePermissionService(IRolePermissionRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<RolePermissionDto>> GetAllAsync()
    {
        var data = await _repo.GetAllAsync();
        return data.Select(x => new RolePermissionDto
        {
            Id = x.Id,
            RoleId = x.RoleId,
            PermissionId = x.PermissionId,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }).ToList();
    }

    public async Task<RolePermissionDto?> GetByIdAsync(Guid id)
    {
        var x = await _repo.GetByIdAsync(id);
        if (x == null) return null;

        return new RolePermissionDto
        {
            Id = x.Id,
            RoleId = x.RoleId,
            PermissionId = x.PermissionId,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        };
    }

    public async Task AddAsync(CreateRolePermissionRequest request)
    {
        var x = new RolePermission
        {
            RoleId = request.RoleId,
            PermissionId = request.PermissionId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(x);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateRolePermissionRequest request)
    {
        var x = await _repo.GetByIdAsync(id);
        if (x == null) return false;

        x.RoleId = request.RoleId;
        x.PermissionId = request.PermissionId;
        x.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(x);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var x = await _repo.GetByIdAsync(id);
        if (x == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
