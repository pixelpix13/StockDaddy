using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class ProductTagService
{
    private readonly IProductTagRepository _repo;

    public ProductTagService(IProductTagRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductTagDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<ProductTagDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<ProductTagDto> AddAsync(CreateProductTagRequest request)
    {
        await _repo.AddAsync(request);
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(t => t.Id).First();
    }

    public async Task<ProductTagDto?> UpdateAsync(int id, UpdateProductTagRequest request)
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
