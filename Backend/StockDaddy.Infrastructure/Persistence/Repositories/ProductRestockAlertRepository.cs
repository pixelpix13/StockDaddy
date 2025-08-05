using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class ProductRestockAlertRepository : IProductRestockAlertRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRestockAlertRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductRestockAlertDto>> GetAllAsync()
    {
        return await _context.ProductRestockAlerts
            .Where(a => !a.IsDeleted)
            .Select(a => new ProductRestockAlertDto
            {
                Id = a.Id,
                ProductId = a.ProductId,
                StoreId = a.StoreId,
                VariantId = a.VariantId,
                TriggeredAt = a.TriggeredAt,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<ProductRestockAlertDto?> GetByIdAsync(int id)
    {
        return await _context.ProductRestockAlerts
            .Where(a => a.Id == id && !a.IsDeleted)
            .Select(a => new ProductRestockAlertDto
            {
                Id = a.Id,
                ProductId = a.ProductId,
                StoreId = a.StoreId,
                VariantId = a.VariantId,
                TriggeredAt = a.TriggeredAt,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateProductRestockAlertRequest alert)
    {
        var entity = new ProductRestockAlert
        {
            ProductId = alert.ProductId,
            StoreId = alert.StoreId,
            VariantId = alert.VariantId,
            Status = alert.Status,
            TriggeredAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.ProductRestockAlerts.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateProductRestockAlertRequest alert)
    {
        var entity = await _context.ProductRestockAlerts.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (entity == null) return;
        entity.Status = alert.Status;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.ProductRestockAlerts.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ProductRestockAlerts.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.ProductRestockAlerts.Update(entity);
        await _context.SaveChangesAsync();
    }

}
