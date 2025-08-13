using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class ShipmentService
{
    private readonly IShipmentRepository _repo;

    public ShipmentService(IShipmentRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ShipmentDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<ShipmentDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<ShipmentDto?> AddAsync(CreateShipmentRequest request)
    {
        await _repo.AddAsync(request);
        // Optionally, fetch the created shipment (e.g., by unique fields or by returning from repo)
        // For now, return null as placeholder if repo does not return the created shipment
        return null;
    }

    public async Task<ShipmentDto?> UpdateAsync(int id, UpdateShipmentRequest request)
    {
        var s = await _repo.GetByIdAsync(id);
        if (s == null) return null;

        await _repo.UpdateAsync(id, request);
        // Fetch and return the updated shipment
        return await _repo.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var s = await _repo.GetByIdAsync(id);
        if (s == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
