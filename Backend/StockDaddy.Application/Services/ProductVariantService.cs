using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class ProductVariantService
{
    private readonly IProductVariantRepository _repo;

    public ProductVariantService(IProductVariantRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductVariantDto>> GetAllAsync()
    {
        var variants = await _repo.GetAllAsync();
        return variants.Select(v => new ProductVariantDto
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
        }).ToList();
    }

    public async Task<ProductVariantDto?> GetByIdAsync(Guid id)
    {
        var v = await _repo.GetByIdAsync(id);
        if (v == null) return null;

        return new ProductVariantDto
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
        };
    }

    public async Task AddAsync(CreateProductVariantRequest request)
    {
        var variant = new ProductVariant
        {
            ProductId = request.ProductId,
            StoreId = request.StoreId,
            HSNCodeId = request.HSNCodeId,
            VariantName = request.VariantName,
            Barcode = request.Barcode,
            SkuCode = request.SkuCode,
            CostPrice = request.CostPrice,
            MarginPercent = request.MarginPercent,
            TaxPercent = request.TaxPercent,
            Price = request.Price,
            Quantity = request.Quantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(variant);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductVariantRequest request)
    {
        var variant = await _repo.GetByIdAsync(id);
        if (variant == null) return false;

        variant.VariantName = request.VariantName;
        variant.Barcode = request.Barcode;
        variant.SkuCode = request.SkuCode;
        variant.CostPrice = request.CostPrice;
        variant.MarginPercent = request.MarginPercent;
        variant.TaxPercent = request.TaxPercent;
        variant.Price = request.Price;
        variant.Quantity = request.Quantity;
        variant.HSNCodeId = request.HSNCodeId;
        variant.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(variant);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var variant = await _repo.GetByIdAsync(id);
        if (variant == null) return false;

        variant.IsDeleted = true;
        variant.DeletedAt = DateTime.UtcNow;
        variant.UpdatedAt = DateTime.UtcNow;
        
        await _repo.DeleteAsync(id);
        return true;
    }
}
