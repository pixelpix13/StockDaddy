using Microsoft.EntityFrameworkCore;
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

    public async Task<List<AuditLog>> GetAllAsync()
    {
        return await _context.AuditLogs
            .Where(a => !a.IsDeleted)
            .ToListAsync();
    }

    public async Task<AuditLog?> GetByIdAsync(Guid id)
    {
        return await _context.AuditLogs
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
    }

    public async Task AddAsync(AuditLog log)
    {
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var log = await _context.AuditLogs.FindAsync(id);
        if (log == null) return;

        log.IsDeleted = true;
        log.DeletedAt = DateTime.UtcNow;
        log.UpdatedAt = DateTime.UtcNow;

        _context.AuditLogs.Update(log);
        await _context.SaveChangesAsync();
    }
}
