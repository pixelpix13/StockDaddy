using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface ICreditLedgerRepository
{
    Task<PagedResult<CreditLedgerDto>> GetPagedAsync(PagedQuery query);
    Task<CreditLedgerDto?> GetByIdAsync(int id);
    Task<CreditLedgerDto?> RecordPaymentAsync(int id, RecordCreditPaymentRequest request);
    Task<CreditLedgerDto?> UpdateAsync(int id, UpdateCreditLedgerRequest request);
    Task RefreshOverdueStatusesAsync();
}
