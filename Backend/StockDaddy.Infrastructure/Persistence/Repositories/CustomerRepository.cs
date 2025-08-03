using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .Where(c => !c.IsDeleted)
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return;

        customer.IsDeleted = true;
        customer.DeletedAt = DateTime.UtcNow;
        customer.UpdatedAt = DateTime.UtcNow;

        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }
}
