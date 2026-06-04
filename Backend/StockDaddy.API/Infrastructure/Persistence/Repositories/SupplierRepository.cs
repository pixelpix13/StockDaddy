using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly ApplicationDbContext _context;

    public SupplierRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<List<SupplierDto>> GetAllAsync()
    {
        return await _context.Suppliers
            .Where(s => !s.IsDeleted)
            .Select(s => new SupplierDto
            {
                Id = s.Id,
                TenantId = s.TenantId,
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
        return await _context.Suppliers
            .Where(s => s.Id == id && !s.IsDeleted)
            .Select(s => new SupplierDto
            {
                Id = s.Id,
                TenantId = s.TenantId,
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
        var entity = new Supplier
        {
            TenantId = supplier.TenantId,
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
