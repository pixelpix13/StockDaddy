using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class AuditLogService
{
    private readonly IAuditLogRepository _repo;

    public AuditLogService(IAuditLogRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<AuditLogDto>> GetAllAsync()
    {
        var logs = await _repo.GetAllAsync();
        return logs.Select(log => new AuditLogDto
        {
            Id = log.Id,
            UserId = log.UserId,
            StoreId = log.StoreId,
            Action = log.Action,
            TableName = log.TableName,
            RecordId = log.RecordId,
            OldData = log.OldData,
            NewData = log.NewData,
            Timestamp = log.Timestamp,
            CreatedAt = log.CreatedAt,
            UpdatedAt = log.UpdatedAt
        }).ToList();
    }

    public async Task<AuditLogDto?> GetByIdAsync(Guid id)
    {
        var log = await _repo.GetByIdAsync(id);
        if (log == null) return null;

        return new AuditLogDto
        {
            Id = log.Id,
            UserId = log.UserId,
            StoreId = log.StoreId,
            Action = log.Action,
            TableName = log.TableName,
            RecordId = log.RecordId,
            OldData = log.OldData,
            NewData = log.NewData,
            Timestamp = log.Timestamp,
            CreatedAt = log.CreatedAt,
            UpdatedAt = log.UpdatedAt
        };
    }

    public async Task AddAsync(CreateAuditLogRequest request)
    {
        var log = new AuditLog
        {
            UserId = request.UserId,
            StoreId = request.StoreId,
            Action = request.Action,
            TableName = request.TableName,
            RecordId = request.RecordId,
            OldData = request.OldData,
            NewData = request.NewData,
            Timestamp = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(log);
    }
}
