
using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        return await _context.Users
            .Where(u => !u.IsDeleted)
            .Select(u => new UserDto
            {
                Id = u.Id,
                TenantId = u.TenantId,
                RoleId = u.RoleId,
                StoreId = u.StoreId,
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                IsDeleted = u.IsDeleted,
                DeletedAt = u.DeletedAt
            })
            .ToListAsync();
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Where(u => u.Id == id && !u.IsDeleted)
            .Select(u => new UserDto
            {
                Id = u.Id,
                TenantId = u.TenantId,
                RoleId = u.RoleId,
                StoreId = u.StoreId,
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                IsDeleted = u.IsDeleted,
                DeletedAt = u.DeletedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateUserRequest user)
    {
        var entity = new User
        {
            TenantId = user.TenantId,
            RoleId = user.RoleId,
            StoreId = user.StoreId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateUserRequest user)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        if (entity == null) return;
        entity.RoleId = user.RoleId;
        entity.StoreId = user.StoreId;
        entity.Username = user.Username;
        entity.Email = user.Email;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        if (user == null) return;
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
