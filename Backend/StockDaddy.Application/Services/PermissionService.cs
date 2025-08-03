using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class PermissionService
{
    private readonly IPermissionRepository _repo;

    public PermissionService(IPermissionRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<PermissionDto>> GetAllAsync()
    {
        var permissions = await _repo.GetAllAsync();
        return permissions.Select(p => new PermissionDto
        {
            Id = p.Id,
            Module = p.Module,
            Action = p.Action,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();
    }

    public async Task<PermissionDto?> GetByIdAsync(Guid id)
    {
        var p = await _repo.GetByIdAsync(id);
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

    public async Task AddAsync(CreatePermissionRequest request)
    {
        var p = new Permission
        {
            Module = request.Module,
            Action = request.Action,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(p);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdatePermissionRequest request)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) return false;

        p.Module = request.Module;
        p.Action = request.Action;
        p.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(p);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
