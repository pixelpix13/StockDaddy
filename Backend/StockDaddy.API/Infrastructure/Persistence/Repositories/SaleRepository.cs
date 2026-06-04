using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Domain.Enums;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly ApplicationDbContext _context;

    public SaleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SaleDto>> GetAllAsync()
    {
        return await _context.Sales
            .Where(s => !s.IsDeleted)
            .Select(s => new SaleDto
            {
                Id = s.Id,
                TenantId = s.TenantId,
                StoreId = s.StoreId,
                CustomerId = s.CustomerId,
                SoldBy = s.SoldBy,
                TotalAmount = s.TotalAmount,
                PaymentMethod = s.PaymentMethod,
                Notes = s.Notes,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<SaleDto?> GetByIdAsync(int id)
    {
        return await _context.Sales
            .Where(s => s.Id == id && !s.IsDeleted)
            .Select(s => new SaleDto
            {
                Id = s.Id,
                TenantId = s.TenantId,
                StoreId = s.StoreId,
                CustomerId = s.CustomerId,
                SoldBy = s.SoldBy,
                TotalAmount = s.TotalAmount,
                PaymentMethod = s.PaymentMethod,
                Notes = s.Notes,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateSaleRequest sale)
    {
        var entity = new Sale
        {
            TenantId = sale.TenantId,
            StoreId = sale.StoreId,
            CustomerId = sale.CustomerId,
            SoldBy = sale.SoldBy,
            TotalAmount = sale.TotalAmount,
            PaymentMethod = sale.PaymentMethod,
            Notes = sale.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Sales.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateSaleRequest sale)
    {
        var entity = await _context.Sales.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (entity == null) return;
        entity.TotalAmount = sale.TotalAmount;
        entity.PaymentMethod = sale.PaymentMethod;
        entity.Notes = sale.Notes;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Sales.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Sales.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Sales.Update(entity);
        await _context.SaveChangesAsync();
    }
}
