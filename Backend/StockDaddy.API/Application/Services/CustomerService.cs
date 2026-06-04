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
        return await _repo.GetAllAsync();
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<CustomerDto> AddAsync(CreateCustomerRequest request)
    {
        await _repo.AddAsync(request);
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(c => c.Id).First();
    }

    public async Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerRequest request)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;
        await _repo.UpdateAsync(id, request);
        return await _repo.GetByIdAsync(id);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return false;
        await _repo.SoftDeleteAsync(id);
        return true;
    }
}
