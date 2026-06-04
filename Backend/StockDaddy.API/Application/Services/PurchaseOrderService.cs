using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class PurchaseOrderService
{
    private readonly IPurchaseOrderRepository _repo;

    public PurchaseOrderService(IPurchaseOrderRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<PurchaseOrderDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<PurchaseOrderDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<PurchaseOrderDto> AddAsync(CreatePurchaseOrderRequest request)
    {
        await _repo.AddAsync(request);
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(o => o.Id).First();
    }

    public async Task<PurchaseOrderDto?> UpdateAsync(int id, UpdatePurchaseOrderRequest request)
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
