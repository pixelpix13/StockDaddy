using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IRefundRepository
{
    Task<List<RefundDto>> GetAllAsync();
    Task<RefundDto?> GetByIdAsync(int id);
    Task AddAsync(CreateRefundRequest refund);
    Task UpdateAsync(int id, UpdateRefundRequest refund);
    Task DeleteAsync(int id);
}
