using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class SaleService
{
    private readonly ISaleRepository _repo;

    public SaleService(ISaleRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<SaleDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<SaleDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<SaleDto?> AddAsync(CreateSaleRequest request)
    {
        await _repo.AddAsync(request);
        // Optionally, fetch the created sale (e.g., by unique fields or by returning from repo)
        // For now, return null as placeholder if repo does not return the created sale
        return null;
    }

    public async Task<SaleDto?> UpdateAsync(int id, UpdateSaleRequest request)
    {
        var sale = await _repo.GetByIdAsync(id);
        if (sale == null) return null;

        await _repo.UpdateAsync(id, request);
        // Fetch and return the updated sale
        return await _repo.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sale = await _repo.GetByIdAsync(id);
        if (sale == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
