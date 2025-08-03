using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class ReturnService
{
    private readonly IReturnRepository _repo;

    public ReturnService(IReturnRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ReturnDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(r => new ReturnDto
        {
            Id = r.Id,
            SaleId = r.SaleId,
            StoreId = r.StoreId,
            Reason = r.Reason,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList();
    }

    public async Task<ReturnDto?> GetByIdAsync(Guid id)
    {
        var r = await _repo.GetByIdAsync(id);
        if (r == null) return null;

        return new ReturnDto
        {
            Id = r.Id,
            SaleId = r.SaleId,
            StoreId = r.StoreId,
            Reason = r.Reason,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }

    public async Task AddAsync(CreateReturnRequest request)
    {
        var r = new Return
        {
            SaleId = request.SaleId,
            StoreId = request.StoreId,
            Reason = request.Reason,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(r);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateReturnRequest request)
    {
        var r = await _repo.GetByIdAsync(id);
        if (r == null) return false;

        r.Reason = request.Reason;
        r.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(r);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var r = await _repo.GetByIdAsync(id);
        if (r == null) return false;

        // Soft delete logic
        r.IsDeleted = true;
        r.DeletedAt = DateTime.UtcNow;
        r.UpdatedAt = DateTime.UtcNow;
        
        await _repo.DeleteAsync(id);
        return true;
    }
}
