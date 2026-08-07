namespace StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

public interface IUserRepository
{
    Task<PagedResult<UserDto>> GetPagedAsync(PagedQuery query);
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int id);
    Task AddAsync(CreateUserRequest user);
    Task UpdateAsync(int id, UpdateUserRequest user);
    Task SoftDeleteAsync(int id);
}
