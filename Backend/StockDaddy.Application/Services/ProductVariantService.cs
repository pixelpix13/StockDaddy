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
        return await _repo.GetAllAsync();
    }

    public async Task<ProductVariantDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<ProductVariantDto> AddAsync(CreateProductVariantRequest request)
    {
        await _repo.AddAsync(request);
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(v => v.Id).First();
    }

    public async Task<ProductVariantDto?> UpdateAsync(int id, UpdateProductVariantRequest request)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;
        await _repo.UpdateAsync(id, request);
        return await _repo.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return false;
        await _repo.DeleteAsync(id);
        return true;
    }

    /// <summary>
    /// Reverts the price of a product variant to the specified value.
    /// </summary>
    public async Task<bool> RevertVariantPriceAsync(int variantId, decimal originalPrice)
    {
        return await _repo.UpdatePriceAsync(variantId, originalPrice);
    }
}
