using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class PaymentService
{
    private readonly IPaymentRepository _repo;

    public PaymentService(IPaymentRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<PaymentDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<PaymentDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<PaymentDto> AddAsync(CreatePaymentRequest request)
    {
        await _repo.AddAsync(request);
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(p => p.Id).First();
    }

    public async Task<PaymentDto?> UpdateAsync(int id, UpdatePaymentRequest request)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;
        await _repo.UpdateAsync(id, request);
        return await _repo.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return false;
        await _repo.DeleteAsync(id);
        return true;
    }
}
