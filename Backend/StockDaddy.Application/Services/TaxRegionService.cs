using System.Globalization;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class TaxRegionService
{
    private readonly ITaxRegionRepository _repo;

    public TaxRegionService(ITaxRegionRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TaxRegionDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<TaxRegionDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<TaxRegionDto?> AddAsync(CreateTaxRegionRequest request)
    {
        await _repo.AddAsync(request);
        // Optionally, fetch the created region (e.g., by unique fields or by returning from repo)
        // For now, return null as placeholder if repo does not return the created region
        return null;
    }

    public async Task<TaxRegionDto?> UpdateAsync(int id, UpdateTaxRegionRequest request)
    {
        var region = await _repo.GetByIdAsync(id);
        if (region == null) return null;

        await _repo.UpdateAsync(id, request);
        // Fetch and return the updated region
        return await _repo.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var region = await _repo.GetByIdAsync(id);
        if (region == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
