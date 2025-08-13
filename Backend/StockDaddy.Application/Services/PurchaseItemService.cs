using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class PurchaseItemService
{
    private readonly IPurchaseItemRepository _repo;

    public PurchaseItemService(IPurchaseItemRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<PurchaseItemDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<PurchaseItemDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<PurchaseItemDto> AddAsync(CreatePurchaseItemRequest request)
    {
        await _repo.AddAsync(request);
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(i => i.Id).First();
    }

    public async Task<PurchaseItemDto?> UpdateAsync(int id, UpdatePurchaseItemRequest request)
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
