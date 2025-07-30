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

    public async Task<BundleItemDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? null : new BundleItemDto
        {
            Id = item.Id,
            BundleId = item.BundleId,
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            EffectiveUnitPrice = item.EffectiveUnitPrice
        };
    }

    public async Task<List<BundleItemDto>> GetByBundleIdAsync(Guid bundleId)
    {
        var list = await _repo.GetByBundleIdAsync(bundleId);
        return list.Select(item => new BundleItemDto
        {
            Id = item.Id,
            BundleId = item.BundleId,
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            EffectiveUnitPrice = item.EffectiveUnitPrice
        }).ToList();
    }

    public async Task<Guid> CreateAsync(CreateBundleItemRequest req)
    {
        var item = new BundleItem
        {
            BundleId = req.BundleId,
            ProductId = req.ProductId,
            Quantity = req.Quantity,
            EffectiveUnitPrice = req.EffectiveUnitPrice
        };

        await _repo.AddAsync(item);
        return item.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateBundleItemRequest req)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return false;

        if (req.Quantity.HasValue)
            item.Quantity = req.Quantity.Value;

        if (req.EffectiveUnitPrice.HasValue)
            item.EffectiveUnitPrice = req.EffectiveUnitPrice.Value;

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
