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
        var tenants = await _repo.GetAllAsync();
        return tenants.Select(t => new TenantDto
        {
            Id = t.Id,
            Name = t.Name,
            CreatedAt = t.CreatedAt
        }).ToList();
    }

    public async Task<TenantDto?> GetByIdAsync(Guid id)
    {
        var tenant = await _repo.GetByIdAsync(id);
        if (tenant == null) return null;

        return new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            CreatedAt = tenant.CreatedAt
        };
    }

    public async Task AddAsync(CreateTenantRequest request)
    {
        var tenant = new Tenant
        {
            Name = request.Name
        };

        await _repo.AddAsync(tenant);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTenantRequest request)
    {
        var tenant = await _repo.GetByIdAsync(id);
        if (tenant == null) return false;

        tenant.Name = request.Name;
        await _repo.UpdateAsync(tenant);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var tenant = await _repo.GetByIdAsync(id);
        if (tenant == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
