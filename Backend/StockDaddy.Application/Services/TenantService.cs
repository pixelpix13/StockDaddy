using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class TenantService
{
    private readonly ITenantRepository _repo;

    public TenantService(ITenantRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TenantDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<TenantDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<TenantDto?> AddAsync(CreateTenantRequest request)
    {
        await _repo.AddAsync(request);
        // Optionally, fetch the created tenant (e.g., by unique name or by returning from repo)
        // For now, return null as placeholder if repo does not return the created tenant
        return null;
    }

    public async Task<TenantDto?> UpdateAsync(int id, UpdateTenantRequest request)
    {
        var tenant = await _repo.GetByIdAsync(id);
        if (tenant == null) return null;

        await _repo.UpdateAsync(id, request);
        return await _repo.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tenant = await _repo.GetByIdAsync(id);
        if (tenant == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
