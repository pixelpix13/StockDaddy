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
        return await _repo.GetAllAsync();
    }

    public async Task<StoreDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<StoreDto?> AddAsync(CreateStoreRequest request)
    {
        await _repo.AddAsync(request);
        // Optionally, fetch the created store (e.g., by unique fields or by returning from repo)
        // For now, return null as placeholder if repo does not return the created store
        return null;
    }

    public async Task<StoreDto?> UpdateAsync(int id, UpdateStoreRequest request)
    {
        var store = await _repo.GetByIdAsync(id);
        if (store == null) return null;

        await _repo.UpdateAsync(id, request);
        // Fetch and return the updated store
        return await _repo.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var store = await _repo.GetByIdAsync(id);
        if (store == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
