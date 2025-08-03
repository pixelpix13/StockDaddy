using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IProductRestockAlertRepository
{
    Task<List<ProductRestockAlert>> GetAllAsync();
    Task<ProductRestockAlert?> GetByIdAsync(Guid id);
    Task AddAsync(ProductRestockAlert alert);
    Task UpdateAsync(ProductRestockAlert alert);
    Task DeleteAsync(Guid id);
}
