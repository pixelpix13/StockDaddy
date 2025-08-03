using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IRolePermissionRepository
{
    Task<List<RolePermission>> GetAllAsync();
    Task<RolePermission?> GetByIdAsync(Guid id);
    Task AddAsync(RolePermission rolePermission);
    Task UpdateAsync(RolePermission rolePermission);
    Task DeleteAsync(Guid id);
}
