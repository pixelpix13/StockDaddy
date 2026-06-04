using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IIntegrationEventRepository
{
    Task<List<IntegrationEventDto>> GetAllAsync();
    Task<IntegrationEventDto?> GetByIdAsync(int id);
    Task AddAsync(CreateIntegrationEventRequest integrationEvent);
    Task UpdateAsync(int id, UpdateIntegrationEventRequest integrationEvent);
    Task DeleteAsync(int id);
}
