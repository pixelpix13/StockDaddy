using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

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
}
