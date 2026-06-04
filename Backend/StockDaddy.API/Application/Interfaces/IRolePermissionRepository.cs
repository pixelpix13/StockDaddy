namespace StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

public interface IRolePermissionRepository
{
    Task<List<RolePermissionDto>> GetAllAsync();
    Task<RolePermissionDto?> GetByIdAsync(int id);
    Task AddAsync(CreateRolePermissionRequest rolePermission);
    Task UpdateAsync(int id, UpdateRolePermissionRequest rolePermission);
    Task DeleteAsync(int id);
}
