using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class UserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _repo.GetAllAsync();
        return users.Select(u => new UserDto
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
        }).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            TenantId = user.TenantId,
            RoleId = user.RoleId,
            StoreId = user.StoreId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsDeleted = user.IsDeleted,
            DeletedAt = user.DeletedAt
        };
    }

    public async Task AddAsync(CreateUserRequest request)
    {
        var user = new User
        {
            TenantId = request.TenantId,
            RoleId = request.RoleId,
            StoreId = request.StoreId,
            Username = request.Username,
            Email = request.Email,
            PasswordHash = request.PasswordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(user);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) return false;

        user.RoleId = request.RoleId;
        user.StoreId = request.StoreId;
        user.Username = request.Username;
        user.Email = request.Email;
        user.PasswordHash = request.PasswordHash;
        user.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(user);
        return true;
    }

    public async Task<bool> SoftDeleteAsync(Guid id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) return false;

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _repo.SoftDeleteAsync(id);
        return true;
    }
}
