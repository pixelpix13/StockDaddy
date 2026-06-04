using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces
{
    public interface IScheduledPriceRevertRepository
    {
        Task<List<ScheduledPriceRevert>> GetAllAsync();
        Task<ScheduledPriceRevert?> GetByIdAsync(int id);
        Task<ScheduledPriceRevert> CreateAsync(ScheduledPriceRevert entity);
        Task<ScheduledPriceRevert?> UpdateAsync(ScheduledPriceRevert entity);
        Task<bool> DeleteAsync(int id);
    }
}
