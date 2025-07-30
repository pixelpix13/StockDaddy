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

    public async Task<StockItemDto?> GetByProductIdAsync(Guid productId)
    {
        var item = await _repo.GetByProductIdAsync(productId);
        if (item == null) return null;

        return new StockItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            LastUpdated = item.LastUpdated,
            Status = item.Status,
            UpdatedBy = item.UpdatedBy
        };
    }

    public async Task<List<StockItemDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(item => new StockItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            LastUpdated = item.LastUpdated,
            Status = item.Status,
            UpdatedBy = item.UpdatedBy
        }).ToList();
    }
    public async Task<bool> CreateStockItemAsync(CreateStockItemRequest request)
    {
        var existing = await _repo.GetByProductIdAsync(request.ProductId);
        if (existing != null)
            return false; // Already exists

        var item = new StockItem
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            LastUpdated = DateTime.UtcNow,
            Status = CalculateStatus(request.Quantity),
            UpdatedBy = request.UpdatedBy
        };

        await _repo.AddAsync(item);
        return true;
    }

    public async Task<bool> UpdateStockAsync(UpdateStockItemRequest request)
    {
        var existing = await _repo.GetByProductIdAsync(request.ProductId);

        if (existing == null)
        {
            var newItem = new StockItem
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                LastUpdated = DateTime.UtcNow,
                Status = CalculateStatus(request.Quantity),
                UpdatedBy = request.UpdatedBy
            };
            await _repo.AddAsync(newItem);
        }
        else
        {
            existing.Quantity = request.Quantity;
            existing.LastUpdated = DateTime.UtcNow;
            existing.Status = CalculateStatus(request.Quantity);
            existing.UpdatedBy = request.UpdatedBy;

            await _repo.UpdateAsync(existing);
        }

        return true;
    }

    private static string CalculateStatus(int quantity)
    {
        if (quantity == 0) return "out-of-stock";
        else if (quantity <= 5) return "low";
        else return "in-stock";
    }
}
