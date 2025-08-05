using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class IntegrationEventRepository : IIntegrationEventRepository
{
    private readonly ApplicationDbContext _context;

    public IntegrationEventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<IntegrationEventDto>> GetAllAsync()
    {
        return await _context.IntegrationEvents
            .Where(e => !e.IsDeleted)
            .Select(e => new IntegrationEventDto
            {
                Id = e.Id,
                StoreId = e.StoreId,
                EventType = e.EventType,
                Payload = e.Payload,
                TriggeredBy = 0, // Map Guid to int if needed, else adjust DTO
                TriggeredAt = e.TriggeredAt,
                Delivered = e.Delivered,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<IntegrationEventDto?> GetByIdAsync(int id)
    {
        var e = await _context.IntegrationEvents.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        if (e == null) return null;
        return new IntegrationEventDto
        {
            Id = e.Id,
            StoreId = e.StoreId,
            EventType = e.EventType,
            Payload = e.Payload,
            TriggeredBy = 0, // Map Guid to int if needed, else adjust DTO
            TriggeredAt = e.TriggeredAt,
            Delivered = e.Delivered,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        };
    }

    public async Task AddAsync(CreateIntegrationEventRequest integrationEvent)
    {
        var entity = new IntegrationEvent
        {
            StoreId = integrationEvent.StoreId,
            EventType = integrationEvent.EventType,
            Payload = integrationEvent.Payload,
            TriggeredBy = Guid.Empty, // Adjust as needed
            TriggeredAt = DateTime.UtcNow,
            Delivered = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.IntegrationEvents.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateIntegrationEventRequest integrationEvent)
    {
        var entity = await _context.IntegrationEvents.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        if (entity == null) return;

        entity.Delivered = integrationEvent.Delivered;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.IntegrationEvents.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.IntegrationEvents.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.IntegrationEvents.Update(entity);
        await _context.SaveChangesAsync();
    }
}
