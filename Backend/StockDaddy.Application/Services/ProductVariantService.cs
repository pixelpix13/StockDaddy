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

    public async Task<List<ProductVariantDto>> GetAllByProductIdAsync(Guid productId)
    {
        var variants = await _repo.GetAllByProductIdAsync(productId);
        return variants.Select(v => new ProductVariantDto
        {
            Id = v.Id,
            ProductId = v.ProductId,
            VariantName = v.VariantName,
            SkuCode = v.SkuCode,
            CostPrice = v.CostPrice,
            MarginPercent = v.MarginPercent,
            TaxPercent = v.TaxPercent,
            Price = v.Price,
            Quantity = v.Quantity
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
            VariantName = v.VariantName,
            SkuCode = v.SkuCode,
            CostPrice = v.CostPrice,
            MarginPercent = v.MarginPercent,
            TaxPercent = v.TaxPercent,
            Price = v.Price,
            Quantity = v.Quantity
        };
    }

    public async Task AddAsync(CreateProductVariantRequest request)
    {
        var price = CalculateFinalPrice(request.CostPrice, request.MarginPercent, request.TaxPercent);

        var entity = new ProductVariant
        {
            ProductId = request.ProductId,
            VariantName = request.VariantName,
            SkuCode = request.SkuCode,
            CostPrice = request.CostPrice,
            MarginPercent = request.MarginPercent,
            TaxPercent = request.TaxPercent,
            Price = price,
            Quantity = request.Quantity
        };

        await _repo.AddAsync(entity);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductVariantRequest request)
    {
        var variant = await _repo.GetByIdAsync(id);
        if (variant == null) return false;

        variant.VariantName = request.VariantName;
        variant.SkuCode = request.SkuCode;
        variant.CostPrice = request.CostPrice;
        variant.MarginPercent = request.MarginPercent;
        variant.TaxPercent = request.TaxPercent;
        variant.Quantity = request.Quantity;
        variant.Price = CalculateFinalPrice(request.CostPrice, request.MarginPercent, request.TaxPercent);

        await _repo.UpdateAsync(variant);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var variant = await _repo.GetByIdAsync(id);
        if (variant == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }

    private static decimal CalculateFinalPrice(decimal cost, decimal margin, decimal tax)
    {
        var marginAmt = cost * margin / 100;
        var taxAmt = (cost + marginAmt) * tax / 100;
        return cost + marginAmt + taxAmt;
    }
}
