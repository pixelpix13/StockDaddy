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
        var suppliers = await _repo.GetAllAsync();
        return suppliers.Select(s => new SupplierDto
        {
            Id = s.Id,
            TenantId = s.TenantId,
            Name = s.Name,
            ContactName = s.ContactName,
            Phone = s.Phone,
            Email = s.Email,
            Address = s.Address,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
            IsDeleted = s.IsDeleted,
            DeletedAt = s.DeletedAt
        }).ToList();
    }

    public async Task<SupplierDto?> GetByIdAsync(Guid id)
    {
        var supplier = await _repo.GetByIdAsync(id);
        if (supplier == null) return null;

        return new SupplierDto
        {
            Id = supplier.Id,
            TenantId = supplier.TenantId,
            Name = supplier.Name,
            ContactName = supplier.ContactName,
            Phone = supplier.Phone,
            Email = supplier.Email,
            Address = supplier.Address,
            CreatedAt = supplier.CreatedAt,
            UpdatedAt = supplier.UpdatedAt,
            IsDeleted = supplier.IsDeleted,
            DeletedAt = supplier.DeletedAt
        };
    }

    public async Task AddAsync(CreateSupplierRequest request)
    {
        var supplier = new Supplier
        {
            TenantId = request.TenantId,
            Name = request.Name,
            ContactName = request.ContactName,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(supplier);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateSupplierRequest request)
    {
        var supplier = await _repo.GetByIdAsync(id);
        if (supplier == null) return false;

        supplier.Name = request.Name;
        supplier.ContactName = request.ContactName;
        supplier.Phone = request.Phone;
        supplier.Email = request.Email;
        supplier.Address = request.Address;
        supplier.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(supplier);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var supplier = await _repo.GetByIdAsync(id);
        if (supplier == null) return false;

        supplier.IsDeleted = true;
        supplier.DeletedAt = DateTime.UtcNow;
        supplier.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(supplier);
        return true;
    }
}
