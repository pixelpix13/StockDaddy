using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class RefundService
{
    private readonly IRefundRepository _repo;

    public RefundService(IRefundRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<RefundDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<RefundDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<RefundDto> AddAsync(CreateRefundRequest request)
    {
        await _repo.AddAsync(request);
        var all = await _repo.GetAllAsync();
        return all.OrderByDescending(r => r.Id).First();
    }

    public async Task<RefundDto?> UpdateAsync(int id, UpdateRefundRequest request)
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
