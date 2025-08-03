using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class ShipmentService
{
    private readonly IShipmentRepository _repo;

    public ShipmentService(IShipmentRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ShipmentDto>> GetAllAsync()
    {
        var shipments = await _repo.GetAllAsync();
        return shipments.Select(s => new ShipmentDto
        {
            Id = s.Id,
            SaleId = s.SaleId,
            StoreId = s.StoreId,
            CourierName = s.CourierName,
            TrackingNumber = s.TrackingNumber,
            ShippedDate = s.ShippedDate,
            EstimatedArrival = s.EstimatedArrival,
            Status = s.Status,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        }).ToList();
    }

    public async Task<ShipmentDto?> GetByIdAsync(Guid id)
    {
        var s = await _repo.GetByIdAsync(id);
        if (s == null) return null;

        return new ShipmentDto
        {
            Id = s.Id,
            SaleId = s.SaleId,
            StoreId = s.StoreId,
            CourierName = s.CourierName,
            TrackingNumber = s.TrackingNumber,
            ShippedDate = s.ShippedDate,
            EstimatedArrival = s.EstimatedArrival,
            Status = s.Status,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        };
    }

    public async Task AddAsync(CreateShipmentRequest request)
    {
        var s = new Shipment
        {
            SaleId = request.SaleId,
            StoreId = request.StoreId,
            CourierName = request.CourierName,
            TrackingNumber = request.TrackingNumber,
            ShippedDate = request.ShippedDate,
            EstimatedArrival = request.EstimatedArrival,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(s);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateShipmentRequest request)
    {
        var s = await _repo.GetByIdAsync(id);
        if (s == null) return false;

        s.CourierName = request.CourierName;
        s.TrackingNumber = request.TrackingNumber;
        s.ShippedDate = request.ShippedDate;
        s.EstimatedArrival = request.EstimatedArrival;
        s.Status = request.Status;
        s.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(s);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var s = await _repo.GetByIdAsync(id);
        if (s == null) return false;

        // Soft delete logic
        s.IsDeleted = true;
        s.DeletedAt = DateTime.UtcNow;
        s.UpdatedAt = DateTime.UtcNow;

        await _repo.DeleteAsync(id);
        return true;
    }
}
