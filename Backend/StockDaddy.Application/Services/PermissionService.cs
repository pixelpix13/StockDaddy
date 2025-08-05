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
        return await _repo.GetAllAsync();
    }

    public async Task<PermissionDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<PermissionDto> AddAsync(CreatePermissionRequest request)
    {
        await _repo.AddAsync(request);
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(p => p.Id).First();
    }

    public async Task<PermissionDto?> UpdateAsync(int id, UpdatePermissionRequest request)
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
