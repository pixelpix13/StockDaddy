using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class StockItemService
{
    private readonly IStockItemRepository _repo;

    public StockItemService(IStockItemRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<StockItemDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<StockItemDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<StockItemDto?> AddAsync(CreateStockItemRequest request)
    {
        await _repo.AddAsync(request);
        // Optionally, fetch the created stock item (e.g., by unique fields or by returning from repo)
        // For now, return null as placeholder if repo does not return the created stock item
        return null;
    }

    public async Task<StockItemDto?> UpdateAsync(int id, UpdateStockItemRequest request)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return null;

        await _repo.UpdateAsync(id, request);
        // Fetch and return the updated stock item
        return await _repo.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
