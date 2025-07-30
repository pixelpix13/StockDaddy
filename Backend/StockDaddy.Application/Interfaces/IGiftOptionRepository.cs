using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IGiftOptionRepository
{
    Task<GiftOption?> GetBySaleIdAsync(Guid saleId);
    Task<GiftOption?> GetByIdAsync(Guid id);
    Task AddAsync(GiftOption option);
    Task UpdateAsync(GiftOption option);
    Task DeleteAsync(Guid id);
}
