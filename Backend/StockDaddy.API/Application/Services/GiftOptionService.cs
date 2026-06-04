using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class GiftOptionService
{
    private readonly IGiftOptionRepository _repo;

    public GiftOptionService(IGiftOptionRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<GiftOptionDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<GiftOptionDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<GiftOptionDto> AddAsync(CreateGiftOptionRequest request)
    {
        await _repo.AddAsync(request);
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(o => o.Id).First();
    }

    public async Task<GiftOptionDto?> UpdateAsync(int id, UpdateGiftOptionRequest request)
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
