using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.Services;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

await MainAsync();

async Task MainAsync()
{

    // ===============================
    // 1. Add Controllers and AutoMapper
    // ===============================
    builder.Services.AddControllers();
    builder.Services.AddAutoMapper(typeof(Program));

    // ===============================
    // 2. Configure PostgreSQL DbContext
    // ===============================
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // ===============================
    // 3. Register Repositories
    // ===============================
    builder.Services.AddScoped<ITenantRepository, TenantRepository>();
    builder.Services.AddScoped<IStoreRepository, StoreRepository>();
    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<ISubcategoryRepository, SubcategoryRepository>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IHsnMasterRepository, HsnMasterRepository>();
    builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
    builder.Services.AddScoped<IProductTagRepository, ProductTagRepository>();
    builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
    builder.Services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
    builder.Services.AddScoped<IStockItemRepository, StockItemRepository>();
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    builder.Services.AddScoped<ISaleRepository, SaleRepository>();
    builder.Services.AddScoped<ISaleItemRepository, SaleItemRepository>();
    builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
    builder.Services.AddScoped<IGiftOptionRepository, GiftOptionRepository>();
    builder.Services.AddScoped<IProductBundleRepository, ProductBundleRepository>();
    builder.Services.AddScoped<IBundleItemRepository, BundleItemRepository>();
    builder.Services.AddScoped<IBundleSaleItemRepository, BundleSaleItemRepository>();
    builder.Services.AddScoped<IReturnRepository, ReturnRepository>();
    builder.Services.AddScoped<IRefundRepository, RefundRepository>();
    builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
    builder.Services.AddScoped<ITaxRegionRepository, TaxRegionRepository>();
    builder.Services.AddScoped<IProductRestockAlertRepository, ProductRestockAlertRepository>();
    builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
    builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
    builder.Services.AddScoped<IPurchaseItemRepository, PurchaseItemRepository>();
    builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
    builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
    builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
    builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
    builder.Services.AddScoped<IIntegrationEventRepository, IntegrationEventRepository>();
    builder.Services.AddScoped<IAdjustedInvoiceRepository, AdjustedInvoiceRepository>();

    // ===============================
    // 4. Register Services
    // ===============================
    builder.Services.AddScoped<TenantService>();
    builder.Services.AddScoped<StoreService>();
    builder.Services.AddScoped<RoleService>();
    builder.Services.AddScoped<UserService>();
    builder.Services.AddScoped<CategoryService>();
    builder.Services.AddScoped<SubcategoryService>();
    builder.Services.AddScoped<ProductService>();
    builder.Services.AddScoped<HsnMasterService>();
    builder.Services.AddScoped<ProductVariantService>();
    builder.Services.AddScoped<ProductTagService>();
    builder.Services.AddScoped<ProductImageService>();
    builder.Services.AddScoped<ProductAttributeService>();
    builder.Services.AddScoped<StockItemService>();
    builder.Services.AddScoped<CustomerService>();
    builder.Services.AddScoped<SaleService>();
    builder.Services.AddScoped<SaleItemService>();
    builder.Services.AddScoped<InvoiceService>();
    builder.Services.AddScoped<GiftOptionService>();
    builder.Services.AddScoped<ProductBundleService>();
    builder.Services.AddScoped<BundleItemService>();
    builder.Services.AddScoped<BundleSaleItemService>();
    builder.Services.AddScoped<ReturnService>();
    builder.Services.AddScoped<RefundService>();
    builder.Services.AddScoped<AuditLogService>();
    builder.Services.AddScoped<TaxRegionService>();
    builder.Services.AddScoped<ProductRestockAlertService>();
    builder.Services.AddScoped<SupplierService>();
    builder.Services.AddScoped<PurchaseOrderService>();
    builder.Services.AddScoped<PurchaseItemService>();
    builder.Services.AddScoped<PaymentService>();
    builder.Services.AddScoped<ShipmentService>();
    builder.Services.AddScoped<PermissionService>();
    builder.Services.AddScoped<RolePermissionService>();
    builder.Services.AddScoped<IntegrationEventService>();
    builder.Services.AddScoped<AdjustedInvoiceService>();


    // ===============================
    // 5. Swagger for API Docs
    // ===============================
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "StockDaddy API",
            Version = "v1"
        });
    });

    // ===============================
    // 6. CORS Policy
    // ===============================
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            policy => policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
    });

    // ===============================
    // 7. Build and Use Middleware
    // ===============================
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapControllers();
        await app.RunAsync();
        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthorization();
        app.MapControllers();
        await app.RunAsync();
    }
}
