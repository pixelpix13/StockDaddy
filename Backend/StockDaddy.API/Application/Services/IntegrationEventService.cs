using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class IntegrationEventService
{
    private readonly IIntegrationEventRepository _repo;

    public IntegrationEventService(IIntegrationEventRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<IntegrationEventDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<IntegrationEventDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<IntegrationEventDto> AddAsync(CreateIntegrationEventRequest request)
    {
        await _repo.AddAsync(request);
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(e => e.Id).First();
    }

    public async Task<IntegrationEventDto?> UpdateAsync(int id, UpdateIntegrationEventRequest request)
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
