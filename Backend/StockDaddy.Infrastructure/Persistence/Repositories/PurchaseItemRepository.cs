using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class PurchaseItemRepository : IPurchaseItemRepository
{
    private readonly ApplicationDbContext _context;

    public PurchaseItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PurchaseItemDto>> GetAllAsync()
    {
        return await _context.PurchaseItems
            .Where(p => !p.IsDeleted)
            .Select(p => new PurchaseItemDto
            {
                Id = p.Id,
                PurchaseOrderId = p.PurchaseOrderId,
                ProductVariantId = p.ProductVariantId,
                Quantity = p.Quantity,
                UnitCost = p.UnitCost,
                TotalCost = p.TotalCost,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<PurchaseItemDto?> GetByIdAsync(int id)
    {
        return await _context.PurchaseItems
            .Where(p => p.Id == id && !p.IsDeleted)
            .Select(p => new PurchaseItemDto
            {
                Id = p.Id,
                PurchaseOrderId = p.PurchaseOrderId,
                ProductVariantId = p.ProductVariantId,
                Quantity = p.Quantity,
                UnitCost = p.UnitCost,
                TotalCost = p.TotalCost,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreatePurchaseItemRequest item)
    {
        var entity = new PurchaseItem
        {
            PurchaseOrderId = item.PurchaseOrderId,
            ProductVariantId = item.ProductVariantId,
            Quantity = item.Quantity,
            UnitCost = item.UnitCost,
            TotalCost = item.UnitCost * item.Quantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.PurchaseItems.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdatePurchaseItemRequest item)
    {
        var entity = await _context.PurchaseItems.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (entity == null) return;
        entity.Quantity = item.Quantity;
        entity.UnitCost = item.UnitCost;
        entity.TotalCost = item.UnitCost * item.Quantity;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.PurchaseItems.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.PurchaseItems.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.PurchaseItems.Update(entity);
        await _context.SaveChangesAsync();
    }
}
