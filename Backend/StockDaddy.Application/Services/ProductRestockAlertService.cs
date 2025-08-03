using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class ProductRestockAlertService
{
    private readonly IProductRestockAlertRepository _repo;

    public ProductRestockAlertService(IProductRestockAlertRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductRestockAlertDto>> GetAllAsync()
    {
        var alerts = await _repo.GetAllAsync();
        return alerts.Select(a => new ProductRestockAlertDto
        {
            Id = a.Id,
            ProductId = a.ProductId,
            StoreId = a.StoreId,
            VariantId = a.VariantId,
            TriggeredAt = a.TriggeredAt,
            Status = a.Status,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        }).ToList();
    }

    public async Task<ProductRestockAlertDto?> GetByIdAsync(Guid id)
    {
        var alert = await _repo.GetByIdAsync(id);
        if (alert == null) return null;

        return new ProductRestockAlertDto
        {
            Id = alert.Id,
            ProductId = alert.ProductId,
            StoreId = alert.StoreId,
            VariantId = alert.VariantId,
            TriggeredAt = alert.TriggeredAt,
            Status = alert.Status,
            CreatedAt = alert.CreatedAt,
            UpdatedAt = alert.UpdatedAt
        };
    }

    public async Task AddAsync(CreateProductRestockAlertRequest request)
    {
        var alert = new ProductRestockAlert
        {
            ProductId = request.ProductId,
            StoreId = request.StoreId,
            VariantId = request.VariantId,
            TriggeredAt = DateTime.UtcNow,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(alert);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductRestockAlertRequest request)
    {
        var alert = await _repo.GetByIdAsync(id);
        if (alert == null) return false;

        alert.Status = request.Status;
        alert.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(alert);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var alert = await _repo.GetByIdAsync(id);
        if (alert == null) return false;

        alert.IsDeleted = true;
        alert.DeletedAt = DateTime.UtcNow;
        alert.UpdatedAt = DateTime.UtcNow;

        await _repo.DeleteAsync(id);
        return true;
    }
}
