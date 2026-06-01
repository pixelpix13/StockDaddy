using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;

namespace StockDaddy.Tests.Fakes;

public class FakeTenantRepository : ITenantRepository
{
    private readonly List<TenantDto> _tenants = new();

    public Task<List<TenantDto>> GetAllAsync()
        => Task.FromResult(_tenants);

    public Task<TenantDto?> GetByIdAsync(int id)
    {
        var tenant = _tenants.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(tenant);
    }

    public Task AddAsync(CreateTenantRequest request)
    {
        _tenants.Add(new TenantDto
        {
            Id = _tenants.Count + 1,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        return Task.CompletedTask;
    }

    public Task UpdateAsync(int id, UpdateTenantRequest request)
    {
        var tenant = _tenants.FirstOrDefault(t => t.Id == id);
        if (tenant != null) tenant.Name = request.Name;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var tenant = _tenants.FirstOrDefault(t => t.Id == id);
        if (tenant != null) _tenants.Remove(tenant);
        return Task.CompletedTask;
    }

    // Helper: lets tests seed data before the test runs
    public void Seed(TenantDto tenant) => _tenants.Add(tenant);
}