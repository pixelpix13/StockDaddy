using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class SaleService
{
    private readonly ISaleRepository _repo;

    public SaleService(ISaleRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<SaleDto>> GetAllAsync()
    {
        var sales = await _repo.GetAllAsync();
        return sales.Select(s => new SaleDto
        {
            Id = s.Id,
            TenantId = s.TenantId,
            StoreId = s.StoreId,
            CustomerId = s.CustomerId,
            SoldBy = s.SoldBy,
            TotalAmount = s.TotalAmount,
            PaymentMethod = s.PaymentMethod,
            Notes = s.Notes,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        }).ToList();
    }

    public async Task<SaleDto?> GetByIdAsync(Guid id)
    {
        var s = await _repo.GetByIdAsync(id);
        if (s == null) return null;

        return new SaleDto
        {
            Id = s.Id,
            TenantId = s.TenantId,
            StoreId = s.StoreId,
            CustomerId = s.CustomerId,
            SoldBy = s.SoldBy,
            TotalAmount = s.TotalAmount,
            PaymentMethod = s.PaymentMethod,
            Notes = s.Notes,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        };
    }

    public async Task AddAsync(CreateSaleRequest request)
    {
        var sale = new Sale
        {
            TenantId = request.TenantId,
            StoreId = request.StoreId,
            CustomerId = request.CustomerId,
            SoldBy = request.SoldBy,
            TotalAmount = request.TotalAmount,
            PaymentMethod = request.PaymentMethod,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(sale);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateSaleRequest request)
    {
        var sale = await _repo.GetByIdAsync(id);
        if (sale == null) return false;

        sale.TotalAmount = request.TotalAmount;
        sale.PaymentMethod = request.PaymentMethod;
        sale.Notes = request.Notes;
        sale.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(sale);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var sale = await _repo.GetByIdAsync(id);
        if (sale == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
