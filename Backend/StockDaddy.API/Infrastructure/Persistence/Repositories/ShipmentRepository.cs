using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Domain.Enums;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class ShipmentRepository : IShipmentRepository
{
    private readonly ApplicationDbContext _context;

    public ShipmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ShipmentDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Shipments.Where(s => !s.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(s => EF.Functions.ILike(s.CourierName, pattern) || EF.Functions.ILike(s.TrackingNumber, pattern) || EF.Functions.ILike(s.Status.ToString(), pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(s => new ShipmentDto
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
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<Shipment> ApplySort(IQueryable<Shipment> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(s => s.CreatedAt),
            ("createdat", false) => query.OrderBy(s => s.CreatedAt),
            (_, true) => query.OrderByDescending(s => s.Id),
            _ => query.OrderBy(s => s.Id),
        };

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
