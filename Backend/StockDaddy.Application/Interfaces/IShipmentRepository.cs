using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IShipmentRepository
{
    Task<List<Shipment>> GetAllAsync();
    Task<Shipment?> GetByIdAsync(Guid id);
    Task AddAsync(Shipment shipment);
    Task UpdateAsync(Shipment shipment);
    Task DeleteAsync(Guid id);
}
