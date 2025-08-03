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
        var orders = await _repo.GetAllAsync();
        return orders.Select(o => new PurchaseOrderDto
        {
            Id = o.Id,
            TenantId = o.TenantId,
            SupplierId = o.SupplierId,
            StoreId = o.StoreId,
            OrderDate = o.OrderDate,
            ExpectedDelivery = o.ExpectedDelivery,
            Status = o.Status,
            Notes = o.Notes,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt
        }).ToList();
    }

    public async Task<PurchaseOrderDto?> GetByIdAsync(Guid id)
    {
        var order = await _repo.GetByIdAsync(id);
        if (order == null) return null;

        return new PurchaseOrderDto
        {
            Id = order.Id,
            TenantId = order.TenantId,
            SupplierId = order.SupplierId,
            StoreId = order.StoreId,
            OrderDate = order.OrderDate,
            ExpectedDelivery = order.ExpectedDelivery,
            Status = order.Status,
            Notes = order.Notes,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }

    public async Task AddAsync(CreatePurchaseOrderRequest request)
    {
        var order = new PurchaseOrder
        {
            TenantId = request.TenantId,
            SupplierId = request.SupplierId,
            StoreId = request.StoreId,
            OrderDate = request.OrderDate,
            ExpectedDelivery = request.ExpectedDelivery,
            Status = request.Status,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(order);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdatePurchaseOrderRequest request)
    {
        var order = await _repo.GetByIdAsync(id);
        if (order == null) return false;

        order.ExpectedDelivery = request.ExpectedDelivery;
        order.Status = request.Status;
        order.Notes = request.Notes;
        order.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(order);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var order = await _repo.GetByIdAsync(id);
        if (order == null) return false;

        order.IsDeleted = true;
        order.DeletedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;

        await _repo.DeleteAsync(id);
        return true;
    }
}
