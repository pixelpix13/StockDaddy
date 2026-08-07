using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class AdjustedInvoiceRepository : IAdjustedInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public AdjustedInvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AdjustedInvoiceDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.AdjustedInvoices.Where(ai => !ai.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(ai => EF.Functions.ILike(ai.AdjustmentReason, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(ai => new AdjustedInvoiceDto
        {
                Id = ai.Id,
                InvoiceId = ai.InvoiceId,
                AdjustedTotalAmount = ai.AdjustedTotalAmount,
                AdjustmentReason = ai.AdjustmentReason,
                AdjustedBy = ai.AdjustedBy,
                AdjustedAt = ai.AdjustedAt,
                IsVisibleToCustomer = ai.IsVisibleToCustomer
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<AdjustedInvoice> ApplySort(IQueryable<AdjustedInvoice> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(ai => ai.CreatedAt),
            ("createdat", false) => query.OrderBy(ai => ai.CreatedAt),
            (_, true) => query.OrderByDescending(ai => ai.Id),
            _ => query.OrderBy(ai => ai.Id),
        };

    public async Task<List<AdjustedInvoiceDto>> GetAllAsync()
    {
        return await _context.AdjustedInvoices
            .Where(ai => !ai.IsDeleted)
            .Select(ai => new AdjustedInvoiceDto
            {
                Id = ai.Id,
                InvoiceId = ai.InvoiceId,
                AdjustedTotalAmount = ai.AdjustedTotalAmount,
                AdjustmentReason = ai.AdjustmentReason,
                AdjustedBy = ai.AdjustedBy,
                AdjustedAt = ai.AdjustedAt,
                IsVisibleToCustomer = ai.IsVisibleToCustomer
            })
            .ToListAsync();
    }

    public async Task<AdjustedInvoiceDto?> GetByIdAsync(int id)
    {
        var ai = await _context.AdjustedInvoices.FirstOrDefaultAsync(ai => ai.Id == id && !ai.IsDeleted);
        if (ai == null) return null;
        return new AdjustedInvoiceDto
        {
            Id = ai.Id,
            InvoiceId = ai.InvoiceId,
            AdjustedTotalAmount = ai.AdjustedTotalAmount,
            AdjustmentReason = ai.AdjustmentReason,
            AdjustedBy = ai.AdjustedBy,
            AdjustedAt = ai.AdjustedAt,
            IsVisibleToCustomer = ai.IsVisibleToCustomer
        };
    }

    public async Task AddAsync(CreateAdjustedInvoiceRequest invoice)
    {
        var entity = new AdjustedInvoice
        {
            InvoiceId = invoice.InvoiceId,
            AdjustedTotalAmount = invoice.AdjustedTotalAmount,
            AdjustmentReason = invoice.AdjustmentReason,
            AdjustedBy = invoice.AdjustedBy,
            AdjustedAt = DateTime.UtcNow,
            IsVisibleToCustomer = invoice.IsVisibleToCustomer,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.AdjustedInvoices.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateAdjustedInvoiceRequest invoice)
    {
        var entity = await _context.AdjustedInvoices.FirstOrDefaultAsync(ai => ai.Id == id && !ai.IsDeleted);
        if (entity == null) return;

        entity.AdjustedTotalAmount = invoice.AdjustedTotalAmount;
        entity.AdjustmentReason = invoice.AdjustmentReason;
        entity.IsVisibleToCustomer = invoice.IsVisibleToCustomer;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.AdjustedInvoices.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.AdjustedInvoices.FirstOrDefaultAsync(ai => ai.Id == id && !ai.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.AdjustedInvoices.Update(entity);
        await _context.SaveChangesAsync();
    }
}
