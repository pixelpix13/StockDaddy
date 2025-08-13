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
        return await _repo.GetAllAsync();
    }


    public async Task<AdjustedInvoiceDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }


    public async Task<AdjustedInvoiceDto> AddAsync(CreateAdjustedInvoiceRequest request)
    {
        await _repo.AddAsync(request);
        // Ideally, repository should return the created DTO. For now, fetch the latest for this invoice/user.
        var all = await _repo.GetAllAsync();
        return all.LastOrDefault(x => x.InvoiceId == request.InvoiceId && x.AdjustedBy == request.AdjustedBy && x.AdjustedTotalAmount == request.AdjustedTotalAmount && x.AdjustmentReason == request.AdjustmentReason) ?? new AdjustedInvoiceDto();
    }


    public async Task<AdjustedInvoiceDto?> UpdateAsync(int id, UpdateAdjustedInvoiceRequest request)
    {
        await _repo.UpdateAsync(id, request);
        return await _repo.GetByIdAsync(id);
    }


    public async Task DeleteAsync(int id)
    {
        await _repo.DeleteAsync(id);
    }
}
