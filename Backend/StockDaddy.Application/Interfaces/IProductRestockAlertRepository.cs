using StockDaddy.Application.DTOs;
namespace StockDaddy.Application.Interfaces;

public interface IProductRestockAlertRepository
{
    Task<List<ProductRestockAlertDto>> GetAllAsync();
    Task<ProductRestockAlertDto?> GetByIdAsync(int id);
    Task AddAsync(CreateProductRestockAlertRequest alert);
    Task UpdateAsync(int id, UpdateProductRestockAlertRequest alert);
    Task DeleteAsync(int id);
}
