using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IIntegrationEventRepository
{
    Task<List<IntegrationEvent>> GetAllAsync();
    Task<IntegrationEvent?> GetByIdAsync(Guid id);
    Task AddAsync(IntegrationEvent integrationEvent);
    Task UpdateAsync(IntegrationEvent integrationEvent);
    Task DeleteAsync(Guid id);
}
