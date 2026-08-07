using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public InvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<InvoiceDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Invoices.Where(i => !i.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(i => EF.Functions.ILike(i.InvoiceNumber, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(i => new InvoiceDto
        {
                Id = i.Id,
                SaleId = i.SaleId,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceDate = i.InvoiceDate,
                DueDate = i.DueDate,
                StoreId = i.StoreId.HasValue ? (int?)null : null, // Adjust as needed for StoreId type
                Status = i.Status,
                FileUrl = i.FileUrl,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<Invoice> ApplySort(IQueryable<Invoice> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(i => i.CreatedAt),
            ("createdat", false) => query.OrderBy(i => i.CreatedAt),
            (_, true) => query.OrderByDescending(i => i.Id),
            _ => query.OrderBy(i => i.Id),
        };

    public async Task<List<InvoiceDto>> GetAllAsync()
    {
        return await _context.Invoices
            .Where(i => !i.IsDeleted)
            .Select(i => new InvoiceDto
            {
                Id = i.Id,
                SaleId = i.SaleId,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceDate = i.InvoiceDate,
                DueDate = i.DueDate,
                StoreId = i.StoreId.HasValue ? (int?)null : null, // Adjust as needed for StoreId type
                Status = i.Status,
                FileUrl = i.FileUrl,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<InvoiceDto?> GetByIdAsync(int id)
    {
        var i = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (i == null) return null;
        return new InvoiceDto
        {
            Id = i.Id,
            SaleId = i.SaleId,
            InvoiceNumber = i.InvoiceNumber,
            InvoiceDate = i.InvoiceDate,
            DueDate = i.DueDate,
            StoreId = i.StoreId.HasValue ? (int?)null : null, // Adjust as needed for StoreId type
            Status = i.Status,
            FileUrl = i.FileUrl,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        };
    }

    public async Task AddAsync(CreateInvoiceRequest invoice)
    {
        var entity = new Invoice
        {
            SaleId = invoice.SaleId,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            StoreId = null, // Adjust as needed for StoreId type
            Status = invoice.Status,
            FileUrl = invoice.FileUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Invoices.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateInvoiceRequest invoice)
    {
        var entity = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (entity == null) return;

        entity.InvoiceDate = invoice.InvoiceDate;
        entity.DueDate = invoice.DueDate;
        entity.Status = invoice.Status;
        entity.FileUrl = invoice.FileUrl;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Invoices.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Invoices.Update(entity);
        await _context.SaveChangesAsync();
    }
}
