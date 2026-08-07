using StockDaddy.Application.DTOs;
namespace StockDaddy.Application.Interfaces;

public interface IAuditLogRepository
{
    Task<PagedResult<AuditLogDto>> GetPagedAsync(PagedQuery query);
    Task<List<AuditLogDto>> GetAllAsync();
    Task<AuditLogDto?> GetByIdAsync(int id);
    Task AddAsync(CreateAuditLogRequest auditLog);
}
