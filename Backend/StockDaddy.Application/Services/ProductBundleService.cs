using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class ProductBundleService
{
    private readonly IProductBundleRepository _repo;

    public ProductBundleService(IProductBundleRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductBundleDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<ProductBundleDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<ProductBundleDto> AddAsync(CreateProductBundleRequest request)
    {
        await _repo.AddAsync(request);
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(b => b.Id).First();
    }

    public async Task<ProductBundleDto?> UpdateAsync(int id, UpdateProductBundleRequest request)
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
}
