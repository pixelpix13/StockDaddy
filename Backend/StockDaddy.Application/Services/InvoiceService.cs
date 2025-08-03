using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class InvoiceService
{
    private readonly IInvoiceRepository _repo;

    public InvoiceService(IInvoiceRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<InvoiceDto>> GetAllAsync()
    {
        var invoices = await _repo.GetAllAsync();
        return invoices.Select(i => new InvoiceDto
        {
            Id = i.Id,
            SaleId = i.SaleId,
            InvoiceNumber = i.InvoiceNumber,
            InvoiceDate = i.InvoiceDate,
            DueDate = i.DueDate,
            StoreId = i.StoreId,
            Status = i.Status,
            FileUrl = i.FileUrl,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        }).ToList();
    }

    public async Task<InvoiceDto?> GetByIdAsync(Guid id)
    {
        var i = await _repo.GetByIdAsync(id);
        if (i == null) return null;

        return new InvoiceDto
        {
            Id = i.Id,
            SaleId = i.SaleId,
            InvoiceNumber = i.InvoiceNumber,
            InvoiceDate = i.InvoiceDate,
            DueDate = i.DueDate,
            StoreId = i.StoreId,
            Status = i.Status,
            FileUrl = i.FileUrl,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        };
    }

    public async Task AddAsync(CreateInvoiceRequest request)
    {
        var invoice = new Invoice
        {
            SaleId = request.SaleId,
            InvoiceNumber = request.InvoiceNumber,
            InvoiceDate = request.InvoiceDate,
            DueDate = request.DueDate,
            StoreId = request.StoreId,
            Status = request.Status,
            FileUrl = request.FileUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(invoice);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateInvoiceRequest request)
    {
        var invoice = await _repo.GetByIdAsync(id);
        if (invoice == null) return false;

        invoice.InvoiceDate = request.InvoiceDate;
        invoice.DueDate = request.DueDate;
        invoice.Status = request.Status;
        invoice.FileUrl = request.FileUrl;
        invoice.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(invoice);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var invoice = await _repo.GetByIdAsync(id);
        if (invoice == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
