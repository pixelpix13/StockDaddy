using StockDaddy.Application.DTOs;
namespace StockDaddy.Application.Interfaces;

public interface IPaymentRepository
{
    Task<PagedResult<PaymentDto>> GetPagedAsync(PagedQuery query);
    Task<List<PaymentDto>> GetAllAsync();
    Task<PaymentDto?> GetByIdAsync(int id);
    Task AddAsync(CreatePaymentRequest payment);
    Task UpdateAsync(int id, UpdatePaymentRequest payment);
    Task DeleteAsync(int id);
}
