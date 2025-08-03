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
        var items = await _repo.GetAllAsync();
        return items.Select(i => new StockItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            StoreId = i.StoreId,
            Quantity = i.Quantity,
            Status = i.Status,
            LastUpdated = i.LastUpdated,
            UpdatedBy = i.UpdatedBy,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        }).ToList();
    }

    public async Task<StockItemDto?> GetByIdAsync(Guid id)
    {
        var i = await _repo.GetByIdAsync(id);
        if (i == null) return null;

        return new StockItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            StoreId = i.StoreId,
            Quantity = i.Quantity,
            Status = i.Status,
            LastUpdated = i.LastUpdated,
            UpdatedBy = i.UpdatedBy,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        };
    }

    public async Task AddAsync(CreateStockItemRequest request)
    {
        var item = new StockItem
        {
            ProductId = request.ProductId,
            StoreId = request.StoreId,
            Quantity = request.Quantity,
            Status = request.Status,
            LastUpdated = DateTime.UtcNow,
            UpdatedBy = request.UpdatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(item);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateStockItemRequest request)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return false;

        item.Quantity = request.Quantity;
        item.Status = request.Status;
        item.LastUpdated = DateTime.UtcNow;
        item.UpdatedBy = request.UpdatedBy;
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
