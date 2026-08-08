using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Authorization;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Helpers;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IRequestContext _requestContext;

    public CompanyRepository(ApplicationDbContext context, IRequestContext requestContext)
    {
        _context = context;
        _requestContext = requestContext;
    }

    public async Task<PagedResult<CompanyDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Companies.Where(c => !c.IsDeleted);

        if (_requestContext.TenantId.HasValue)
        {
            baseQuery = baseQuery.Where(c => c.TenantId == _requestContext.TenantId.Value);
        }

        var storeFilter = QueryScope.ResolveStoreFilter(query, _requestContext);
        if (storeFilter.HasValue)
        {
            baseQuery = baseQuery.Where(c => c.StoreId == storeFilter.Value);
        }

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(c =>
                EF.Functions.ILike(c.Name, pattern) ||
                EF.Functions.ILike(c.Email, pattern) ||
                EF.Functions.ILike(c.Phone, pattern) ||
                EF.Functions.ILike(c.ContactName, pattern) ||
                EF.Functions.ILike(c.Gstin, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(c => MapToDto(c));
        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<Company> ApplySort(IQueryable<Company> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("name", true) => query.OrderByDescending(c => c.Name),
            ("name", false) => query.OrderBy(c => c.Name),
            ("email", true) => query.OrderByDescending(c => c.Email),
            ("email", false) => query.OrderBy(c => c.Email),
            ("createdat", true) => query.OrderByDescending(c => c.CreatedAt),
            ("createdat", false) => query.OrderBy(c => c.CreatedAt),
            (_, true) => query.OrderByDescending(c => c.Id),
            _ => query.OrderBy(c => c.Id),
        };

    public async Task<List<CompanyDto>> GetAllAsync()
    {
        var query = _context.Companies.Where(c => !c.IsDeleted);

        if (_requestContext.TenantId.HasValue)
        {
            query = query.Where(c => c.TenantId == _requestContext.TenantId.Value);
        }

        if (_requestContext.ActiveStoreId.HasValue)
        {
            query = query.Where(c => c.StoreId == _requestContext.ActiveStoreId.Value);
        }

        return await query
            .OrderBy(c => c.Name)
            .Select(c => MapToDto(c))
            .ToListAsync();
    }

    public async Task<CompanyDto?> GetByIdAsync(int id)
    {
        var query = _context.Companies.Where(c => c.Id == id && !c.IsDeleted);

        if (_requestContext.TenantId.HasValue)
        {
            query = query.Where(c => c.TenantId == _requestContext.TenantId.Value);
        }

        if (_requestContext.ActiveStoreId.HasValue)
        {
            query = query.Where(c => c.StoreId == _requestContext.ActiveStoreId.Value);
        }

        return await query.Select(c => MapToDto(c)).FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateCompanyRequest company)
    {
        var storeId = _requestContext.ActiveStoreId
            ?? throw new InvalidOperationException("Active store is required to create a company.");

        var entity = new Company
        {
            TenantId = company.TenantId,
            StoreId = storeId,
            Name = company.Name,
            ContactName = company.ContactName,
            Phone = company.Phone,
            Email = company.Email,
            Address = company.Address,
            Gstin = company.Gstin,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Companies.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateCompanyRequest company)
    {
        var entity = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (entity == null) return;

        entity.Name = company.Name;
        entity.ContactName = company.ContactName;
        entity.Phone = company.Phone;
        entity.Email = company.Email;
        entity.Address = company.Address;
        entity.Gstin = company.Gstin;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Companies.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Companies.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<PagedResult<CustomerSaleHistoryDto>> GetSalesHistoryAsync(int companyId, PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Sales
            .Where(s => !s.IsDeleted && s.CompanyId == companyId);

        baseQuery = (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("createdat", false) => baseQuery.OrderBy(s => s.CreatedAt),
            ("createdat", true) => baseQuery.OrderByDescending(s => s.CreatedAt),
            ("totalamount", false) => baseQuery.OrderBy(s => s.TotalAmount),
            ("totalamount", true) => baseQuery.OrderByDescending(s => s.TotalAmount),
            (_, true) => baseQuery.OrderByDescending(s => s.Id),
            _ => baseQuery.OrderByDescending(s => s.CreatedAt),
        };

        var totalCount = await baseQuery.CountAsync();
        var sales = await baseQuery
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(s => new { s.Id, s.CreatedAt, s.SubtotalAmount, s.TaxAmount, s.DiscountAmount, s.TotalAmount, s.PaymentMethod })
            .ToListAsync();

        var saleIds = sales.Select(s => s.Id).ToList();
        var items = await _context.SaleItems
            .Where(i => !i.IsDeleted && saleIds.Contains(i.SaleId))
            .Join(_context.ProductVariants.Where(v => !v.IsDeleted),
                i => i.ProductVariantId,
                v => v.Id,
                (i, v) => new { i, v })
            .Join(_context.Products.Where(p => !p.IsDeleted),
                x => x.v.ProductId,
                p => p.Id,
                (x, p) => new
                {
                    x.i.SaleId,
                    Item = new SaleItemDetailDto
                    {
                        Id = x.i.Id,
                        ProductVariantId = x.i.ProductVariantId,
                        VariantName = x.v.VariantName,
                        SkuCode = x.v.SkuCode,
                        ProductName = p.Name,
                        Quantity = x.i.Quantity,
                        UnitPrice = x.i.UnitPrice,
                        TotalPrice = x.i.TotalPrice
                    }
                })
            .ToListAsync();

        var itemsBySale = items.GroupBy(i => i.SaleId).ToDictionary(g => g.Key, g => g.Select(x => x.Item).ToList());

        var resultItems = sales.Select(s => new CustomerSaleHistoryDto
        {
            Id = s.Id,
            CreatedAt = s.CreatedAt,
            SubtotalAmount = s.SubtotalAmount,
            TaxAmount = s.TaxAmount,
            DiscountAmount = s.DiscountAmount,
            TotalAmount = s.TotalAmount,
            PaymentMethod = s.PaymentMethod.ToString(),
            Items = itemsBySale.TryGetValue(s.Id, out var saleItems) ? saleItems : new List<SaleItemDetailDto>()
        }).ToList();

        return new PagedResult<CustomerSaleHistoryDto>
        {
            Items = resultItems,
            Page = q.Page,
            PageSize = q.PageSize,
            TotalCount = totalCount
        };
    }

    private static CompanyDto MapToDto(Company c) => new()
    {
        Id = c.Id,
        TenantId = c.TenantId,
        StoreId = c.StoreId,
        Name = c.Name,
        ContactName = c.ContactName,
        Phone = c.Phone,
        Email = c.Email,
        Address = c.Address,
        Gstin = c.Gstin,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt,
        IsDeleted = c.IsDeleted,
        DeletedAt = c.DeletedAt
    };
}
