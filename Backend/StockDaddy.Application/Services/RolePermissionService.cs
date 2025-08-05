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
        return await _repo.GetAllAsync();
    }

    public async Task<RolePermissionDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<RolePermissionDto> AddAsync(CreateRolePermissionRequest request)
    {
        await _repo.AddAsync(request);
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(rp => rp.Id).First();
    }

    public async Task<RolePermissionDto?> UpdateAsync(int id, UpdateRolePermissionRequest request)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;
        await _repo.UpdateAsync(id, request);
        return await _repo.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return false;
        await _repo.DeleteAsync(id);
        return true;
    }
}
