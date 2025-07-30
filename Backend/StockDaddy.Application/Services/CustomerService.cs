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

    public async Task<List<CustomerDto>> GetAllAsync(Guid tenantId)
    {
        var customers = await _repo.GetAllAsync(tenantId);
        return customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            TenantId = c.TenantId,
            Name = c.Name,
            Phone = c.Phone,
            Email = c.Email,
            Address = c.Address,
            CreatedAt = c.CreatedAt
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
            CreatedAt = c.CreatedAt
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
            Address = request.Address
        };

        await _repo.AddAsync(c);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateCustomerRequest request)
    {
        var customer = await _repo.GetByIdAsync(id);
        if (customer == null) return false;

        customer.Name = request.Name;
        customer.Phone = request.Phone;
        customer.Email = request.Email;
        customer.Address = request.Address;

        await _repo.UpdateAsync(customer);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
