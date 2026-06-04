using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class BundleSaleItemRepository : IBundleSaleItemRepository
{
    private readonly ApplicationDbContext _context;

    public BundleSaleItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BundleSaleItemDto>> GetAllAsync()
    {
        return await _context.BundleSaleItems
            .Where(i => !i.IsDeleted)
            .Select(i => new BundleSaleItemDto
            {
                Id = i.Id,
                SaleId = i.SaleId,
                BundleId = i.BundleId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<BundleSaleItemDto?> GetByIdAsync(int id)
    {
        var i = await _context.BundleSaleItems.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (i == null) return null;
        return new BundleSaleItemDto
        {
            Id = i.Id,
            SaleId = i.SaleId,
            BundleId = i.BundleId,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            TotalPrice = i.TotalPrice,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        };
    }

    public async Task AddAsync(CreateBundleSaleItemRequest item)
    {
        var entity = new BundleSaleItem
        {
            SaleId = item.SaleId,
            BundleId = item.BundleId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.BundleSaleItems.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateBundleSaleItemRequest item)
    {
        var entity = await _context.BundleSaleItems.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (entity == null) return;

        entity.Quantity = item.Quantity;
        entity.UnitPrice = item.UnitPrice;
        entity.TotalPrice = item.TotalPrice;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.BundleSaleItems.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.BundleSaleItems.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.BundleSaleItems.Update(entity);
        await _context.SaveChangesAsync();
    }
}
