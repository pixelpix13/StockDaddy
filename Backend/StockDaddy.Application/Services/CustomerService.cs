using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class CustomerService
{
    private readonly ICustomerRepository _repo;

    public CustomerService(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CustomerDto>> GetAllAsync()
    {
        var customers = await _repo.GetAllAsync();
        return customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            TenantId = c.TenantId,
            Name = c.Name,
            Phone = c.Phone,
            Email = c.Email,
            Address = c.Address,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            IsDeleted = c.IsDeleted,
            DeletedAt = c.DeletedAt
        }).ToList();
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c == null) return null;

        return new CustomerDto
        {
            Id = c.Id,
            TenantId = c.TenantId,
            Name = c.Name,
            Phone = c.Phone,
            Email = c.Email,
            Address = c.Address,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            IsDeleted = c.IsDeleted,
            DeletedAt = c.DeletedAt
        };
    }

    public async Task AddAsync(CreateCustomerRequest request)
    {
        var c = new Customer
        {
            TenantId = request.TenantId,
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(c);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateCustomerRequest request)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c == null) return false;

        c.Name = request.Name;
        c.Phone = request.Phone;
        c.Email = request.Email;
        c.Address = request.Address;
        c.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(c);
        return true;
    }

    public async Task<bool> SoftDeleteAsync(Guid id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c == null) return false;

        await _repo.SoftDeleteAsync(id);
        return true;
    }
}
