using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public AuditLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<PagedResult<AuditLogDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.AuditLogs.Where(a => !a.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(a => EF.Functions.ILike(a.Action, pattern) || EF.Functions.ILike(a.TableName, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(a => new AuditLogDto
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
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<AuditLog> ApplySort(IQueryable<AuditLog> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(a => a.CreatedAt),
            ("createdat", false) => query.OrderBy(a => a.CreatedAt),
            (_, true) => query.OrderByDescending(a => a.Id),
            _ => query.OrderBy(a => a.Id),
        };

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
