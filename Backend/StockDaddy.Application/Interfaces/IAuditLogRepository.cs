using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Interfaces;

public interface IAuditLogRepository
{
    Task<List<AuditLog>> GetAllAsync();
    Task<AuditLog?> GetByIdAsync(Guid id);
    Task AddAsync(AuditLog auditLog);
}
