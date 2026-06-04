using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class SaleItemRepository : ISaleItemRepository
{
    private readonly ApplicationDbContext _context;

    public SaleItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SaleItemDto>> GetAllBySaleIdAsync(int saleId)
    {
        return await _context.SaleItems
            .Where(i => i.SaleId == saleId && !i.IsDeleted)
            .Select(i => new SaleItemDto
            {
                Id = i.Id,
                SaleId = i.SaleId,
                ProductVariantId = i.ProductVariantId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            })
            .ToListAsync();
    }

    public async Task<SaleItemDto?> GetByIdAsync(int id)
    {
        return await _context.SaleItems
            .Where(i => i.Id == id && !i.IsDeleted)
            .Select(i => new SaleItemDto
            {
                Id = i.Id,
                SaleId = i.SaleId,
                ProductVariantId = i.ProductVariantId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateSaleItemRequest item)
    {
        var entity = new SaleItem
        {
            SaleId = item.SaleId,
            ProductVariantId = item.ProductVariantId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.UnitPrice * item.Quantity,
            IsDeleted = false
        };
        await _context.SaleItems.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateSaleItemRequest item)
    {
        var entity = await _context.SaleItems.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (entity == null) return;
        entity.Quantity = item.Quantity;
        entity.UnitPrice = item.UnitPrice;
        entity.TotalPrice = item.UnitPrice * item.Quantity;
        _context.SaleItems.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.SaleItems.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        _context.SaleItems.Update(entity);
        await _context.SaveChangesAsync();
    }
}
