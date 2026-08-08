using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Authorization;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Helpers;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IRequestContext _requestContext;

    public AuditLogRepository(ApplicationDbContext context, IRequestContext requestContext)
    {
        _context = context;
        _requestContext = requestContext;
    }

    public async Task<PagedResult<AuditLogDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.AuditLogs
            .AsNoTracking()
            .Where(a => !a.IsDeleted);

        var storeFilter = QueryScope.ResolveStoreFilter(q, _requestContext);
        if (storeFilter.HasValue)
        {
            baseQuery = baseQuery.Where(a => a.StoreId == storeFilter.Value);
        }

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = RepositoryPaging.LikePattern(q.Search);
            var idMatch = RepositoryPaging.TryParseSearchId(q.Search, out var searchId);
            baseQuery = baseQuery.Where(a =>
                (idMatch && a.Id == searchId) ||
                (idMatch && a.UserId == searchId) ||
                (idMatch && a.StoreId == searchId) ||
                EF.Functions.ILike(a.Action, pattern) ||
                EF.Functions.ILike(a.TableName, pattern) ||
                EF.Functions.ILike(a.RecordId, pattern) ||
                EF.Functions.ILike(a.OldData, pattern) ||
                EF.Functions.ILike(a.NewData, pattern) ||
                _context.Users.Any(u => u.Id == a.UserId && !u.IsDeleted && EF.Functions.ILike(u.Username, pattern)));
        }

        if (!string.IsNullOrEmpty(q.Status))
        {
            baseQuery = baseQuery.Where(a => a.Action == q.Status);
        }

        if (!string.IsNullOrEmpty(q.Entity))
        {
            baseQuery = baseQuery.Where(a => a.TableName == q.Entity);
        }

        if (q.UserId.HasValue)
        {
            baseQuery = baseQuery.Where(a => a.UserId == q.UserId.Value);
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
            UpdatedAt = a.UpdatedAt,
            Username = _context.Users
                .Where(u => u.Id == a.UserId && !u.IsDeleted)
                .Select(u => u.Username)
                .FirstOrDefault()
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<AuditLog> ApplySort(IQueryable<AuditLog> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("action", true) => query.OrderByDescending(a => a.Action),
            ("action", false) => query.OrderBy(a => a.Action),
            ("tablename", true) => query.OrderByDescending(a => a.TableName),
            ("tablename", false) => query.OrderBy(a => a.TableName),
            ("timestamp", true) => query.OrderByDescending(a => a.Timestamp),
            ("timestamp", false) => query.OrderBy(a => a.Timestamp),
            ("createdat", true) => query.OrderByDescending(a => a.CreatedAt),
            ("createdat", false) => query.OrderBy(a => a.CreatedAt),
            (_, true) => query.OrderByDescending(a => a.Id),
            _ => query.OrderBy(a => a.Id),
        };

    public async Task<List<AuditLogDto>> GetAllAsync()
    {
        var query = _context.AuditLogs
            .AsNoTracking()
            .Where(a => !a.IsDeleted);

        if (_requestContext.ActiveStoreId.HasValue)
        {
            query = query.Where(a => a.StoreId == _requestContext.ActiveStoreId.Value);
        }

        return await query
            .OrderByDescending(a => a.Id)
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
                UpdatedAt = a.UpdatedAt,
                Username = _context.Users
                    .Where(u => u.Id == a.UserId && !u.IsDeleted)
                    .Select(u => u.Username)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }

    public async Task<AuditLogDto?> GetByIdAsync(int id)
    {
        var query = _context.AuditLogs
            .AsNoTracking()
            .Where(a => a.Id == id && !a.IsDeleted);

        if (_requestContext.ActiveStoreId.HasValue)
        {
            query = query.Where(a => a.StoreId == _requestContext.ActiveStoreId.Value);
        }

        return await query
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
                UpdatedAt = a.UpdatedAt,
                Username = _context.Users
                    .Where(u => u.Id == a.UserId && !u.IsDeleted)
                    .Select(u => u.Username)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();
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
            OldData = auditLog.OldData ?? string.Empty,
            NewData = auditLog.NewData ?? string.Empty,
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
