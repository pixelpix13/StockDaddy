using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IReturnRepository
{
    Task<List<Return>> GetAllAsync();
    Task<Return?> GetByIdAsync(Guid id);
    Task AddAsync(Return returnEntity);
    Task UpdateAsync(Return returnEntity);
    Task DeleteAsync(Guid id);
}
