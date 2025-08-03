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
        var payments = await _repo.GetAllAsync();
        return payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            InvoiceId = p.InvoiceId,
            Amount = p.Amount,
            PaymentMethod = p.PaymentMethod,
            PaidAt = p.PaidAt,
            ReceivedBy = p.ReceivedBy,
            ReferenceNo = p.ReferenceNo,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();
    }

    public async Task<PaymentDto?> GetByIdAsync(Guid id)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) return null;

        return new PaymentDto
        {
            Id = p.Id,
            InvoiceId = p.InvoiceId,
            Amount = p.Amount,
            PaymentMethod = p.PaymentMethod,
            PaidAt = p.PaidAt,
            ReceivedBy = p.ReceivedBy,
            ReferenceNo = p.ReferenceNo,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }

    public async Task AddAsync(CreatePaymentRequest request)
    {
        var p = new Payment
        {
            InvoiceId = request.InvoiceId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            PaidAt = request.PaidAt,
            ReceivedBy = request.ReceivedBy,
            ReferenceNo = request.ReferenceNo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(p);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdatePaymentRequest request)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) return false;

        p.Amount = request.Amount;
        p.PaymentMethod = request.PaymentMethod;
        p.PaidAt = request.PaidAt;
        p.ReceivedBy = request.ReceivedBy;
        p.ReferenceNo = request.ReferenceNo;
        p.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(p);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) return false;

        // Soft delete logic
        p.IsDeleted = true;
        p.DeletedAt = DateTime.UtcNow;
        p.UpdatedAt = DateTime.UtcNow;
        
        await _repo.DeleteAsync(id);
        return true;
    }
}
