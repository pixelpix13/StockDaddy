using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface ISubcategoryRepository
{
    Task<List<Subcategory>> GetAllAsync();
    Task<Subcategory?> GetByIdAsync(Guid id);
    Task AddAsync(Subcategory subcategory);
    Task UpdateAsync(Subcategory subcategory);
    Task DeleteAsync(Guid id);
}
