using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class BundleSaleItemService
{
    private readonly IBundleSaleItemRepository _repo;

    public BundleSaleItemService(IBundleSaleItemRepository repo)
    {
        _repo = repo;
    }

    public async Task<BundleSaleItemDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? null : new BundleSaleItemDto
        {
            Id = item.Id,
            SaleId = item.SaleId,
            BundleId = item.BundleId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice
        };
    }

    public async Task<List<BundleSaleItemDto>> GetBySaleIdAsync(Guid saleId)
    {
        var items = await _repo.GetBySaleIdAsync(saleId);
        return items.Select(item => new BundleSaleItemDto
        {
            Id = item.Id,
            SaleId = item.SaleId,
            BundleId = item.BundleId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice
        }).ToList();
    }

    public async Task<Guid> CreateAsync(CreateBundleSaleItemRequest req)
    {
        var item = new BundleSaleItem
        {
            SaleId = req.SaleId,
            BundleId = req.BundleId,
            Quantity = req.Quantity,
            UnitPrice = req.UnitPrice,
            TotalPrice = req.Quantity * req.UnitPrice
        };

        await _repo.AddAsync(item);
        return item.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateBundleSaleItemRequest req)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return false;

        if (req.Quantity.HasValue)
            item.Quantity = req.Quantity.Value;

        if (req.UnitPrice.HasValue)
            item.UnitPrice = req.UnitPrice.Value;

        item.TotalPrice = item.Quantity * item.UnitPrice;
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
