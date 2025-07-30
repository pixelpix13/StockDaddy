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

    public async Task<List<SaleItemDto>> GetAllBySaleIdAsync(Guid saleId)
    {
        var items = await _repo.GetAllBySaleIdAsync(saleId);
        return items.Select(i => new SaleItemDto
        {
            Id = i.Id,
            SaleId = i.SaleId,
            ProductVariantId = i.ProductVariantId,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            TotalPrice = i.TotalPrice
        }).ToList();
    }

    public async Task<SaleItemDto?> GetByIdAsync(Guid id)
    {
        var i = await _repo.GetByIdAsync(id);
        return i == null ? null : new SaleItemDto
        {
            Id = i.Id,
            SaleId = i.SaleId,
            ProductVariantId = i.ProductVariantId,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            TotalPrice = i.TotalPrice
        };
    }

    public async Task<Guid> CreateAsync(CreateSaleItemRequest req)
    {
        var item = new SaleItem
        {
            SaleId = req.SaleId,
            ProductVariantId = req.ProductVariantId,
            Quantity = req.Quantity,
            UnitPrice = req.UnitPrice,
            TotalPrice = req.Quantity * req.UnitPrice
        };

        await _repo.AddAsync(item);
        return item.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateSaleItemRequest req)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return false;

        item.Quantity = req.Quantity;
        item.UnitPrice = req.UnitPrice;
        item.TotalPrice = req.Quantity * req.UnitPrice;

        await _repo.UpdateAsync(item);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
