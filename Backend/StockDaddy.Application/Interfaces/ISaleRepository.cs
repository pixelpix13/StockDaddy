using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface ISaleRepository
{
    Task<List<SaleDto>> GetAllAsync();
    Task<SaleDto?> GetByIdAsync(int id);
    Task AddAsync(CreateSaleRequest sale);
    Task UpdateAsync(int id, UpdateSaleRequest sale);
    Task DeleteAsync(int id);
}
