using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class RefundRepository : IRefundRepository
{
    private readonly ApplicationDbContext _context;

    public RefundRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<RefundDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Refunds.Where(r => !r.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(r => EF.Functions.ILike(r.Method, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(r => new RefundDto
        {
                Id = r.Id,
                ReturnId = r.ReturnId,
                StoreId = r.StoreId,
                Amount = r.Amount,
                RefundedAt = r.RefundedAt,
                Method = r.Method,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<Refund> ApplySort(IQueryable<Refund> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(r => r.CreatedAt),
            ("createdat", false) => query.OrderBy(r => r.CreatedAt),
            (_, true) => query.OrderByDescending(r => r.Id),
            _ => query.OrderBy(r => r.Id),
        };

    public async Task<List<RefundDto>> GetAllAsync()
    {
        return await _context.Refunds
            .Where(r => !r.IsDeleted)
            .Select(r => new RefundDto
            {
                Id = r.Id,
                ReturnId = r.ReturnId,
                StoreId = r.StoreId,
                Amount = r.Amount,
                RefundedAt = r.RefundedAt,
                Method = r.Method,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<RefundDto?> GetByIdAsync(int id)
    {
        return await _context.Refunds
            .Where(r => r.Id == id && !r.IsDeleted)
            .Select(r => new RefundDto
            {
                Id = r.Id,
                ReturnId = r.ReturnId,
                StoreId = r.StoreId,
                Amount = r.Amount,
                RefundedAt = r.RefundedAt,
                Method = r.Method,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateRefundRequest refund)
    {
        var entity = new Refund
        {
            ReturnId = refund.ReturnId,
            StoreId = refund.StoreId,
            Amount = refund.Amount,
            RefundedAt = refund.RefundedAt,
            Method = refund.Method,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Refunds.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateRefundRequest refund)
    {
        var entity = await _context.Refunds.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        if (entity == null) return;
        entity.Amount = refund.Amount;
        entity.RefundedAt = refund.RefundedAt;
        entity.Method = refund.Method;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Refunds.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Refunds.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Refunds.Update(entity);
        await _context.SaveChangesAsync();
    }
}
