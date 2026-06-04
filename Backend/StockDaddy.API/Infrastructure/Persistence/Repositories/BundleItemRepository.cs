using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class BundleItemRepository : IBundleItemRepository
{
    private readonly ApplicationDbContext _context;

    public BundleItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BundleItemDto>> GetAllAsync()
    {
        return await _context.BundleItems
            .Where(b => !b.IsDeleted)
            .Select(b => new BundleItemDto
            {
                Id = b.Id,
                BundleId = b.BundleId,
                ProductId = b.ProductId,
                Quantity = b.Quantity,
                EffectiveUnitPrice = b.EffectiveUnitPrice,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<BundleItemDto?> GetByIdAsync(int id)
    {
        var b = await _context.BundleItems.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (b == null) return null;
        return new BundleItemDto
        {
            Id = b.Id,
            BundleId = b.BundleId,
            ProductId = b.ProductId,
            Quantity = b.Quantity,
            EffectiveUnitPrice = b.EffectiveUnitPrice,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        };
    }

    public async Task AddAsync(CreateBundleItemRequest item)
    {
        var entity = new BundleItem
        {
            BundleId = item.BundleId,
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            EffectiveUnitPrice = item.EffectiveUnitPrice,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.BundleItems.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateBundleItemRequest item)
    {
        var entity = await _context.BundleItems.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (entity == null) return;

        entity.Quantity = item.Quantity;
        entity.EffectiveUnitPrice = item.EffectiveUnitPrice;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.BundleItems.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.BundleItems.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.BundleItems.Update(entity);
        await _context.SaveChangesAsync();
    }
}
