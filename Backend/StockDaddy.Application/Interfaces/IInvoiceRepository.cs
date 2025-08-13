using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IInvoiceRepository
{
    Task<List<InvoiceDto>> GetAllAsync();
    Task<InvoiceDto?> GetByIdAsync(int id);
    Task AddAsync(CreateInvoiceRequest invoice);
    Task UpdateAsync(int id, UpdateInvoiceRequest invoice);
    Task DeleteAsync(int id);
}
