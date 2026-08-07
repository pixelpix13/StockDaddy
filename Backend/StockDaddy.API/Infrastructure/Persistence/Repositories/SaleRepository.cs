using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Domain.Enums;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly ApplicationDbContext _context;

    public SaleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<SaleDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Sales.Where(s => !s.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = RepositoryPaging.LikePattern(q.Search);
            var idMatch = RepositoryPaging.TryParseSearchId(q.Search, out var searchId);
            baseQuery = baseQuery.Where(s =>
                (idMatch && s.Id == searchId) ||
                (idMatch && s.CustomerId == searchId) ||
                (idMatch && s.SoldBy == searchId) ||
                EF.Functions.ILike(s.Notes ?? string.Empty, pattern) ||
                EF.Functions.ILike(s.PaymentMethod.ToString(), pattern) ||
                EF.Functions.ILike(s.TotalAmount.ToString(), pattern) ||
                _context.Users.Any(u => u.Id == s.SoldBy && !u.IsDeleted && EF.Functions.ILike(u.Username, pattern)) ||
                _context.Customers.Any(c => c.Id == s.CustomerId && !c.IsDeleted && EF.Functions.ILike(c.Name, pattern)));
        }

        if (!string.IsNullOrEmpty(q.PaymentMethod) &&
            Enum.TryParse<PaymentMethod>(q.PaymentMethod, true, out var paymentFilter))
        {
            baseQuery = baseQuery.Where(s => s.PaymentMethod == paymentFilter);
        }

        if (q.CustomerId.HasValue)
        {
            baseQuery = baseQuery.Where(s => s.CustomerId == q.CustomerId.Value);
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(s => new SaleDto
        {
                Id = s.Id,
                TenantId = s.TenantId,
                StoreId = s.StoreId,
                CustomerId = s.CustomerId,
                CustomerName = s.Customer != null ? s.Customer.Name : null,
                SoldBy = s.SoldBy,
                SoldByName = _context.Users
                    .Where(u => u.Id == s.SoldBy && !u.IsDeleted)
                    .Select(u => u.Username)
                    .FirstOrDefault(),
                SubtotalAmount = s.SubtotalAmount,
                TaxAmount = s.TaxAmount,
                DiscountAmount = s.DiscountAmount,
                TotalAmount = s.TotalAmount,
                PaymentMethod = s.PaymentMethod,
                Notes = s.Notes,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<Sale> ApplySort(IQueryable<Sale> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", true) => query.OrderByDescending(s => s.CreatedAt),
            ("createdat", false) => query.OrderBy(s => s.CreatedAt),
            (_, true) => query.OrderByDescending(s => s.Id),
            _ => query.OrderBy(s => s.Id),
        };

    public async Task<List<SaleDto>> GetAllAsync()
    {
        return await _context.Sales
            .Where(s => !s.IsDeleted)
            .Select(s => new SaleDto
            {
                Id = s.Id,
                TenantId = s.TenantId,
                StoreId = s.StoreId,
                CustomerId = s.CustomerId,
                SoldBy = s.SoldBy,
                TotalAmount = s.TotalAmount,
                PaymentMethod = s.PaymentMethod,
                Notes = s.Notes,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<SaleDto?> GetByIdAsync(int id)
    {
        return await _context.Sales
            .Where(s => s.Id == id && !s.IsDeleted)
            .Select(s => new SaleDto
            {
                Id = s.Id,
                TenantId = s.TenantId,
                StoreId = s.StoreId,
                CustomerId = s.CustomerId,
                SoldBy = s.SoldBy,
                TotalAmount = s.TotalAmount,
                PaymentMethod = s.PaymentMethod,
                Notes = s.Notes,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> AddAsync(CreateSaleRequest sale)
    {
        var entity = new Sale
        {
            TenantId = sale.TenantId,
            StoreId = sale.StoreId,
            CustomerId = sale.CustomerId,
            SoldBy = sale.SoldBy,
            TotalAmount = sale.TotalAmount,
            PaymentMethod = sale.PaymentMethod,
            Notes = sale.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Sales.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(int id, UpdateSaleRequest sale)
    {
        var entity = await _context.Sales.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (entity == null) return;
        entity.TotalAmount = sale.TotalAmount;
        entity.PaymentMethod = sale.PaymentMethod;
        entity.Notes = sale.Notes;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Sales.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Sales.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Sales.Update(entity);
        await _context.SaveChangesAsync();
    }
}
