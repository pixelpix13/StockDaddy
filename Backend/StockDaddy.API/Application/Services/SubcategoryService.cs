using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class SubcategoryService
{
    private readonly ISubcategoryRepository _repo;

    public SubcategoryService(ISubcategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<SubcategoryDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<SubcategoryDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<SubcategoryDto?> AddAsync(CreateSubcategoryRequest request)
    {
        await _repo.AddAsync(request);
        // Optionally, fetch the created subcategory (e.g., by unique fields or by returning from repo)
        // For now, return null as placeholder if repo does not return the created subcategory
        return null;
    }

    public async Task<SubcategoryDto?> UpdateAsync(int id, UpdateSubcategoryRequest request)
    {
        var sub = await _repo.GetByIdAsync(id);
        if (sub == null) return null;

        await _repo.UpdateAsync(id, request);
        // Fetch and return the updated subcategory
        return await _repo.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sub = await _repo.GetByIdAsync(id);
        if (sub == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
