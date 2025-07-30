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

    public async Task<InvoiceDto?> GetByIdAsync(Guid id)
    {
        var i = await _repo.GetByIdAsync(id);
        return i == null ? null : new InvoiceDto
        {
            Id = i.Id,
            SaleId = i.SaleId,
            InvoiceNumber = i.InvoiceNumber,
            InvoiceDate = i.InvoiceDate,
            DueDate = i.DueDate,
            Status = i.Status,
            FileUrl = i.FileUrl
        };
    }

    public async Task<Guid> CreateAsync(CreateInvoiceRequest request)
    {
        var invoice = new Invoice
        {
            SaleId = request.SaleId,
            InvoiceNumber = request.InvoiceNumber,
            InvoiceDate = request.InvoiceDate,
            DueDate = request.DueDate,
            FileUrl = request.FileUrl,
            Status = "Unpaid"
        };

        await _repo.AddAsync(invoice);
        return invoice.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateInvoiceRequest request)
    {
        var invoice = await _repo.GetByIdAsync(id);
        if (invoice == null) return false;

        if (request.DueDate.HasValue)
            invoice.DueDate = request.DueDate.Value;

        if (!string.IsNullOrWhiteSpace(request.Status))
            invoice.Status = request.Status;

        if (!string.IsNullOrWhiteSpace(request.FileUrl))
            invoice.FileUrl = request.FileUrl;

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
