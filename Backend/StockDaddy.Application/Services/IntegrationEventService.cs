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
        var events = await _repo.GetAllAsync();
        return events.Select(e => new IntegrationEventDto
        {
            Id = e.Id,
            StoreId = e.StoreId,
            EventType = e.EventType,
            Payload = e.Payload,
            TriggeredBy = e.TriggeredBy,
            TriggeredAt = e.TriggeredAt,
            Delivered = e.Delivered,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        }).ToList();
    }

    public async Task<IntegrationEventDto?> GetByIdAsync(Guid id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return null;

        return new IntegrationEventDto
        {
            Id = e.Id,
            StoreId = e.StoreId,
            EventType = e.EventType,
            Payload = e.Payload,
            TriggeredBy = e.TriggeredBy,
            TriggeredAt = e.TriggeredAt,
            Delivered = e.Delivered,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        };
    }

    public async Task AddAsync(CreateIntegrationEventRequest request)
    {
        var e = new IntegrationEvent
        {
            StoreId = request.StoreId,
            EventType = request.EventType,
            Payload = request.Payload,
            TriggeredBy = request.TriggeredBy,
            TriggeredAt = DateTime.UtcNow,
            Delivered = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(e);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateIntegrationEventRequest request)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return false;

        e.Delivered = request.Delivered;
        e.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(e);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
