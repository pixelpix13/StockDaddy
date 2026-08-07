using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class ReturnRepository : IReturnRepository
{
    private readonly ApplicationDbContext _context;

    public ReturnRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ReturnDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Returns.Where(r => !r.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(r => EF.Functions.ILike(r.Reason, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(r => new ReturnDto
        {
                Id = r.Id,
                SaleId = r.SaleId,
                StoreId = r.StoreId,
                Reason = r.Reason,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<Return> ApplySort(IQueryable<Return> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(r => r.CreatedAt),
            ("createdat", false) => query.OrderBy(r => r.CreatedAt),
            (_, true) => query.OrderByDescending(r => r.Id),
            _ => query.OrderBy(r => r.Id),
        };

    public async Task<List<ReturnDto>> GetAllAsync()
    {
        return await _context.Returns
            .Where(r => !r.IsDeleted)
            .Select(r => new ReturnDto
            {
                Id = r.Id,
                SaleId = r.SaleId,
                StoreId = r.StoreId,
                Reason = r.Reason,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<ReturnDto?> GetByIdAsync(int id)
    {
        return await _context.Returns
            .Where(r => r.Id == id && !r.IsDeleted)
            .Select(r => new ReturnDto
            {
                Id = r.Id,
                SaleId = r.SaleId,
                StoreId = r.StoreId,
                Reason = r.Reason,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateReturnRequest returnEntity)
    {
        var entity = new Return
        {
            SaleId = returnEntity.SaleId,
            StoreId = returnEntity.StoreId,
            Reason = returnEntity.Reason,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Returns.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateReturnRequest returnEntity)
    {
        var entity = await _context.Returns.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (entity == null) return;
        entity.Reason = returnEntity.Reason;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Returns.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Returns.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Returns.Update(entity);
        await _context.SaveChangesAsync();
    }
}
