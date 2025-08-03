using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IGiftOptionRepository
{
    Task<List<GiftOption>> GetAllAsync();
    Task<GiftOption?> GetByIdAsync(Guid id);
    Task AddAsync(GiftOption option);
    Task UpdateAsync(GiftOption option);
    Task DeleteAsync(Guid id);
}
