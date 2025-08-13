using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class SaleItemService
{
    private readonly ISaleItemRepository _repo;

    public SaleItemService(ISaleItemRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<SaleItemDto>> GetAllBySaleIdAsync(int saleId)
    {
        return await _repo.GetAllBySaleIdAsync(saleId);
    }

    public async Task<SaleItemDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<SaleItemDto?> CreateAsync(CreateSaleItemRequest req)
    {
        await _repo.AddAsync(req);
        // Optionally, fetch the created sale item (e.g., by unique fields or by returning from repo)
        // For now, return null as placeholder if repo does not return the created sale item
        return null;
    }

    public async Task<SaleItemDto?> UpdateAsync(int id, UpdateSaleItemRequest req)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return null;

        await _repo.UpdateAsync(id, req);
        // Fetch and return the updated sale item
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
