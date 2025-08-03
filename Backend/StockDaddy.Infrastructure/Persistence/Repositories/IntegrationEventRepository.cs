using Microsoft.EntityFrameworkCore;
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

    public async Task<List<IntegrationEvent>> GetAllAsync()
    {
        return await _context.IntegrationEvents
            .Where(e => !e.IsDeleted)
            .ToListAsync();
    }

    public async Task<IntegrationEvent?> GetByIdAsync(Guid id)
    {
        return await _context.IntegrationEvents
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
    }

    public async Task AddAsync(IntegrationEvent integrationEvent)
    {
        await _context.IntegrationEvents.AddAsync(integrationEvent);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(IntegrationEvent integrationEvent)
    {
        _context.IntegrationEvents.Update(integrationEvent);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var e = await _context.IntegrationEvents.FindAsync(id);
        if (e == null) return;

        _context.IntegrationEvents.Update(e); // Already marked as deleted in service
        await _context.SaveChangesAsync();
    }
}
