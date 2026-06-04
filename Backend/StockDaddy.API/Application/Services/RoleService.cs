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
        return await _repo.GetAllAsync();
    }

    public async Task<RoleDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<RoleDto> AddAsync(CreateRoleRequest request)
    {
        await _repo.AddAsync(request);
        // Assuming the repository returns the created RoleDto, otherwise fetch the latest
        var roles = await _repo.GetAllAsync();
        return roles.OrderByDescending(r => r.Id).First();
    }

    public async Task<RoleDto?> UpdateAsync(int id, UpdateRoleRequest request)
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
