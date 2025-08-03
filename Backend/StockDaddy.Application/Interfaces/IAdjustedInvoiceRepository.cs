using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IAdjustedInvoiceRepository
{
    Task<List<AdjustedInvoice>> GetAllAsync();
    Task<AdjustedInvoice?> GetByIdAsync(Guid id);
    Task AddAsync(AdjustedInvoice invoice);
    Task UpdateAsync(AdjustedInvoice invoice);
    Task DeleteAsync(Guid id);
}
