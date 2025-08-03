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

    public async Task<List<BundleSaleItemDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(i => new BundleSaleItemDto
        {
            Id = i.Id,
            SaleId = i.SaleId,
            BundleId = i.BundleId,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            TotalPrice = i.TotalPrice,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        }).ToList();
    }

    public async Task<BundleSaleItemDto?> GetByIdAsync(Guid id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return null;

        return new BundleSaleItemDto
        {
            Id = item.Id,
            SaleId = item.SaleId,
            BundleId = item.BundleId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }

    public async Task AddAsync(CreateBundleSaleItemRequest request)
    {
        var item = new BundleSaleItem
        {
            SaleId = request.SaleId,
            BundleId = request.BundleId,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            TotalPrice = request.TotalPrice,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(item);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateBundleSaleItemRequest request)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return false;

        item.Quantity = request.Quantity;
        item.UnitPrice = request.UnitPrice;
        item.TotalPrice = request.TotalPrice;
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
