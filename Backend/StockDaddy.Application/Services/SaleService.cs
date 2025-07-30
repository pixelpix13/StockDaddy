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

    public async Task<List<SaleDto>> GetAllAsync(Guid tenantId)
    {
        var sales = await _repo.GetAllAsync(tenantId);
        return sales.Select(s => new SaleDto
        {
            Id = s.Id,
            TenantId = s.TenantId,
            CustomerId = s.CustomerId,
            TotalAmount = s.TotalAmount,
            PaymentMethod = s.PaymentMethod,
            SoldBy = s.SoldBy,
            CreatedAt = s.CreatedAt,
            Notes = s.Notes
        }).ToList();
    }

    public async Task<SaleDto?> GetByIdAsync(Guid id)
    {
        var sale = await _repo.GetByIdAsync(id);
        if (sale == null) return null;

        return new SaleDto
        {
            Id = sale.Id,
            TenantId = sale.TenantId,
            CustomerId = sale.CustomerId,
            TotalAmount = sale.TotalAmount,
            PaymentMethod = sale.PaymentMethod,
            SoldBy = sale.SoldBy,
            CreatedAt = sale.CreatedAt,
            Notes = sale.Notes
        };
    }

    public async Task<Guid> CreateAsync(CreateSaleRequest request)
    {
        var sale = new Sale
        {
            TenantId = request.TenantId,
            CustomerId = request.CustomerId,
            TotalAmount = request.TotalAmount,
            PaymentMethod = request.PaymentMethod,
            SoldBy = request.SoldBy,
            Notes = request.Notes
        };

        await _repo.AddAsync(sale);
        return sale.Id;
    }
    public async Task<bool> UpdateAsync(Guid id, UpdateSaleRequest request)
    {
        var sale = await _repo.GetByIdAsync(id);
        if (sale == null) return false;

        sale.CustomerId = request.CustomerId;
        sale.TotalAmount = request.TotalAmount;
        sale.PaymentMethod = request.PaymentMethod;
        sale.Notes = request.Notes;

        await _repo.UpdateAsync(sale);
        return true;
    }
    // DELETE /sales/{id}
    public async Task<bool> DeleteAsync(Guid id)
    {
        var sale = await _repo.GetByIdAsync(id);
        if (sale == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
