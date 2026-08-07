using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IRoleRepository
{
    Task<PagedResult<RoleDto>> GetPagedAsync(PagedQuery query);
    Task<List<RoleDto>> GetAllAsync();
    Task<RoleDto?> GetByIdAsync(int id);
    Task AddAsync(CreateRoleRequest role);
    Task UpdateAsync(int id, UpdateRoleRequest role);
    Task DeleteAsync(int id);
}
