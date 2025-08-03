using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IRefundRepository
{
    Task<List<Refund>> GetAllAsync();
    Task<Refund?> GetByIdAsync(Guid id);
    Task AddAsync(Refund refund);
    Task UpdateAsync(Refund refund);
    Task DeleteAsync(Guid id);
}
