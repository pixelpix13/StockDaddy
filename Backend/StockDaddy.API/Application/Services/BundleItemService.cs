using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class BundleItemService
{
    private readonly IBundleItemRepository _repo;

    public BundleItemService(IBundleItemRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<BundleItemDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<BundleItemDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<BundleItemDto> AddAsync(CreateBundleItemRequest request)
    {
        await _repo.AddAsync(request);
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(i => i.Id).First();
    }

    public async Task<BundleItemDto?> UpdateAsync(int id, UpdateBundleItemRequest request)
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
