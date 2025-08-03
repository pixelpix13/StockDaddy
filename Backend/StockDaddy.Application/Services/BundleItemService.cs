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
        var items = await _repo.GetAllAsync();
        return items.Select(i => new BundleItemDto
        {
            Id = i.Id,
            BundleId = i.BundleId,
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            EffectiveUnitPrice = i.EffectiveUnitPrice,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        }).ToList();
    }

    public async Task<BundleItemDto?> GetByIdAsync(Guid id)
    {
        var i = await _repo.GetByIdAsync(id);
        if (i == null) return null;

        return new BundleItemDto
        {
            Id = i.Id,
            BundleId = i.BundleId,
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            EffectiveUnitPrice = i.EffectiveUnitPrice,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        };
    }

    public async Task AddAsync(CreateBundleItemRequest request)
    {
        var item = new BundleItem
        {
            BundleId = request.BundleId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            EffectiveUnitPrice = request.EffectiveUnitPrice,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(item);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateBundleItemRequest request)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return false;

        item.Quantity = request.Quantity;
        item.EffectiveUnitPrice = request.EffectiveUnitPrice;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(item);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return false;

        // Soft delete logic
        item.IsDeleted = true;
        item.DeletedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.DeleteAsync(id);
        return true;
    }
}
