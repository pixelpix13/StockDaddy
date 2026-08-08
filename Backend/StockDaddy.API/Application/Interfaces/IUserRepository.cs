namespace StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

public interface IUserRepository
{
    Task<PagedResult<UserDto>> GetPagedAsync(PagedQuery query);
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int id);
    Task AddAsync(CreateUserRequest user);
    Task UpdateAsync(int id, UpdateUserRequest user);
    Task SyncStoreAssignmentsAsync(int userId, int tenantId, int fallbackRoleId, List<UserStoreAssignmentDto> assignments, int? defaultStoreId, int? updateFallbackRoleId = null);
    Task SoftDeleteAsync(int id);
}
