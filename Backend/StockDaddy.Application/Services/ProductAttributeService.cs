using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class ProductAttributeService
{
    private readonly IProductAttributeRepository _repo;

    public ProductAttributeService(IProductAttributeRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductAttributeDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<ProductAttributeDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<ProductAttributeDto> AddAsync(CreateProductAttributeRequest request)
    {
        await _repo.AddAsync(request);
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(a => a.Id).First();
    }

    public async Task<ProductAttributeDto?> UpdateAsync(int id, UpdateProductAttributeRequest request)
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
