using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(Guid tenantId);
    Task<Customer?> GetByIdAsync(Guid id);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(Guid id);
}
