using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class AdjustedInvoiceService
{
    private readonly IAdjustedInvoiceRepository _repo;

    public AdjustedInvoiceService(IAdjustedInvoiceRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<AdjustedInvoiceDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(x => new AdjustedInvoiceDto
        {
            Id = x.Id,
            InvoiceId = x.InvoiceId,
            AdjustedTotalAmount = x.AdjustedTotalAmount,
            AdjustmentReason = x.AdjustmentReason,
            AdjustedBy = x.AdjustedBy,
            AdjustedAt = x.AdjustedAt,
            IsVisibleToCustomer = x.IsVisibleToCustomer
        }).ToList();
    }

    public async Task<AdjustedInvoiceDto?> GetByIdAsync(Guid id)
    {
        var x = await _repo.GetByIdAsync(id);
        if (x == null) return null;

        return new AdjustedInvoiceDto
        {
            Id = x.Id,
            InvoiceId = x.InvoiceId,
            AdjustedTotalAmount = x.AdjustedTotalAmount,
            AdjustmentReason = x.AdjustmentReason,
            AdjustedBy = x.AdjustedBy,
            AdjustedAt = x.AdjustedAt,
            IsVisibleToCustomer = x.IsVisibleToCustomer
        };
    }

    public async Task AddAsync(CreateAdjustedInvoiceRequest request)
    {
        var x = new AdjustedInvoice
        {
            InvoiceId = request.InvoiceId,
            AdjustedTotalAmount = request.AdjustedTotalAmount,
            AdjustmentReason = request.AdjustmentReason,
            AdjustedBy = request.AdjustedBy,
            AdjustedAt = DateTime.UtcNow,
            IsVisibleToCustomer = request.IsVisibleToCustomer
        };

        await _repo.AddAsync(x);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateAdjustedInvoiceRequest request)
    {
        var x = await _repo.GetByIdAsync(id);
        if (x == null) return false;

        x.AdjustedTotalAmount = request.AdjustedTotalAmount;
        x.AdjustmentReason = request.AdjustmentReason;
        x.IsVisibleToCustomer = request.IsVisibleToCustomer;

        await _repo.UpdateAsync(x);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var x = await _repo.GetByIdAsync(id);
        if (x == null) return false;

        // Soft delete logic
        x.IsDeleted = true;
        x.DeletedAt = DateTime.UtcNow;
        x.UpdatedAt = DateTime.UtcNow;

        await _repo.DeleteAsync(id);
        return true;
    }
}
