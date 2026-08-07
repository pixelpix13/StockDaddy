// StockDaddy API entry point — DI registration, JWT auth, CORS, Swagger, DB seed.
// See README.md in this folder for architecture overview.
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StockDaddy.API.Services;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.Services;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Infrastructure.Persistence.Repositories;
using StockDaddy.Infrastructure.Repositories;
using StockDaddy.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FeatureOptions>(builder.Configuration.GetSection("Features"));

// ===============================
// 1. Add Controllers and AutoMapper
// ===============================
builder.Services.AddControllers(options =>
    {
        options.Filters.Add<StockDaddy.Application.Authorization.PermissionAuthorizationFilter>();
        options.Filters.Add<StockDaddy.Application.Authorization.ActivityAuditFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddAutoMapper(typeof(Program));

// ===============================
// 2. Configure PostgreSQL DbContext
// ===============================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

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
builder.Services.AddScoped<ICreditLedgerRepository, CreditLedgerRepository>();
builder.Services.AddScoped<IScheduledPriceRevertRepository, ScheduledPriceRevertRepository>();

// ===============================
// 4. Register Services
// ===============================
builder.Services.AddScoped<IAuthService, AuthService>();
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
builder.Services.AddScoped<ScheduledPriceRevertService>();
builder.Services.AddScoped<OrchestrationService>();
builder.Services.AddScoped<RbacService>();
builder.Services.AddHostedService<ScheduledPriceRevertBackgroundService>();

// ===============================
// 5. Configure JWT Authentication
// ===============================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secret = jwtSettings["Secret"] ?? "StockDaddy_Super_Secret_Key_For_JWT_Authentication_2026_Minimum_32_Chars!";
var key = Encoding.UTF8.GetBytes(secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "StockDaddyAPI",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "StockDaddyClient",
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// ===============================
// 6. Swagger for API Docs
// ===============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StockDaddy API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ===============================
// 7. CORS Policy
// ===============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// ===============================
// 8. Build and Use Middleware
// ===============================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.InitializeAsync(context);
}

await app.RunAsync();
