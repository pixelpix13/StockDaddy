using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface ICustomerRepository
{
    Task<List<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByIdAsync(int id);
    Task AddAsync(CreateCustomerRequest customer);
    Task UpdateAsync(int id, UpdateCustomerRequest customer);
    Task SoftDeleteAsync(int id);
}
