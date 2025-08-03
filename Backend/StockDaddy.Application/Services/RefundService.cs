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
        var refunds = await _repo.GetAllAsync();
        return refunds.Select(r => new RefundDto
        {
            Id = r.Id,
            ReturnId = r.ReturnId,
            StoreId = r.StoreId,
            Amount = r.Amount,
            RefundedAt = r.RefundedAt,
            Method = r.Method,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList();
    }

    public async Task<RefundDto?> GetByIdAsync(Guid id)
    {
        var r = await _repo.GetByIdAsync(id);
        if (r == null) return null;

        return new RefundDto
        {
            Id = r.Id,
            ReturnId = r.ReturnId,
            StoreId = r.StoreId,
            Amount = r.Amount,
            RefundedAt = r.RefundedAt,
            Method = r.Method,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }

    public async Task AddAsync(CreateRefundRequest request)
    {
        var r = new Refund
        {
            ReturnId = request.ReturnId,
            StoreId = request.StoreId,
            Amount = request.Amount,
            RefundedAt = request.RefundedAt,
            Method = request.Method,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(r);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateRefundRequest request)
    {
        var r = await _repo.GetByIdAsync(id);
        if (r == null) return false;

        r.Amount = request.Amount;
        r.RefundedAt = request.RefundedAt;
        r.Method = request.Method;
        r.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(r);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var r = await _repo.GetByIdAsync(id);
        if (r == null) return false;
        
        r.IsDeleted = true;
        r.DeletedAt = DateTime.UtcNow;
        r.UpdatedAt = DateTime.UtcNow;

        await _repo.DeleteAsync(id);
        return true;
    }
}
