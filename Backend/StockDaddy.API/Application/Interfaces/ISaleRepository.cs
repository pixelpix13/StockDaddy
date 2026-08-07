using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface ISaleRepository
{
    Task<PagedResult<SaleDto>> GetPagedAsync(PagedQuery query);
    Task<List<SaleDto>> GetAllAsync();
    Task<SaleDto?> GetByIdAsync(int id);
    Task<int> AddAsync(CreateSaleRequest sale);
    Task UpdateAsync(int id, UpdateSaleRequest sale);
    Task DeleteAsync(int id);
}
