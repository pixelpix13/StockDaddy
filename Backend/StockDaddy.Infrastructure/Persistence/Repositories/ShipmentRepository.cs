using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class ShipmentRepository : IShipmentRepository
{
    private readonly ApplicationDbContext _context;

    public ShipmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Shipment>> GetAllAsync()
    {
        return await _context.Shipments
            .Where(s => !s.IsDeleted)
            .ToListAsync();
    }

    public async Task<Shipment?> GetByIdAsync(Guid id)
    {
        return await _context.Shipments
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }

    public async Task AddAsync(Shipment shipment)
    {
        await _context.Shipments.AddAsync(shipment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Shipment shipment)
    {

        _context.Shipments.Update(shipment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var shipment = await _context.Shipments.FindAsync(id);
        if (shipment == null) return;


        _context.Shipments.Update(shipment);
        await _context.SaveChangesAsync();
    }
}
