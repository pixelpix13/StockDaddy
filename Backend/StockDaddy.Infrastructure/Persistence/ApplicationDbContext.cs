using Microsoft.EntityFrameworkCore;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // ========== DbSets for Each Table ==========
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Subcategory> Subcategories => Set<Subcategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<HsnMaster> HSNMasters => Set<HsnMaster>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductTag> ProductTags => Set<ProductTag>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
    public DbSet<StockItem> StockItems => Set<StockItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<GiftOption> GiftOptions => Set<GiftOption>();
    public DbSet<ProductBundle> ProductBundles => Set<ProductBundle>();
    public DbSet<BundleItem> BundleItems => Set<BundleItem>();
    public DbSet<BundleSaleItem> BundleSaleItems => Set<BundleSaleItem>();
    public DbSet<Return> Returns => Set<Return>();
    public DbSet<Refund> Refunds => Set<Refund>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<TaxRegion> TaxRegions => Set<TaxRegion>();
    public DbSet<ProductRestockAlert> ProductRestockAlerts => Set<ProductRestockAlert>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<IntegrationEvent> IntegrationEvents => Set<IntegrationEvent>();
    public DbSet<AdjustedInvoice> AdjustedInvoices => Set<AdjustedInvoice>();
    public DbSet<HsnMaster> HsnMaster => Set<HsnMaster>();

    // ========== Fluent Configuration ==========
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Optional: Add indexes, constraints, unique rules, enum conversions, etc.

        // Example:
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Name);

        modelBuilder.Entity<HsnMaster>()
            .HasIndex(h => h.HSNCode)
            .IsUnique();

        // Add enum conversions if needed here
    }
}
