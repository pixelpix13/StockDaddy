using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<CustomerDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Customers.Where(c => !c.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(c => EF.Functions.ILike(c.Name, pattern) || EF.Functions.ILike(c.Email, pattern) || EF.Functions.ILike(c.Phone, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(c => new CustomerDto
        {
                Id = c.Id,
                TenantId = c.TenantId,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                Address = c.Address,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                IsDeleted = c.IsDeleted,
                DeletedAt = c.DeletedAt
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<Customer> ApplySort(IQueryable<Customer> query, PagedQuery q) =>
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

    public async Task<List<CustomerDto>> GetAllAsync()
    {
        return await _context.Customers
            .Where(c => !c.IsDeleted)
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                TenantId = c.TenantId,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                Address = c.Address,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                IsDeleted = c.IsDeleted,
                DeletedAt = c.DeletedAt
            })
            .ToListAsync();
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var c = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (c == null) return null;
        return new CustomerDto
        {
            Id = c.Id,
            TenantId = c.TenantId,
            Name = c.Name,
            Phone = c.Phone,
            Email = c.Email,
            Address = c.Address,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            IsDeleted = c.IsDeleted,
            DeletedAt = c.DeletedAt
        };
    }

    public async Task AddAsync(CreateCustomerRequest customer)
    {
        var entity = new Customer
        {
            TenantId = customer.TenantId,
            Name = customer.Name,
            Phone = customer.Phone,
            Email = customer.Email,
            Address = customer.Address,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Customers.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateCustomerRequest customer)
    {
        var entity = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (entity == null) return;

        entity.Name = customer.Name;
        entity.Phone = customer.Phone;
        entity.Email = customer.Email;
        entity.Address = customer.Address;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Customers.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var entity = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Customers.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<PagedResult<CustomerSaleHistoryDto>> GetSalesHistoryAsync(int customerId, PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Sales
            .Where(s => !s.IsDeleted && s.CustomerId == customerId);

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
}
