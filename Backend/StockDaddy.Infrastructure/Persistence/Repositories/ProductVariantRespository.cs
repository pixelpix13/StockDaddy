using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class ProductVariantRepository : IProductVariantRepository
{
    private readonly ApplicationDbContext _context;

    public ProductVariantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductVariantDto>> GetAllAsync()
    {
        return await _context.ProductVariants
            .Where(v => !v.IsDeleted)
            .Select(v => new ProductVariantDto
            {
                Id = v.Id,
                ProductId = v.ProductId,
                StoreId = v.StoreId,
                HSNCodeId = v.HSNCodeId,
                VariantName = v.VariantName,
                Barcode = v.Barcode,
                SkuCode = v.SkuCode,
                CostPrice = v.CostPrice,
                MarginPercent = v.MarginPercent,
                TaxPercent = v.TaxPercent,
                Price = v.Price,
                Quantity = v.Quantity,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<ProductVariantDto?> GetByIdAsync(int id)
    {
        return await _context.ProductVariants
            .Where(v => v.Id == id && !v.IsDeleted)
            .Select(v => new ProductVariantDto
            {
                Id = v.Id,
                ProductId = v.ProductId,
                StoreId = v.StoreId,
                HSNCodeId = v.HSNCodeId,
                VariantName = v.VariantName,
                Barcode = v.Barcode,
                SkuCode = v.SkuCode,
                CostPrice = v.CostPrice,
                MarginPercent = v.MarginPercent,
                TaxPercent = v.TaxPercent,
                Price = v.Price,
                Quantity = v.Quantity,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateProductVariantRequest variant)
    {
        var entity = new ProductVariant
        {
            ProductId = variant.ProductId,
            StoreId = variant.StoreId,
            HSNCodeId = variant.HSNCodeId,
            VariantName = variant.VariantName,
            Barcode = variant.Barcode,
            SkuCode = variant.SkuCode,
            CostPrice = variant.CostPrice,
            MarginPercent = variant.MarginPercent,
            TaxPercent = variant.TaxPercent,
            Price = variant.Price,
            Quantity = variant.Quantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.ProductVariants.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateProductVariantRequest variant)
    {
        var entity = await _context.ProductVariants.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
        if (entity == null) return;
        entity.VariantName = variant.VariantName;
        entity.Barcode = variant.Barcode;
        entity.SkuCode = variant.SkuCode;
        entity.CostPrice = variant.CostPrice;
        entity.MarginPercent = variant.MarginPercent;
        entity.TaxPercent = variant.TaxPercent;
        entity.Price = variant.Price;
        entity.Quantity = variant.Quantity;
        entity.HSNCodeId = variant.HSNCodeId;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.ProductVariants.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ProductVariants.FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.ProductVariants.Update(entity);
        await _context.SaveChangesAsync();
    }
}
