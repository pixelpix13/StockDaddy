using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IAdjustedInvoiceRepository
{
    Task<List<AdjustedInvoiceDto>> GetAllAsync();
    Task<AdjustedInvoiceDto?> GetByIdAsync(int id);
    Task AddAsync(CreateAdjustedInvoiceRequest invoice);
    Task UpdateAsync(int id, UpdateAdjustedInvoiceRequest invoice);
    Task DeleteAsync(int id);
}
