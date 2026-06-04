using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentDto>> GetAllAsync()
    {
        return await _context.Payments
            .Where(p => !p.IsDeleted)
            .Select(p => new PaymentDto
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
            })
            .ToListAsync();
    }

    public async Task<PaymentDto?> GetByIdAsync(int id)
    {
        var p = await _context.Payments.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
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

    public async Task AddAsync(CreatePaymentRequest payment)
    {
        var entity = new Payment
        {
            InvoiceId = payment.InvoiceId,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            PaidAt = payment.PaidAt,
            ReceivedBy = payment.ReceivedBy,
            ReferenceNo = payment.ReferenceNo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Payments.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdatePaymentRequest payment)
    {
        var entity = await _context.Payments.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (entity == null) return;

        entity.Amount = payment.Amount;
        entity.PaymentMethod = payment.PaymentMethod;
        entity.PaidAt = payment.PaidAt;
        entity.ReceivedBy = payment.ReceivedBy;
        entity.ReferenceNo = payment.ReferenceNo;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Payments.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Payments.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Payments.Update(entity);
        await _context.SaveChangesAsync();
    }
}
