using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

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
            Username = u.Username,
            Email = u.Email
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
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task AddAsync(CreateUserRequest request)
    {
        var hashedPassword = HashPassword(request.Password);

        var user = new User
        {
            TenantId = request.TenantId,
            RoleId = request.RoleId,
            Username = request.Username,
            Email = request.Email,
            PasswordHash = hashedPassword,
        };

        await _repo.AddAsync(user);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) return false;

        user.Username = request.Username;
        user.Email = request.Email;
        user.RoleId = request.RoleId;
        user.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(user);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }

    private static string HashPassword(string password)
    {
        // For now use SHA256 (not ideal in production; use BCrypt or ASP.NET Identity)
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
