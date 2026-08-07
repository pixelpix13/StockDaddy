using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class ProductVariantRepository : IProductVariantRepository
{
    private readonly ApplicationDbContext _context;

    public ProductVariantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductVariantDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.ProductVariants.Where(v => !v.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(v => EF.Functions.ILike(v.VariantName, pattern) || EF.Functions.ILike(v.SkuCode, pattern) || EF.Functions.ILike(v.Barcode, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(v => new ProductVariantDto
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
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<ProductVariant> ApplySort(IQueryable<ProductVariant> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("name", true) => query.OrderByDescending(v => v.VariantName),
            ("name", false) => query.OrderBy(v => v.VariantName),
            ("createdat", true) => query.OrderByDescending(v => v.CreatedAt),
            ("createdat", false) => query.OrderBy(v => v.CreatedAt),
            (_, true) => query.OrderByDescending(v => v.Id),
            _ => query.OrderBy(v => v.Id),
        };

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

    public async Task<int> AddAsync(CreateProductVariantRequest variant)
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
        return entity.Id;
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
    public async Task<bool> UpdatePriceAsync(int id, decimal newPrice)
    {
        var variant = await _context.ProductVariants.FindAsync(id);
        if (variant == null) return false;

        variant.Price = newPrice;
        await _context.SaveChangesAsync();
        return true;
    }

}
