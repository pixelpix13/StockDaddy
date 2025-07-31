using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class StoreService
{
    private readonly IStoreRepository _repo;

    public StoreService(IStoreRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<StoreDto>> GetAllAsync()
    {
        var stores = await _repo.GetAllAsync();
        return stores.Select(s => new StoreDto
        {
            Id = s.Id,
            TenantId = s.TenantId,
            Name = s.Name,
            Location = s.Location,
            CreatedAt = s.CreatedAt
        }).ToList();
    }

    public async Task<StoreDto?> GetByIdAsync(Guid id)
    {
        var store = await _repo.GetByIdAsync(id);
        if (store == null) return null;

        return new StoreDto
        {
            Id = store.Id,
            TenantId = store.TenantId,
            Name = store.Name,
            Location = store.Location,
            CreatedAt = store.CreatedAt
        };
    }

    public async Task AddAsync(CreateStoreRequest request)
    {
        var store = new Store
        {
            TenantId = request.TenantId,
            Name = request.Name,
            Location = request.Location,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(store);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateStoreRequest request)
    {
        var store = await _repo.GetByIdAsync(id);
        if (store == null) return false;

        store.Name = request.Name;
        store.Location = request.Location;
        store.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(store);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var store = await _repo.GetByIdAsync(id);
        if (store == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
