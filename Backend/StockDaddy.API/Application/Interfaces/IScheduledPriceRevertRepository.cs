using StockDaddy.Domain.Entities;
using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces
{
    public interface IScheduledPriceRevertRepository
    {
        Task<PagedResult<ScheduledPriceRevert>> GetPagedAsync(PagedQuery query);
        Task<List<ScheduledPriceRevert>> GetAllAsync();
        Task<ScheduledPriceRevert?> GetByIdAsync(int id);
        Task<ScheduledPriceRevert> CreateAsync(ScheduledPriceRevert entity);
        Task<ScheduledPriceRevert?> UpdateAsync(ScheduledPriceRevert entity);
        Task<bool> DeleteAsync(int id);
    }
}
