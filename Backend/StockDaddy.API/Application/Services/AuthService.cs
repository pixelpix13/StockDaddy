using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StockDaddy.Application.Authorization;
using StockDaddy.Application.Helpers;
using StockDaddy.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Application.Services;

/// <summary>
/// Issues JWT tokens on login/register and validates credentials against Users table.
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly RbacService _rbacService;

    public AuthService(ApplicationDbContext context, IConfiguration configuration, RbacService rbacService)
    {
        _context = context;
        _configuration = configuration;
        _rbacService = rbacService;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var normalizedInput = request.UsernameOrEmail.Trim().ToLower();

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => !u.IsDeleted && 
                (u.Email.ToLower() == normalizedInput || u.Username.ToLower() == normalizedInput));

        if (user == null)
        {
            return null;
        }

        if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _context.Users
            .AnyAsync(u => !u.IsDeleted && 
                (u.Email.ToLower() == request.Email.Trim().ToLower() || u.Username.ToLower() == request.Username.Trim().ToLower()));

        if (existingUser)
        {
            return null;
        }

        var user = new User
        {
            TenantId = request.TenantId,
            RoleId = request.RoleId,
            StoreId = request.DefaultStoreId ?? request.StoreId,
            Username = request.Username.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = PasswordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        await SyncUserStoresOnRegisterAsync(user, request);

        var role = await _context.Roles.FindAsync(user.RoleId);
        user.Role = role;

        return await GenerateAuthResponseAsync(user);
    }

    private async Task SyncUserStoresOnRegisterAsync(User user, RegisterRequest request)
    {
        var ids = request.StoreIds.Where(id => id > 0).Distinct().ToList();
        if (ids.Count == 0 && request.StoreId.HasValue)
        {
            ids = [request.StoreId.Value];
        }

        if (ids.Count > 0)
        {
            ids = await _context.Stores
                .Where(s => !s.IsDeleted && s.TenantId == user.TenantId && ids.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync();
        }

        int? resolvedDefault = request.DefaultStoreId.HasValue && ids.Contains(request.DefaultStoreId.Value)
            ? request.DefaultStoreId.Value
            : request.StoreId.HasValue && ids.Contains(request.StoreId.Value)
                ? request.StoreId
                : ids.Count > 0 ? ids[0] : null;

        if (resolvedDefault.HasValue)
        {
            user.StoreId = resolvedDefault;
            user.UpdatedAt = DateTime.UtcNow;
        }

        foreach (var storeId in ids)
        {
            await _context.UserStores.AddAsync(new UserStore
            {
                UserId = user.Id,
                StoreId = storeId,
                RoleId = request.RoleId,
                IsDefault = resolvedDefault.HasValue && storeId == resolvedDefault.Value
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<UserDto?> GetCurrentUserAsync(int userId)
    {
        var response = await RefreshSessionAsync(userId);
        return response?.User;
    }

    public async Task<AuthResponse?> RefreshSessionAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        if (user == null)
        {
            return null;
        }

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse?> SwitchStoreAsync(int userId, int storeId)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        if (user == null)
        {
            return null;
        }

        var permissions = await _rbacService.GetPermissionKeysForRoleAsync(user.RoleId);
        var canAccessAll = PermissionKeys.GrantsAccessToAllStores(user.Role?.Name, permissions);

        if (!canAccessAll)
        {
            var allowed = await GetAllowedStoreIdsAsync(user);
            if (!allowed.Contains(storeId))
            {
                return null;
            }
        }
        else
        {
            var storeExists = await _context.Stores.AnyAsync(s =>
                s.Id == storeId && s.TenantId == user.TenantId && !s.IsDeleted);
            if (!storeExists)
            {
                return null;
            }
        }

        user.StoreId = storeId;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await GenerateAuthResponseAsync(user, storeId);
    }

    public async Task<List<UserStoreOptionDto>> GetUserStoresAsync(int userId, int? activeStoreId = null)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        if (user == null)
        {
            return [];
        }

        var permissions = await _rbacService.GetPermissionKeysForRoleAsync(user.RoleId);
        var canAccessAll = PermissionKeys.GrantsAccessToAllStores(user.Role?.Name, permissions);

        var defaultStoreId = user.StoreId;
        List<UserStoreOptionDto> stores;

        if (canAccessAll)
        {
            var assignmentRoles = await _context.UserStores
                .Where(us => us.UserId == userId)
                .Join(_context.Roles.Where(r => !r.IsDeleted),
                    us => us.RoleId,
                    r => r.Id,
                    (us, r) => new { us.StoreId, us.IsDefault, r.Id, r.Name })
                .ToDictionaryAsync(x => x.StoreId);

            stores = await _context.Stores
                .Where(s => !s.IsDeleted && s.TenantId == user.TenantId)
                .OrderBy(s => s.Name)
                .Select(s => new UserStoreOptionDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Location = s.Location,
                    IsDefault = defaultStoreId.HasValue && s.Id == defaultStoreId.Value
                })
                .ToListAsync();

            foreach (var store in stores)
            {
                if (assignmentRoles.TryGetValue(store.Id, out var assigned))
                {
                    store.RoleId = assigned.Id;
                    store.RoleName = assigned.Name;
                    store.IsDefault = assigned.IsDefault;
                }
                else
                {
                    store.RoleId = user.RoleId;
                    store.RoleName = user.Role?.Name ?? string.Empty;
                }
            }
        }
        else
        {
            stores = await _context.UserStores
                .Where(us => us.UserId == userId)
                .Join(_context.Stores.Where(s => !s.IsDeleted && s.TenantId == user.TenantId),
                    us => us.StoreId,
                    s => s.Id,
                    (us, s) => new { us, s })
                .Join(_context.Roles.Where(r => !r.IsDeleted),
                    x => x.us.RoleId,
                    r => r.Id,
                    (x, r) => new UserStoreOptionDto
                    {
                        Id = x.s.Id,
                        Name = x.s.Name,
                        Location = x.s.Location,
                        RoleId = r.Id,
                        RoleName = r.Name,
                        IsDefault = x.us.IsDefault
                    })
                .OrderBy(s => s.Name)
                .ToListAsync();

            if (stores.Count == 0 && user.StoreId.HasValue)
            {
                var fallbackRoleName = user.Role?.Name ?? string.Empty;
                var legacyStore = await _context.Stores
                    .Where(s => s.Id == user.StoreId.Value && !s.IsDeleted)
                    .Select(s => new { s.Id, s.Name, s.Location })
                    .FirstOrDefaultAsync();
                if (legacyStore != null)
                {
                    stores =
                    [
                        new UserStoreOptionDto
                        {
                            Id = legacyStore.Id,
                            Name = legacyStore.Name,
                            Location = legacyStore.Location,
                            RoleId = user.RoleId,
                            RoleName = fallbackRoleName,
                            IsDefault = true
                        }
                    ];
                }
            }
        }

        var active = activeStoreId ?? user.StoreId ?? stores.FirstOrDefault(s => s.IsDefault)?.Id ?? stores.FirstOrDefault()?.Id;
        foreach (var store in stores)
        {
            store.IsActive = store.Id == active;
        }

        return stores;
    }

    private async Task<List<int>> GetAllowedStoreIdsAsync(User user)
    {
        var assigned = await _context.UserStores
            .Where(us => us.UserId == user.Id)
            .Join(_context.Stores.Where(s => !s.IsDeleted && s.TenantId == user.TenantId),
                us => us.StoreId,
                s => s.Id,
                (us, s) => s.Id)
            .Distinct()
            .ToListAsync();

        if (assigned.Count > 0)
        {
            return assigned;
        }

        if (user.StoreId.HasValue)
        {
            return [user.StoreId.Value];
        }

        return [];
    }

    private async Task<(List<int> StoreIds, List<UserStoreOptionDto> AssignedStores, List<UserStoreAssignmentDto> StoreAssignments, int? DefaultStoreId)> LoadStoreAssignmentsAsync(User user)
    {
        var permissions = await _rbacService.GetPermissionKeysForRoleAsync(user.RoleId);
        var canAccessAll = PermissionKeys.GrantsAccessToAllStores(user.Role?.Name, permissions);

        if (canAccessAll)
        {
            var assignmentRoles = await _context.UserStores
                .Where(us => us.UserId == user.Id)
                .Join(_context.Roles.Where(r => !r.IsDeleted),
                    us => us.RoleId,
                    r => r.Id,
                    (us, r) => new { us.StoreId, us.IsDefault, r.Id, r.Name })
                .ToDictionaryAsync(x => x.StoreId);

            var allStores = await _context.Stores
                .Where(s => !s.IsDeleted && s.TenantId == user.TenantId)
                .OrderBy(s => s.Name)
                .Select(s => new UserStoreOptionDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Location = s.Location,
                    IsDefault = user.StoreId.HasValue && s.Id == user.StoreId.Value
                })
                .ToListAsync();

            foreach (var store in allStores)
            {
                if (assignmentRoles.TryGetValue(store.Id, out var storeRole))
                {
                    store.RoleId = storeRole.Id;
                    store.RoleName = storeRole.Name;
                    store.IsDefault = storeRole.IsDefault;
                }
                else
                {
                    store.RoleId = user.RoleId;
                    store.RoleName = user.Role?.Name ?? string.Empty;
                }
            }

            if (allStores.Count > 0 && !allStores.Any(s => s.IsDefault))
            {
                allStores[0].IsDefault = true;
            }

            var storeAssignments = allStores
                .Where(s => assignmentRoles.ContainsKey(s.Id))
                .Select(s => new UserStoreAssignmentDto
                {
                    StoreId = s.Id,
                    RoleId = s.RoleId,
                    RoleName = s.RoleName,
                    StoreName = s.Name,
                    IsDefault = s.IsDefault
                })
                .ToList();

            return (allStores.Select(s => s.Id).ToList(), allStores, storeAssignments, user.StoreId ?? allStores.FirstOrDefault(s => s.IsDefault)?.Id);
        }

        var assignedStores = await _context.UserStores
            .Where(us => us.UserId == user.Id)
            .Join(_context.Stores.Where(s => !s.IsDeleted && s.TenantId == user.TenantId),
                us => us.StoreId,
                s => s.Id,
                (us, s) => new { us, s })
            .Join(_context.Roles.Where(r => !r.IsDeleted),
                x => x.us.RoleId,
                r => r.Id,
                (x, r) => new UserStoreOptionDto
                {
                    Id = x.s.Id,
                    Name = x.s.Name,
                    Location = x.s.Location,
                    RoleId = r.Id,
                    RoleName = r.Name,
                    IsDefault = x.us.IsDefault
                })
            .OrderBy(s => s.Name)
            .ToListAsync();

        if (assignedStores.Count == 0 && user.StoreId.HasValue)
        {
            var fallbackRoleName = user.Role?.Name ?? string.Empty;
            var legacyStore = await _context.Stores
                .Where(s => s.Id == user.StoreId.Value && !s.IsDeleted)
                .Select(s => new { s.Id, s.Name, s.Location })
                .FirstOrDefaultAsync();
            if (legacyStore != null)
            {
                assignedStores =
                [
                    new UserStoreOptionDto
                    {
                        Id = legacyStore.Id,
                        Name = legacyStore.Name,
                        Location = legacyStore.Location,
                        RoleId = user.RoleId,
                        RoleName = fallbackRoleName,
                        IsDefault = true
                    }
                ];
            }
        }

        var assignments = assignedStores.Select(s => new UserStoreAssignmentDto
        {
            StoreId = s.Id,
            RoleId = s.RoleId,
            RoleName = s.RoleName,
            StoreName = s.Name,
            IsDefault = s.IsDefault
        }).ToList();

        var defaultId = assignedStores.FirstOrDefault(s => s.IsDefault)?.Id ?? user.StoreId ?? assignedStores.FirstOrDefault()?.Id;
        return (assignedStores.Select(s => s.Id).ToList(), assignedStores, assignments, defaultId);
    }

    private async Task<(int RoleId, Role? Role)> ResolveEffectiveRoleAsync(User user, int? activeStoreId)
    {
        if (activeStoreId.HasValue)
        {
            var assignment = await _context.UserStores
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.UserId == user.Id && us.StoreId == activeStoreId.Value);

            if (assignment != null)
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == assignment.RoleId && !r.IsDeleted);
                if (role != null)
                {
                    return (assignment.RoleId, role);
                }
            }
        }

        if (user.Role != null)
        {
            return (user.RoleId, user.Role);
        }

        var fallback = await _context.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId && !r.IsDeleted);
        return (user.RoleId, fallback);
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user, int? activeStoreIdOverride = null)
    {
        var assignments = await LoadStoreAssignmentsAsync(user);
        var activeStoreId = activeStoreIdOverride ?? user.StoreId ?? assignments.DefaultStoreId;
        var (effectiveRoleId, effectiveRole) = await ResolveEffectiveRoleAsync(user, activeStoreId);
        var permissions = await _rbacService.GetPermissionKeysForRoleAsync(effectiveRoleId);

        var jwtSettings = _configuration.GetSection("Jwt");
        var secret = jwtSettings["Secret"] ?? "StockDaddy_Super_Secret_Key_For_JWT_Authentication_2026_Minimum_32_Chars!";
        var issuer = jwtSettings["Issuer"] ?? "StockDaddyAPI";
        var audience = jwtSettings["Audience"] ?? "StockDaddyClient";
        var expiryMinutes = double.TryParse(jwtSettings["ExpiryMinutes"], out var mins) ? mins : 1440;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new("tenantId", user.TenantId.ToString()),
            new("roleId", effectiveRoleId.ToString())
        };

        if (activeStoreId.HasValue)
        {
            claims.Add(new Claim("storeId", activeStoreId.Value.ToString()));
        }

        if (effectiveRole != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, effectiveRole.Name));
        }

        foreach (var permission in permissions)
        {
            claims.Add(new Claim(PermissionKeys.ClaimType, permission));
        }

        var expiration = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponse
        {
            Token = tokenString,
            Expiration = expiration,
            User = new UserDto
            {
                Id = user.Id,
                TenantId = user.TenantId,
                RoleId = effectiveRoleId,
                RoleName = effectiveRole?.Name ?? string.Empty,
                StoreId = activeStoreId,
                DefaultStoreId = assignments.DefaultStoreId,
                StoreIds = assignments.StoreIds,
                AssignedStores = assignments.AssignedStores,
                StoreAssignments = assignments.StoreAssignments,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsDeleted = user.IsDeleted,
                DeletedAt = user.DeletedAt,
                Permissions = permissions
            }
        };
    }
}
