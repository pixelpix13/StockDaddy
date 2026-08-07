using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface ITenantRepository
{
    Task<PagedResult<TenantDto>> GetPagedAsync(PagedQuery query);
    Task<List<TenantDto>> GetAllAsync();
    Task<TenantDto?> GetByIdAsync(int id);
    Task AddAsync(CreateTenantRequest tenant);
    Task UpdateAsync(int id, UpdateTenantRequest tenant);
    Task DeleteAsync(int id);
}
