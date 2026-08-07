using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Authorization;
using StockDaddy.Application.DTOs;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Application.Services;

/// <summary>
/// Role-permission matrix and user role assignment for in-app RBAC management.
/// </summary>
public class RbacService
{
    private readonly ApplicationDbContext _context;

    public RbacService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RbacMatrixDto> GetMatrixAsync()
    {
        var permissions = await GetPermissionsAsync();
        var roles = await GetRolesWithPermissionsAsync();

        return new RbacMatrixDto
        {
            Permissions = permissions,
            Roles = roles
        };
    }

    public async Task<List<PermissionSummaryDto>> GetPermissionsAsync()
    {
        return await _context.Permissions
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Module)
            .ThenBy(p => p.Action)
            .Select(p => new PermissionSummaryDto
            {
                Id = p.Id,
                Module = p.Module,
                Action = p.Action.ToString(),
                Key = PermissionKeys.Format(p.Module, p.Action)
            })
            .ToListAsync();
    }

    public async Task<List<RoleWithPermissionsDto>> GetRolesWithPermissionsAsync()
    {
        var roles = await _context.Roles
            .Where(r => !r.IsDeleted)
            .Include(r => r.RolePermissions)
            .OrderBy(r => r.Name)
            .ToListAsync();

        return roles.Select(r => new RoleWithPermissionsDto
        {
            Id = r.Id,
            Name = r.Name,
            PermissionIds = r.RolePermissions?
                .Where(rp => !rp.IsDeleted)
                .Select(rp => rp.PermissionId)
                .ToList() ?? []
        }).ToList();
    }

    public async Task<RoleWithPermissionsDto?> UpdateRolePermissionsAsync(int roleId, UpdateRolePermissionsRequest request)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == roleId && !r.IsDeleted);

        if (role == null)
        {
            return null;
        }

        var validPermissionIds = await _context.Permissions
            .Where(p => !p.IsDeleted && request.PermissionIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();

        var now = DateTime.UtcNow;
        var desired = validPermissionIds.ToHashSet();

        foreach (var mapping in role.RolePermissions!.Where(rp => !rp.IsDeleted))
        {
            if (!desired.Contains(mapping.PermissionId))
            {
                mapping.IsDeleted = true;
                mapping.DeletedAt = now;
                mapping.UpdatedAt = now;
            }
        }

        var currentIds = role.RolePermissions!
            .Where(rp => !rp.IsDeleted)
            .Select(rp => rp.PermissionId)
            .ToHashSet();

        foreach (var permissionId in desired.Where(id => !currentIds.Contains(id)))
        {
            role.RolePermissions!.Add(new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            });
        }

        await _context.SaveChangesAsync();

        return new RoleWithPermissionsDto
        {
            Id = role.Id,
            Name = role.Name,
            PermissionIds = desired.ToList()
        };
    }

    public async Task<UserDto?> AssignUserRoleAsync(int userId, AssignUserRoleRequest request)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        if (user == null)
        {
            return null;
        }

        var roleExists = await _context.Roles.AnyAsync(r => r.Id == request.RoleId && !r.IsDeleted);
        if (!roleExists)
        {
            return null;
        }

        user.RoleId = request.RoleId;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var permissions = await GetPermissionKeysForRoleAsync(user.RoleId);

        return new UserDto
        {
            Id = user.Id,
            TenantId = user.TenantId,
            RoleId = user.RoleId,
            RoleName = user.Role?.Name ?? string.Empty,
            StoreId = user.StoreId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsDeleted = user.IsDeleted,
            DeletedAt = user.DeletedAt,
            Permissions = permissions
        };
    }

    public async Task<List<string>> GetPermissionKeysForRoleAsync(int roleId)
    {
        var mappings = await _context.RolePermissions
            .Where(rp => !rp.IsDeleted && rp.RoleId == roleId)
            .Include(rp => rp.Permission)
            .ToListAsync();

        return mappings
            .Where(rp => rp.Permission != null && !rp.Permission.IsDeleted)
            .Select(rp => PermissionKeys.Format(rp.Permission!.Module, rp.Permission.Action))
            .Distinct()
            .OrderBy(p => p)
            .ToList();
    }

    private static readonly HashSet<string> ProtectedRoleNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Admin", "Manager", "Cashier"
    };

    public async Task<(RoleWithPermissionsDto? Result, string? Error)> CreateRoleAsync(CreateRoleRequest request)
    {
        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return (null, "Role name is required.");
        }

        var exists = await _context.Roles.AnyAsync(r => !r.IsDeleted && r.Name.ToLower() == name.ToLower());
        if (exists)
        {
            return (null, $"Role '{name}' already exists.");
        }

        var now = DateTime.UtcNow;
        var role = new Role
        {
            Name = name,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();

        return (new RoleWithPermissionsDto
        {
            Id = role.Id,
            Name = role.Name,
            PermissionIds = []
        }, null);
    }

    public async Task<(RoleWithPermissionsDto? Result, string? Error)> UpdateRoleAsync(int roleId, UpdateRoleRequest request)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == roleId && !r.IsDeleted);

        if (role == null)
        {
            return (null, "Role not found.");
        }

        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return (null, "Role name is required.");
        }

        var duplicate = await _context.Roles.AnyAsync(r =>
            !r.IsDeleted && r.Id != roleId && r.Name.ToLower() == name.ToLower());
        if (duplicate)
        {
            return (null, $"Role '{name}' already exists.");
        }

        role.Name = name;
        role.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return (new RoleWithPermissionsDto
        {
            Id = role.Id,
            Name = role.Name,
            PermissionIds = role.RolePermissions?
                .Where(rp => !rp.IsDeleted)
                .Select(rp => rp.PermissionId)
                .ToList() ?? []
        }, null);
    }

    public async Task<(bool Success, string? Error)> DeleteRoleAsync(int roleId)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId && !r.IsDeleted);
        if (role == null)
        {
            return (false, "Role not found.");
        }

        if (ProtectedRoleNames.Contains(role.Name))
        {
            return (false, $"Built-in role '{role.Name}' cannot be deleted.");
        }

        var usersAssigned = await _context.Users.AnyAsync(u => !u.IsDeleted && u.RoleId == roleId);
        if (usersAssigned)
        {
            return (false, "Cannot delete a role that is assigned to users. Reassign users first.");
        }

        var now = DateTime.UtcNow;
        role.IsDeleted = true;
        role.DeletedAt = now;
        role.UpdatedAt = now;
        await _context.SaveChangesAsync();

        return (true, null);
    }
}
