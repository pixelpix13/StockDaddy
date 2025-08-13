using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IShipmentRepository
{
    Task<List<ShipmentDto>> GetAllAsync();
    Task<ShipmentDto?> GetByIdAsync(int id);
    Task AddAsync(CreateShipmentRequest shipment);
    Task UpdateAsync(int id, UpdateShipmentRequest shipment);
    Task DeleteAsync(int id);
}
