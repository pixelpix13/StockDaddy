using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Infrastructure.Persistence;
using StockDaddy.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// ===========================
// 1. Add Application Services
// ===========================
builder.Services.AddControllers(); // Enables MVC API controllers

// ===========================
// 2. Configure DbContext
// ===========================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// ===========================
// 3. Register Repositories
// ===========================
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Repeat for other repositories:
// builder.Services.AddScoped<IProductRepository, ProductRepository>();
// builder.Services.AddScoped<ISaleRepository, SaleRepository>();
// ...

// ===========================
// 4. Swagger (OpenAPI)
// ===========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StockDaddy API", Version = "v1" });
});

// ===========================
// 5. CORS (Optional for React Frontend)
// ===========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// ===========================
// 6. Build App
// ===========================
var app = builder.Build();

// ===========================
// 7. Use Middleware
// ===========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll"); // If using frontend like React

app.UseAuthorization();

app.MapControllers();

app.Run();
