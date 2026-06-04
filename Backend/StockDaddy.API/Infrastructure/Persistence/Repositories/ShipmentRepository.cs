using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Domain.Enums;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class ShipmentRepository : IShipmentRepository
{
    private readonly ApplicationDbContext _context;

    public ShipmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShipmentDto>> GetAllAsync()
    {
        return await _context.Shipments
            .Where(s => !s.IsDeleted)
            .Select(s => new ShipmentDto
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
            })
            .ToListAsync();
    }

    public async Task<ShipmentDto?> GetByIdAsync(int id)
    {
        return await _context.Shipments
            .Where(s => s.Id == id && !s.IsDeleted)
            .Select(s => new ShipmentDto
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
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateShipmentRequest shipment)
    {
        var entity = new Shipment
        {
            SaleId = shipment.SaleId,
            StoreId = shipment.StoreId,
            CourierName = shipment.CourierName,
            TrackingNumber = shipment.TrackingNumber,
            ShippedDate = shipment.ShippedDate,
            EstimatedArrival = shipment.EstimatedArrival,
            Status = shipment.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Shipments.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateShipmentRequest shipment)
    {
        var entity = await _context.Shipments.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (entity == null) return;
        entity.CourierName = shipment.CourierName;
        entity.TrackingNumber = shipment.TrackingNumber;
        entity.ShippedDate = shipment.ShippedDate;
        entity.EstimatedArrival = shipment.EstimatedArrival;
        entity.Status = shipment.Status;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Shipments.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Shipments.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Shipments.Update(entity);
        await _context.SaveChangesAsync();
    }
}
