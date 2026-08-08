using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Authorization;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Helpers;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IRequestContext _requestContext;

    public SupplierRepository(ApplicationDbContext context, IRequestContext requestContext)
    {
        _context = context;
        _requestContext = requestContext;
    }

    public async Task<PagedResult<SupplierDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Suppliers.Where(s => !s.IsDeleted);

        if (_requestContext.TenantId.HasValue)
        {
            baseQuery = baseQuery.Where(s => s.TenantId == _requestContext.TenantId.Value);
        }

        var storeFilter = QueryScope.ResolveStoreFilter(q, _requestContext);
        if (storeFilter.HasValue)
        {
            baseQuery = baseQuery.Where(s => s.StoreId == storeFilter.Value);
        }

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(s => EF.Functions.ILike(s.Name, pattern) || EF.Functions.ILike(s.Email, pattern) || EF.Functions.ILike(s.ContactName, pattern));
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(s => new SupplierDto
        {
            Id = s.Id,
            TenantId = s.TenantId,
            StoreId = s.StoreId,
            Name = s.Name,
            ContactName = s.ContactName,
            Phone = s.Phone,
            Email = s.Email,
            Address = s.Address,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
            IsDeleted = s.IsDeleted,
            DeletedAt = s.DeletedAt
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<Supplier> ApplySort(IQueryable<Supplier> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("name", true) => query.OrderByDescending(s => s.Name),
            ("name", false) => query.OrderBy(s => s.Name),
            ("email", true) => query.OrderByDescending(s => s.Email),
            ("email", false) => query.OrderBy(s => s.Email),
            ("createdat", true) => query.OrderByDescending(s => s.CreatedAt),
            ("createdat", false) => query.OrderBy(s => s.CreatedAt),
            (_, true) => query.OrderByDescending(s => s.Id),
            _ => query.OrderBy(s => s.Id),
        };

    public async Task<List<SupplierDto>> GetAllAsync()
    {
        var query = _context.Suppliers.Where(s => !s.IsDeleted);

        if (_requestContext.TenantId.HasValue)
        {
            query = query.Where(s => s.TenantId == _requestContext.TenantId.Value);
        }

        if (_requestContext.ActiveStoreId.HasValue)
        {
            query = query.Where(s => s.StoreId == _requestContext.ActiveStoreId.Value);
        }

        return await query
            .Select(s => new SupplierDto
            {
                Id = s.Id,
                TenantId = s.TenantId,
                StoreId = s.StoreId,
                Name = s.Name,
                ContactName = s.ContactName,
                Phone = s.Phone,
                Email = s.Email,
                Address = s.Address,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                IsDeleted = s.IsDeleted,
                DeletedAt = s.DeletedAt
            })
            .ToListAsync();
    }

    public async Task<SupplierDto?> GetByIdAsync(int id)
    {
        var query = _context.Suppliers.Where(s => s.Id == id && !s.IsDeleted);

        if (_requestContext.TenantId.HasValue)
        {
            query = query.Where(s => s.TenantId == _requestContext.TenantId.Value);
        }

        if (_requestContext.ActiveStoreId.HasValue)
        {
            query = query.Where(s => s.StoreId == _requestContext.ActiveStoreId.Value);
        }

        return await query
            .Select(s => new SupplierDto
            {
                Id = s.Id,
                TenantId = s.TenantId,
                StoreId = s.StoreId,
                Name = s.Name,
                ContactName = s.ContactName,
                Phone = s.Phone,
                Email = s.Email,
                Address = s.Address,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                IsDeleted = s.IsDeleted,
                DeletedAt = s.DeletedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateSupplierRequest supplier)
    {
        var storeId = _requestContext.ActiveStoreId
            ?? throw new InvalidOperationException("Active store is required to create a supplier.");

        var entity = new Supplier
        {
            TenantId = supplier.TenantId,
            StoreId = storeId,
            Name = supplier.Name,
            ContactName = supplier.ContactName,
            Phone = supplier.Phone,
            Email = supplier.Email,
            Address = supplier.Address,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Suppliers.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateSupplierRequest supplier)
    {
        var entity = await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (entity == null) return;
        entity.Name = supplier.Name;
        entity.ContactName = supplier.ContactName;
        entity.Phone = supplier.Phone;
        entity.Email = supplier.Email;
        entity.Address = supplier.Address;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Suppliers.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (entity == null) return;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Suppliers.Update(entity);
        await _context.SaveChangesAsync();
    }
}
