using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public AuditLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<List<AuditLogDto>> GetAllAsync()
    {
        return await _context.AuditLogs
            .Where(a => !a.IsDeleted)
            .Select(a => new AuditLogDto
            {
                Id = a.Id,
                UserId = a.UserId,
                StoreId = a.StoreId,
                Action = a.Action,
                TableName = a.TableName,
                RecordId = a.RecordId,
                OldData = a.OldData,
                NewData = a.NewData,
                Timestamp = a.Timestamp,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync();
    }


    public async Task<AuditLogDto?> GetByIdAsync(int id)
    {
        var a = await _context.AuditLogs.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (a == null) return null;
        return new AuditLogDto
        {
            Id = a.Id,
            UserId = a.UserId,
            StoreId = a.StoreId,
            Action = a.Action,
            TableName = a.TableName,
            RecordId = a.RecordId,
            OldData = a.OldData,
            NewData = a.NewData,
            Timestamp = a.Timestamp,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        };
    }


    public async Task AddAsync(CreateAuditLogRequest auditLog)
    {
        var entity = new AuditLog
        {
            UserId = auditLog.UserId,
            StoreId = auditLog.StoreId,
            Action = auditLog.Action,
            TableName = auditLog.TableName,
            RecordId = auditLog.RecordId,
            OldData = auditLog.OldData,
            NewData = auditLog.NewData,
            Timestamp = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.AuditLogs.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var entity = await _context.AuditLogs.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.AuditLogs.Update(entity);
        await _context.SaveChangesAsync();
    }
}
