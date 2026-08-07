using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IPermissionRepository
{
    Task<PagedResult<PermissionDto>> GetPagedAsync(PagedQuery query);
    Task<List<PermissionDto>> GetAllAsync();
    Task<PermissionDto?> GetByIdAsync(int id);
    Task AddAsync(CreatePermissionRequest permission);
    Task UpdateAsync(int id, UpdatePermissionRequest permission);
    Task DeleteAsync(int id);
}
