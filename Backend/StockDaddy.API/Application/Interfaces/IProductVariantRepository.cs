using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IProductVariantRepository
{
    Task<List<ProductVariantDto>> GetAllAsync();
    Task<ProductVariantDto?> GetByIdAsync(int id);
    Task AddAsync(CreateProductVariantRequest variant);
    Task UpdateAsync(int id, UpdateProductVariantRequest variant);
    Task DeleteAsync(int id);
    Task<bool> UpdatePriceAsync(int id, decimal newPrice);

}
