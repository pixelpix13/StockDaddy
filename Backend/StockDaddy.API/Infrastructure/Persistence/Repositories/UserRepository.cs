
using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Helpers;
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

    public async Task<PagedResult<UserDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Users.Where(u => !u.IsDeleted);

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = RepositoryPaging.LikePattern(q.Search);
            var idMatch = RepositoryPaging.TryParseSearchId(q.Search, out var searchId);
            baseQuery = baseQuery.Where(u =>
                (idMatch && u.Id == searchId) ||
                (idMatch && u.RoleId == searchId) ||
                EF.Functions.ILike(u.Username, pattern) ||
                EF.Functions.ILike(u.Email, pattern));
        }

        if (q.RoleId.HasValue)
        {
            baseQuery = baseQuery.Where(u => u.RoleId == q.RoleId.Value);
        }

        baseQuery = ApplySort(baseQuery, q);

        var projected = baseQuery.Select(u => new UserDto
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
            
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    private static IQueryable<User> ApplySort(IQueryable<User> query, PagedQuery q) =>
        (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("name", true) => query.OrderByDescending(u => u.Username),
            ("name", false) => query.OrderBy(u => u.Username),
            ("username", true) => query.OrderByDescending(u => u.Username),
            ("username", false) => query.OrderBy(u => u.Username),
            ("email", true) => query.OrderByDescending(u => u.Email),
            ("email", false) => query.OrderBy(u => u.Email),
            ("createdat", true) => query.OrderByDescending(u => u.CreatedAt),
            ("createdat", false) => query.OrderBy(u => u.CreatedAt),
            (_, true) => query.OrderByDescending(u => u.Id),
            _ => query.OrderBy(u => u.Id),
        };

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
            PasswordHash = string.IsNullOrWhiteSpace(user.PasswordHash)
                ? string.Empty
                : PasswordHasher.Hash(user.PasswordHash),
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
        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            entity.PasswordHash = PasswordHasher.Hash(user.PasswordHash);
        }
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
