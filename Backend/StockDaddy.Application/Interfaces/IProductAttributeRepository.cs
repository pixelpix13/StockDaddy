using StockDaddy.Application.DTOs;


namespace StockDaddy.Application.Interfaces;

public interface IProductAttributeRepository
{
    Task<List<ProductAttributeDto>> GetAllAsync();
    Task<ProductAttributeDto?> GetByIdAsync(int id);
    Task AddAsync(CreateProductAttributeRequest attribute);
    Task UpdateAsync(int id, UpdateProductAttributeRequest attribute);
    Task DeleteAsync(int id);
}
