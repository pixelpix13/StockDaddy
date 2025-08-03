using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task SoftDeleteAsync(Guid id);
}
