using StockDaddy.Application.DTOs;


namespace StockDaddy.Application.Interfaces;

public interface ISupplierRepository
{
    Task<List<SupplierDto>> GetAllAsync();
    Task<SupplierDto?> GetByIdAsync(int id);
    Task AddAsync(CreateSupplierRequest supplier);
    Task UpdateAsync(int id, UpdateSupplierRequest supplier);
    Task DeleteAsync(int id); // Soft delete
}
