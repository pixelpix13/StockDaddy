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
        var items = await _repo.GetAllAsync();
        return items.Select(i => new PurchaseItemDto
        {
            Id = i.Id,
            PurchaseOrderId = i.PurchaseOrderId,
            ProductVariantId = i.ProductVariantId,
            Quantity = i.Quantity,
            UnitCost = i.UnitCost,
            TotalCost = i.TotalCost,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        }).ToList();
    }

    public async Task<PurchaseItemDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return null;

        return new PurchaseItemDto
        {
            Id = item.Id,
            PurchaseOrderId = item.PurchaseOrderId,
            ProductVariantId = item.ProductVariantId,
            Quantity = item.Quantity,
            UnitCost = item.UnitCost,
            TotalCost = item.TotalCost,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }

    public async Task AddAsync(CreatePurchaseItemRequest request)
    {
        var item = new PurchaseItem
        {
            PurchaseOrderId = request.PurchaseOrderId,
            ProductVariantId = request.ProductVariantId,
            Quantity = request.Quantity,
            UnitCost = request.UnitCost,
            TotalCost = request.Quantity * request.UnitCost,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(item);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdatePurchaseItemRequest request)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return false;

        item.Quantity = request.Quantity;
        item.UnitCost = request.UnitCost;
        item.TotalCost = request.Quantity * request.UnitCost;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
