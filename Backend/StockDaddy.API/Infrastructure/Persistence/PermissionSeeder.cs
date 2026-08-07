using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Authorization;
using StockDaddy.Application.Helpers;
using StockDaddy.Domain.Entities;
using StockDaddy.Domain.Enums;

namespace StockDaddy.Infrastructure.Persistence;

/// <summary>
/// Idempotent seed for permissions, default role mappings, and bootstrap admin user.
/// </summary>
public static class PermissionSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var now = DateTime.UtcNow;

        await EnsurePermissionsAsync(context, now);
        await EnsureDefaultRoleMappingsAsync(context, now);
        await EnsureDefaultAdminUserAsync(context, now);
    }

    private static async Task EnsurePermissionsAsync(ApplicationDbContext context, DateTime now)
    {
        var existing = await context.Permissions
            .Where(p => !p.IsDeleted)
            .Select(p => new { p.Module, p.Action })
            .ToListAsync();

        var existingSet = existing
            .Select(p => PermissionKeys.Format(p.Module, p.Action))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toAdd = new List<Permission>();

        foreach (var module in PermissionKeys.Modules)
        {
            foreach (PermissionAction action in Enum.GetValues<PermissionAction>())
            {
                var key = PermissionKeys.Format(module, action);
                if (existingSet.Contains(key))
                {
                    continue;
                }

                toAdd.Add(new Permission
                {
                    Module = module,
                    Action = action,
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsDeleted = false
                });
            }
        }

        if (toAdd.Count > 0)
        {
            context.Permissions.AddRange(toAdd);
            await context.SaveChangesAsync();
        }
    }

    private static async Task EnsureDefaultRoleMappingsAsync(ApplicationDbContext context, DateTime now)
    {
        var roles = await context.Roles.Where(r => !r.IsDeleted).ToListAsync();
        if (roles.Count == 0)
        {
            return;
        }

        var permissions = await context.Permissions.Where(p => !p.IsDeleted).ToListAsync();
        var existingMappings = await context.RolePermissions
            .Where(rp => !rp.IsDeleted)
            .Select(rp => new { rp.RoleId, rp.PermissionId })
            .ToListAsync();

        var existingSet = existingMappings
            .Select(m => $"{m.RoleId}:{m.PermissionId}")
            .ToHashSet();

        var adminRole = roles.FirstOrDefault(r => r.Name == "Admin");
        var managerRole = roles.FirstOrDefault(r => r.Name == "Manager");
        var cashierRole = roles.FirstOrDefault(r => r.Name == "Cashier");

        var toAdd = new List<RolePermission>();

        if (adminRole != null)
        {
            foreach (var permission in permissions)
            {
                AddMapping(toAdd, existingSet, adminRole.Id, permission.Id, now);
            }
        }

        if (managerRole != null)
        {
            foreach (var permission in permissions.Where(p =>
                         p.Module is not ("Users" or "AccessControl" or "Settings" or "BillAdjustment" or "Activity")))
            {
                AddMapping(toAdd, existingSet, managerRole.Id, permission.Id, now);
            }

            foreach (var permission in permissions.Where(p =>
                         p.Module == "Activity" && p.Action == PermissionAction.Read))
            {
                AddMapping(toAdd, existingSet, managerRole.Id, permission.Id, now);
            }

            foreach (var permission in permissions.Where(p =>
                         p.Module == "Credit"))
            {
                AddMapping(toAdd, existingSet, managerRole.Id, permission.Id, now);
            }

            foreach (var permission in permissions.Where(p =>
                         p.Module is "Dashboard" or "Catalog" or "Product" or "Inventory" or "Sales" or "Purchase" or "Supplier" or "Customer"
                         && p.Action == PermissionAction.Read))
            {
                AddMapping(toAdd, existingSet, managerRole.Id, permission.Id, now);
            }
        }

        if (cashierRole != null)
        {
            var cashierModules = new[] { "Dashboard", "Product", "Inventory", "Sales", "Customer" };
            foreach (var permission in permissions.Where(p =>
                         cashierModules.Contains(p.Module) &&
                         p.Action is PermissionAction.Read or PermissionAction.Write))
            {
                AddMapping(toAdd, existingSet, cashierRole.Id, permission.Id, now);
            }
        }

        if (toAdd.Count > 0)
        {
            context.RolePermissions.AddRange(toAdd);
            await context.SaveChangesAsync();
        }
    }

    private static void AddMapping(
        List<RolePermission> toAdd,
        HashSet<string> existingSet,
        int roleId,
        int permissionId,
        DateTime now)
    {
        var key = $"{roleId}:{permissionId}";
        if (existingSet.Contains(key))
        {
            return;
        }

        toAdd.Add(new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        });
        existingSet.Add(key);
    }

    private static async Task EnsureDefaultAdminUserAsync(ApplicationDbContext context, DateTime now)
    {
        if (await context.Users.AnyAsync(u => !u.IsDeleted))
        {
            return;
        }

        var tenant = await context.Tenants.FirstOrDefaultAsync(t => !t.IsDeleted);
        var store = await context.Stores.FirstOrDefaultAsync(s => !s.IsDeleted);
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => !r.IsDeleted && r.Name == "Admin");

        if (tenant == null || adminRole == null)
        {
            return;
        }

        context.Users.Add(new User
        {
            TenantId = tenant.Id,
            StoreId = store?.Id,
            RoleId = adminRole.Id,
            Username = "admin",
            Email = "admin@stockdaddy.local",
            PasswordHash = PasswordHasher.Hash("Admin@123"),
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        });

        await context.SaveChangesAsync();
    }
}
