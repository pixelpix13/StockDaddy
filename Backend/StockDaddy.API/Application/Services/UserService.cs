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

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<UserDto?> AddAsync(CreateUserRequest request)
    {
        await _repo.AddAsync(request);
        // Optionally, fetch the created user (e.g., by unique fields or by returning from repo)
        // Here, let's assume Username+Email is unique and fetch by those, or repo can return the created UserDto
        // For now, return null as placeholder if repo does not return the created user
        return null;
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserRequest request)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) return null;

        await _repo.UpdateAsync(id, request);
        // Fetch and return the updated user
        return await _repo.GetByIdAsync(id);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) return false;

        await _repo.SoftDeleteAsync(id);
        return true;
    }
}
