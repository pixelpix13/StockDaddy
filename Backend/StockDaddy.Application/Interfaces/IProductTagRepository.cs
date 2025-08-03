using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IProductTagRepository
{
    Task<List<ProductTag>> GetAllAsync();
    Task<ProductTag?> GetByIdAsync(Guid id);
    Task AddAsync(ProductTag tag);
    Task UpdateAsync(ProductTag tag);
    Task DeleteAsync(Guid id);
}
