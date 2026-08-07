using Microsoft.EntityFrameworkCore;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        if (!await context.Tenants.AnyAsync())
        {
            await SeedTenantDataAsync(context);
        }

        await PermissionSeeder.SeedAsync(context);
    }

    private static async Task SeedTenantDataAsync(ApplicationDbContext context)
    {
        var now = DateTime.UtcNow;

        var tenant = new Tenant
        {
            Name = "Default Tenant",
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();

        var store = new Store
        {
            TenantId = tenant.Id,
            Name = "Main Store",
            Location = "123 Commerce Street",
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        var roles = new[]
        {
            new Role { Name = "Admin", CreatedAt = now, UpdatedAt = now, IsDeleted = false },
            new Role { Name = "Manager", CreatedAt = now, UpdatedAt = now, IsDeleted = false },
            new Role { Name = "Cashier", CreatedAt = now, UpdatedAt = now, IsDeleted = false }
        };

        var hsn = new HsnMaster
        {
            HSNCode = "DEFAULT",
            Description = "Default HSN code for general merchandise",
            CGSTPercent = 9,
            SGSTPercent = 9,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        context.Stores.Add(store);
        context.Roles.AddRange(roles);
        context.HSNMasters.Add(hsn);
        await context.SaveChangesAsync();

        var category = new Category
        {
            TenantId = tenant.Id,
            StoreId = store.Id,
            Name = "General",
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var subcategories = new[]
        {
            new Subcategory
            {
                TenantId = tenant.Id,
                StoreId = store.Id,
                CategoryId = category.Id,
                Name = "Apparel",
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            },
            new Subcategory
            {
                TenantId = tenant.Id,
                StoreId = store.Id,
                CategoryId = category.Id,
                Name = "Electronics",
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            },
            new Subcategory
            {
                TenantId = tenant.Id,
                StoreId = store.Id,
                CategoryId = category.Id,
                Name = "Accessories",
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            }
        };

        context.Subcategories.AddRange(subcategories);
        await context.SaveChangesAsync();
    }
}
