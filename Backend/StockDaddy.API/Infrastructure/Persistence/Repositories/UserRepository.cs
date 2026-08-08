
using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Authorization;
using StockDaddy.Application.Helpers;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IRequestContext _requestContext;

    public UserRepository(ApplicationDbContext context, IRequestContext requestContext)
    {
        _context = context;
        _requestContext = requestContext;
    }

    public async Task<PagedResult<UserDto>> GetPagedAsync(PagedQuery query)
    {
        var q = RepositoryPaging.Normalize(query);
        var baseQuery = _context.Users.Where(u => !u.IsDeleted);

        if (_requestContext.TenantId.HasValue)
        {
            var tenantId = _requestContext.TenantId.Value;
            baseQuery = baseQuery.Where(u => u.TenantId == tenantId);
        }

        var storeFilter = QueryScope.ResolveStoreFilter(q, _requestContext);
        if (storeFilter.HasValue)
        {
            baseQuery = baseQuery.Where(u =>
                u.UserStores.Any(us => us.StoreId == storeFilter.Value) ||
                u.StoreId == storeFilter.Value);
        }

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

        var result = await RepositoryPaging.ExecuteAsync(projected, q);
        await EnrichStoreAssignmentsAsync(result.Items);
        return result;
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
        var dto = await _context.Users
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

        if (dto == null) return null;

        await EnrichStoreAssignmentsAsync([dto]);
        return dto;
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
        await SyncStoreAssignmentsAsync(
            entity.Id,
            entity.TenantId,
            user.RoleId,
            UserStoreAssignmentHelper.Normalize(user.StoreAssignments, user.StoreIds, user.RoleId, user.DefaultStoreId, user.StoreId),
            user.DefaultStoreId ?? user.StoreId);
    }

    public async Task UpdateAsync(int id, UpdateUserRequest user)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        if (entity == null) return;
        entity.RoleId = user.RoleId;
        entity.Username = user.Username;
        entity.Email = user.Email;
        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            entity.PasswordHash = PasswordHasher.Hash(user.PasswordHash);
        }
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(entity);
        await _context.SaveChangesAsync();
        await SyncStoreAssignmentsAsync(
            entity.Id,
            entity.TenantId,
            user.RoleId,
            UserStoreAssignmentHelper.Normalize(user.StoreAssignments, user.StoreIds, user.RoleId, user.DefaultStoreId, user.StoreId),
            user.DefaultStoreId ?? user.StoreId);
    }

    public async Task SyncStoreAssignmentsAsync(
        int userId,
        int tenantId,
        int fallbackRoleId,
        List<UserStoreAssignmentDto> assignments,
        int? defaultStoreId,
        int? updateFallbackRoleId = null)
    {
        var normalized = assignments
            .Where(a => a.StoreId > 0 && a.RoleId > 0)
            .GroupBy(a => a.StoreId)
            .Select(g => g.First())
            .ToList();

        if (normalized.Count > 0)
        {
            var storeIds = normalized.Select(a => a.StoreId).ToList();
            var validStoreIds = await _context.Stores
                .Where(s => !s.IsDeleted && s.TenantId == tenantId && storeIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();

            var roleIds = normalized.Select(a => a.RoleId).Distinct().ToList();
            var validRoleIds = await _context.Roles
                .Where(r => !r.IsDeleted && roleIds.Contains(r.Id))
                .Select(r => r.Id)
                .ToHashSetAsync();

            normalized = normalized
                .Where(a => validStoreIds.Contains(a.StoreId) && validRoleIds.Contains(a.RoleId))
                .ToList();
        }

        var resolvedDefault = defaultStoreId.HasValue && normalized.Any(a => a.StoreId == defaultStoreId.Value)
            ? defaultStoreId.Value
            : normalized.FirstOrDefault(a => a.IsDefault)?.StoreId
              ?? normalized.FirstOrDefault()?.StoreId;

        if (resolvedDefault.HasValue)
        {
            foreach (var assignment in normalized)
            {
                assignment.IsDefault = assignment.StoreId == resolvedDefault.Value;
            }
        }

        var existing = await _context.UserStores.Where(us => us.UserId == userId).ToListAsync();
        if (existing.Count > 0)
        {
            _context.UserStores.RemoveRange(existing);
        }

        foreach (var assignment in normalized)
        {
            await _context.UserStores.AddAsync(new UserStore
            {
                UserId = userId,
                StoreId = assignment.StoreId,
                RoleId = assignment.RoleId,
                IsDefault = assignment.IsDefault
            });
        }

        var user = await _context.Users.FirstAsync(u => u.Id == userId);
        if (updateFallbackRoleId.HasValue)
        {
            user.RoleId = updateFallbackRoleId.Value;
        }
        else if (fallbackRoleId > 0)
        {
            user.RoleId = fallbackRoleId;
        }

        user.StoreId = resolvedDefault;
        user.UpdatedAt = DateTime.UtcNow;
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

    private async Task EnrichStoreAssignmentsAsync(IReadOnlyList<UserDto> users)
    {
        if (users.Count == 0) return;

        var userIds = users.Select(u => u.Id).ToList();
        var assignments = await _context.UserStores
            .Where(us => userIds.Contains(us.UserId))
            .ToListAsync();

        var storeIds = assignments.Select(a => a.StoreId).Distinct().ToList();
        var roleIds = assignments.Select(a => a.RoleId).Distinct().ToList();
        var storeNames = await _context.Stores
            .Where(s => storeIds.Contains(s.Id))
            .Select(s => new { s.Id, s.Name })
            .ToDictionaryAsync(s => s.Id, s => s.Name);
        var roleNames = await _context.Roles
            .Where(r => roleIds.Contains(r.Id))
            .Select(r => new { r.Id, r.Name })
            .ToDictionaryAsync(r => r.Id, r => r.Name);

        foreach (var user in users)
        {
            var userAssignments = assignments.Where(a => a.UserId == user.Id).ToList();
            if (userAssignments.Count > 0)
            {
                user.StoreIds = userAssignments.Select(a => a.StoreId).ToList();
                user.DefaultStoreId = userAssignments.FirstOrDefault(a => a.IsDefault)?.StoreId ?? user.StoreId;
                user.StoreAssignments = userAssignments.Select(a => new UserStoreAssignmentDto
                {
                    StoreId = a.StoreId,
                    RoleId = a.RoleId,
                    IsDefault = a.IsDefault,
                    StoreName = storeNames.GetValueOrDefault(a.StoreId),
                    RoleName = roleNames.GetValueOrDefault(a.RoleId)
                }).ToList();
            }
            else if (user.StoreId.HasValue)
            {
                user.StoreIds = [user.StoreId.Value];
                user.DefaultStoreId = user.StoreId;
                user.StoreAssignments =
                [
                    new UserStoreAssignmentDto
                    {
                        StoreId = user.StoreId.Value,
                        RoleId = user.RoleId,
                        IsDefault = true,
                        StoreName = storeNames.GetValueOrDefault(user.StoreId.Value),
                        RoleName = roleNames.GetValueOrDefault(user.RoleId)
                    }
                ];
            }
        }
    }
}
