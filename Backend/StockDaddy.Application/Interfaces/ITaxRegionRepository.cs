using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface ITaxRegionRepository
{
    Task<List<TaxRegionDto>> GetAllAsync();
    Task<TaxRegionDto?> GetByIdAsync(int id);
    Task AddAsync(CreateTaxRegionRequest region);
    Task UpdateAsync(int id,UpdateTaxRegionRequest region);
    Task DeleteAsync(int id);
}
