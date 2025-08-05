using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class SupplierService
{
    private readonly ISupplierRepository _repo;

    public SupplierService(ISupplierRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<SupplierDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<SupplierDto?> GetByIdAsync(int id)
    {
        var supplier = await _repo.GetByIdAsync(id);
        if (supplier == null)
            return null;
        return supplier;
    }

    public async Task<SupplierDto?> AddAsync(CreateSupplierRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Supplier name is required.");
        if (string.IsNullOrWhiteSpace(request.ContactName))
            throw new ArgumentException("Contact name is required.");
        if (string.IsNullOrWhiteSpace(request.Phone))
            throw new ArgumentException("Phone is required.");
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required.");
        if (string.IsNullOrWhiteSpace(request.Address))
            throw new ArgumentException("Address is required.");

        await _repo.AddAsync(request);
        // Optionally, fetch the created supplier (e.g., by unique fields or by returning from repo)
        // For now, return null as placeholder if repo does not return the created supplier
        return null;
    }

    public async Task<SupplierDto?> UpdateAsync(int id, UpdateSupplierRequest request)
    {
        var supplier = await _repo.GetByIdAsync(id);
        if (supplier == null)
            return null;

        await _repo.UpdateAsync(id, request);
        // Fetch and return the updated supplier
        return await _repo.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var supplier = await _repo.GetByIdAsync(id);
        if (supplier == null)
            return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
