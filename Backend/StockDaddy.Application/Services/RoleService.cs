using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class RoleService
{
    private readonly IRoleRepository _repo;

    public RoleService(IRoleRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<RoleDto>> GetAllAsync()
    {
        var roles = await _repo.GetAllAsync();
        return roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id)
    {
        var role = await _repo.GetByIdAsync(id);
        if (role == null) return null;

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            CreatedAt = role.CreatedAt
        };
    }

    public async Task AddAsync(CreateRoleRequest request)
    {
        var role = new Role
        {
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(role);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateRoleRequest request)
    {
        var role = await _repo.GetByIdAsync(id);
        if (role == null) return false;

        role.Name = request.Name;
        role.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(role);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var role = await _repo.GetByIdAsync(id);
        if (role == null) return false;

        // Soft delete logic
        role.IsDeleted = true;
        role.DeletedAt = DateTime.UtcNow;
        role.UpdatedAt = DateTime.UtcNow;
        
        await _repo.DeleteAsync(id);
        return true;
    }
}
