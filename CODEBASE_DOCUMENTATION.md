# StockDaddy — Complete Codebase Documentation
### A Learning Guide to .NET 9, Clean Architecture, and Professional API Development

> **Who this is for**: Developers learning how to build a real-world REST API with .NET. Every code block in this document is the *actual source code* from this project. Every line is explained — not just what it does, but *why* it was written that way and *what concepts it teaches*.

---

## Table of Contents

1. [What is StockDaddy?](#1-what-is-stockdaddy)
2. [The Big Picture: Clean Architecture](#2-the-big-picture-clean-architecture)
3. [Technology Stack](#3-technology-stack)
4. [Layer 1 — Domain: The Heart of the System](#4-layer-1--domain-the-heart-of-the-system)
5. [Layer 2 — Application: Business Logic and Contracts](#5-layer-2--application-business-logic-and-contracts)
6. [Layer 3 — Infrastructure: Database Access](#6-layer-3--infrastructure-database-access)
7. [Layer 4 — API: HTTP Entry Point](#7-layer-4--api-http-entry-point)
8. [Complete Request Flow Trace](#8-complete-request-flow-trace)
9. [Core C# Concepts Explained](#9-core-c-concepts-explained)
10. [Best Practices in This Codebase](#10-best-practices-in-this-codebase)
11. [Background Services](#11-background-services)
12. [The Full Entity Map](#12-the-full-entity-map)
13. [Common Patterns Reference](#13-common-patterns-reference)

---

## 1. What is StockDaddy?

StockDaddy is a **multi-tenant inventory management REST API**. "Multi-tenant" means many different businesses (tenants) share the same running application, but each business's data is completely isolated — Company A cannot see Company B's products, sales, or users.

**What it manages:**

| Domain Area | Entities |
|---|---|
| Organizations | Tenant, Store |
| People | User, Role, Permission, Customer, Supplier |
| Products | Product, ProductVariant, ProductBundle, ProductTag, ProductImage, ProductAttribute, Category, Subcategory, HsnMaster |
| Stock | StockItem, ProductRestockAlert |
| Selling | Sale, SaleItem, Invoice, GiftOption, BundleSaleItem |
| Buying | PurchaseOrder, PurchaseItem |
| After-sale | Return, Refund, AdjustedInvoice, Shipment |
| Finance | Payment, TaxRegion |
| System | AuditLog, IntegrationEvent, ScheduledPriceRevert |

---

## 2. The Big Picture: Clean Architecture

This application is built with **Clean Architecture** — a way of organizing code into layers where each layer has a single responsibility and dependencies only point inward.

> **Important**: StockDaddy uses a **single .NET project** (`StockDaddy.API`). The four architectural layers are enforced through **folder organization** and **namespace conventions** rather than separate `.csproj` files. This is a pragmatic monolith approach — the same Clean Architecture rules apply, but all code lives in one deployable unit.

```
┌─────────────────────────────────────────────────────────────────────┐
│  StockDaddy.API  (single .csproj — all layers live here as folders) │
│                                                                     │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  API Layer  — StockDaddy.API/Controllers/UserControllers/   │   │
│  │  ↳ Program.cs, Controllers, Background Services (BgServices)│   │
│  │  ↳ Receives HTTP requests, returns HTTP responses           │   │
│  └───────────────────────────┬─────────────────────────────────┘   │
│                              │ calls Services                       │
│  ┌───────────────────────────▼─────────────────────────────────┐   │
│  │  Application Layer  — StockDaddy.API/Application/           │   │
│  │  ↳ Services/, Interfaces/, DTOs/                            │   │
│  │  ↳ Orchestrates business logic                              │   │
│  └───────────────────────────┬─────────────────────────────────┘   │
│                              │ contracts implemented by             │
│  ┌───────────────────────────▼─────────────────────────────────┐   │
│  │  Infrastructure Layer  — StockDaddy.API/Infrastructure/     │   │
│  │  ↳ Persistence/Repositories/, Persistence/ (DbContext)      │   │
│  │  ↳ Migrations/                                              │   │
│  │  ↳ Does actual database work                                │   │
│  └─────────────────────────────────────────────────────────────┘   │
│             ▲  (all layers reference Domain)                        │
│  ┌──────────┴──────────────────────────────────────────────────┐   │
│  │  Domain Layer  — StockDaddy.API/Domain/                     │   │
│  │  ↳ Entities/, Enums/ (pure business objects)                │   │
│  │  ↳ Has ZERO external dependencies                           │   │
│  └─────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────┘
```

**Layer separation is enforced by namespaces, not project boundaries:**

| Folder | Namespace | What lives here |
|--------|-----------|------------------|
| `Domain/Entities/` | `StockDaddy.Domain.Entities` | Pure C# entity classes |
| `Domain/Enums/` | `StockDaddy.Domain.Enums` | Enum definitions |
| `Application/DTOs/` | `StockDaddy.Application.DTOs` | Request/response shapes |
| `Application/Interfaces/` | `StockDaddy.Application.Interfaces` | Repository contracts |
| `Application/Services/` | `StockDaddy.Application.Services` | Business orchestration |
| `Infrastructure/Persistence/` | `StockDaddy.Infrastructure.Persistence` | DbContext |
| `Infrastructure/Persistence/Repositories/` | `StockDaddy.Infrastructure.Repositories` | EF Core implementations |
| `Controllers/UserControllers/` | `StockDaddy.API.Controllers` | HTTP endpoints |
| `BgServices/` | `StockDaddy.API.Services` | Background/hosted services |

**Why this structure?**

1. **Domain is pure** — `Tenant.cs` knows nothing about databases, HTTP, or EF Core. It just describes what a Tenant is.
2. **Application defines contracts** — `ITenantRepository` says "I need these database operations" without knowing how they are implemented.
3. **Infrastructure fulfills contracts** — `TenantRepository` implements `ITenantRepository` using EF Core and PostgreSQL.
4. **API is just delivery** — Controllers translate between HTTP and your business logic. You could replace the API with a CLI, desktop app, or mobile app without changing any business logic.
5. **Easy to test** — You can replace `TenantRepository` with a fake in-memory version for unit tests.
6. **Single project simplicity** — One `.csproj` to build, one deployment artifact. The logical boundaries are maintained by namespaces and folder discipline, not build-time project references.

---

## 3. Technology Stack

| Technology | What It Is | Why We Use It |
|---|---|---|
| **.NET 9** | Microsoft's cross-platform framework | Fast, modern, cross-platform, massive ecosystem |
| **C#** | Statically typed, object-oriented language | Type-safe, expressive, excellent async support |
| **ASP.NET Core** | Web framework for REST APIs | Built-in DI, routing, middleware pipeline |
| **Entity Framework Core** | ORM — translates C# to SQL | Write queries in C#, no raw SQL, type-safe |
| **Npgsql** | PostgreSQL driver for EF Core | Connects EF Core to PostgreSQL |
| **PostgreSQL** | Relational database | Reliable, open-source, ACID compliant |
| **AutoMapper** | Object-to-object mapping | Reduces manual mapping boilerplate |
| **Swagger / OpenAPI** | API documentation + testing UI | Auto-generated docs from code |

---

## 4. Layer 1 — Domain: The Heart of the System

**Folder**: `Backend/StockDaddy.API/Domain/`  
**Namespaces**: `StockDaddy.Domain.Entities`, `StockDaddy.Domain.Enums`

The Domain layer contains your **business entities** — the "things" your system manages — and **enums** — fixed sets of values.

**Rule**: The Domain layer has NO `using` statements that reference Application, Infrastructure, or any external libraries. It knows nothing about EF Core, ASP.NET Core, or databases. Even though it shares a project with those layers, it is written as if it were completely isolated.

### 4.1 Entities

#### `Tenant.cs` — The Organization

```csharp
namespace StockDaddy.Domain.Entities;

public class Tenant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public ICollection<User>? Users { get; set; }
    public ICollection<Product>? Products { get; set; }
    public ICollection<Store>? Stores { get; set; }
}
```

**Line-by-line explanation:**

| Code | WHAT | WHY |
|---|---|---|
| `namespace StockDaddy.Domain.Entities;` | Declares which project/folder this belongs to | Namespaces prevent naming collisions — two classes can both be named `Tenant` in different namespaces |
| `public class Tenant` | Defines a new type called `Tenant` | `public` = accessible from all other projects. `class` = a reference type (stored on the heap, can be null) |
| `public int Id { get; set; }` | The primary key — uniquely identifies this record | `int` = 32-bit integer. `{ get; set; }` is an **auto-property** — C# generates a hidden backing field automatically. EF Core recognizes `Id` as the primary key by convention. |
| `public string Name { get; set; } = string.Empty;` | The tenant's name | `= string.Empty` initializes to `""` instead of `null`. Always initialize string properties to avoid null reference exceptions. |
| `public DateTime CreatedAt { get; set; } = DateTime.UtcNow;` | Timestamp of creation | `DateTime.UtcNow` is the current time in UTC (Universal Time). **Always store timestamps in UTC** — never local time — so your data is timezone-independent. |
| `public bool IsDeleted { get; set; } = false;` | Soft-delete flag | `bool` is true/false. This implements **soft delete** — instead of `DELETE FROM Tenants WHERE Id = 1`, we set this to `true`. The record stays in the database forever. |
| `public DateTime? DeletedAt { get; set; }` | When the tenant was deleted | The `?` after `DateTime` makes it **nullable** — it can be `null` (not deleted yet) or a date (when it was deleted). |
| `public ICollection<User>? Users { get; set; }` | List of users belonging to this tenant | `ICollection<T>` is an interface for a collection. The `?` means it could be null if not loaded. These are **navigation properties** — EF Core uses them to represent database relationships (JOINs). |

#### `User.cs` — Application User

```csharp
namespace StockDaddy.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int RoleId { get; set; }
    public int? StoreId { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation
    public Tenant? Tenant { get; set; }
    public Role? Role { get; set; }
    public Store? Store { get; set; }
}
```

**Key learning points:**

| Code | WHAT | WHY |
|---|---|---|
| `public int TenantId { get; set; }` | **Foreign key** to the Tenants table | Every user belongs to one tenant. This is how relational databases link tables. |
| `public int? StoreId { get; set; }` | Optional FK to Stores | `int?` — nullable integer. A user might work at a specific store, or they might be an admin with no store assignment. `?` makes this optional. |
| `public string PasswordHash { get; set; }` | The password stored as a hash | **Never store plain-text passwords.** A hash is a one-way transformation. If the database is stolen, attackers cannot recover passwords from a proper hash. |
| `public Tenant? Tenant { get; set; }` | Navigation property — loads the related Tenant | EF Core can populate this when you ask it to (`.Include(u => u.Tenant)`). The `?` means it might not be loaded. |

#### `Product.cs` — Inventory Item

```csharp
namespace StockDaddy.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int? StoreId { get; set; }
    public int? SubcategoryId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = "pcs";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation
    public Tenant? Tenant { get; set; }
    public Store? Store { get; set; }
    public Subcategory? Subcategory { get; set; }
}
```

Notice that `Product` does **not** have a price. That lives on `ProductVariant`. A product called "T-Shirt" can have variants: "Small/Red", "Large/Blue" — each with their own price, SKU, and stock quantity. This models real-world inventory correctly.

#### `ProductVariant.cs` — The Actual Sellable Item

```csharp
namespace StockDaddy.Domain.Entities;

public class ProductVariant
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int StoreId { get; set; }
    public int HSNCodeId { get; set; }

    public string VariantName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;

    public decimal CostPrice { get; set; }
    public decimal MarginPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;

    // Navigation
    public Product? Product { get; set; }
    public Store? Store { get; set; }
    public HsnMaster? HSNMaster { get; set; }
}
```

**Why `decimal` for prices?**

`decimal` is a precise decimal number type. **Never use `double` or `float` for money.** They use binary floating-point which cannot represent 0.1 exactly:

```csharp
double x = 0.1 + 0.2;    // x = 0.30000000000000004  ← floating-point error!
decimal y = 0.1m + 0.2m; // y = 0.3                  ← exact
```

The `m` suffix on a literal means decimal: `9.99m`.

#### `Sale.cs` — A Sales Transaction

```csharp
using StockDaddy.Domain.Enums;

namespace StockDaddy.Domain.Entities;

public class Sale
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int? StoreId { get; set; }
    public int? CustomerId { get; set; }
    public int SoldBy { get; set; }

    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;

    // Navigation
    public Tenant? Tenant { get; set; }
    public Store? Store { get; set; }
    public Customer? Customer { get; set; }
    public User? SoldByUser { get; set; }
}
```

### 4.2 Enums

Enums define a fixed set of named values. They prevent "magic strings" and "magic numbers".

#### `PermissionAction.cs`

```csharp
namespace StockDaddy.Domain.Enums;

public enum PermissionAction
{
    Read,
    Write,
    Update,
    Delete
}
```

**Why enums?**

```csharp
// ❌ BAD — magic string, easy to typo, no compile-time check
permission.Action = "reaD"; // typo compiles fine, breaks at runtime

// ✅ GOOD — enum, typo is a compile error
permission.Action = PermissionAction.Read; // compiler checks this
```

#### `PurchaseOrderStatus.cs`

```csharp
namespace StockDaddy.Domain.Enums;

public enum PurchaseOrderStatus
{
    Pending,
    Paid,
    Unpaid,
    Overdue,
    Cancelled,
    Delivered,
    Failed
}
```

Enums are stored as integers in the database (0, 1, 2...) by default. EF Core handles the conversion automatically.

---

## 5. Layer 2 — Application: Business Logic and Contracts

**Folder**: `Backend/StockDaddy.API/Application/`  
**Namespaces**: `StockDaddy.Application.DTOs`, `StockDaddy.Application.Interfaces`, `StockDaddy.Application.Services`

The Application layer is the brain. It:
- Defines **interfaces** (contracts) for what database operations exist
- Defines **DTOs** (Data Transfer Objects) for moving data safely between layers
- Contains **services** that orchestrate business workflows

### 5.1 Interfaces — The Contracts

An interface is a **promise without an implementation**. It says "any class that implements me MUST provide these methods." The Application layer defines interfaces so it can work with the database without knowing how the database actually works.

#### `ITenantRepository.cs`

```csharp
using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface ITenantRepository
{
    Task<List<TenantDto>> GetAllAsync();
    Task<TenantDto?> GetByIdAsync(int id);
    Task AddAsync(CreateTenantRequest tenant);
    Task UpdateAsync(int id, UpdateTenantRequest tenant);
    Task DeleteAsync(int id);
}
```

**Explanation:**

| Code | WHAT | WHY |
|---|---|---|
| `public interface ITenantRepository` | Defines a contract — a list of method signatures | `interface` has no method bodies, only signatures. The `I` prefix is a universal C# convention — at a glance you know it's an interface. |
| `Task<List<TenantDto>> GetAllAsync();` | Declares a method that returns a list of tenants asynchronously | `Task<T>` is the async return type. `List<TenantDto>` is what it returns when done. No `{` body `}` — interfaces declare, they do not implement. |
| `Task<TenantDto?> GetByIdAsync(int id);` | Find one tenant by ID, or null if not found | The `?` after `TenantDto` says "this might return null" — specifically when no tenant with that ID exists. |
| `Task AddAsync(CreateTenantRequest tenant);` | Create a new tenant | `Task` without `<T>` is async void — it completes but returns nothing. |

**Why define interfaces in Application and not Infrastructure?**

The Application layer defines the contract. Infrastructure implements it. This means:
- Application can work without Infrastructure
- You can have multiple Infrastructure implementations (PostgreSQL, MongoDB, in-memory for tests)
- Changing the database does not affect Application or API layers

### 5.2 DTOs — Data Transfer Objects

DTOs are plain data containers — no logic, just properties. They are the "shapes" that data takes when crossing layer boundaries.

#### `TenantDto.cs` — What the API returns

```csharp
namespace StockDaddy.Application.DTOs;

public class TenantDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

Notice what is **not** here: `IsDeleted`, `DeletedAt`, navigation properties. The DTO exposes only what the API consumer needs to see.

**Why not return the `Tenant` entity directly?**

1. **Security** — The `User` entity has `PasswordHash`. You never want that in a JSON response.
2. **Circular references** — `Tenant` has `Users`, and `User` has `Tenant`. Serializing this to JSON would cause an infinite loop.
3. **Stability** — You can change your database schema without breaking the API contract.
4. **Clarity** — The DTO is an explicit declaration of "this is what the API returns."

#### `CreateTenantRequest.cs` — What the API accepts

```csharp
namespace StockDaddy.Application.DTOs;

public class CreateTenantRequest
{
    public string Name { get; set; } = string.Empty;
}
```

Only `Name` is accepted from the client. The server sets `Id`, `CreatedAt`, `UpdatedAt`, `IsDeleted` itself — the client has no business providing those.

### 5.3 Services — Business Orchestration

Services are where business logic lives. They coordinate multiple repository calls and enforce business rules.

#### `TenantService.cs`

```csharp
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;

namespace StockDaddy.Application.Services;

public class TenantService
{
    private readonly ITenantRepository _repo;

    public TenantService(ITenantRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TenantDto>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<TenantDto?> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<TenantDto?> UpdateAsync(int id, UpdateTenantRequest request)
    {
        var tenant = await _repo.GetByIdAsync(id);
        if (tenant == null) return null;      // guard clause — fail fast

        await _repo.UpdateAsync(id, request);
        return await _repo.GetByIdAsync(id);  // return updated data
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tenant = await _repo.GetByIdAsync(id);
        if (tenant == null) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}
```

**Explanation:**

| Code | WHAT | WHY |
|---|---|---|
| `private readonly ITenantRepository _repo;` | Stores the repository reference | `private` = only this class uses it. `readonly` = set once in constructor, never changed. `ITenantRepository` = the interface (abstraction), not the concrete class. |
| `public TenantService(ITenantRepository repo)` | **Constructor injection** | The DI container sees "TenantService needs an ITenantRepository" and provides the registered implementation (`TenantRepository`) automatically. |
| `_repo = repo;` | Store the injected dependency | The injected dependency is now accessible to all methods in this class. |
| `if (tenant == null) return null;` | **Guard clause** — early return | Instead of deeply nested `if/else`, we return immediately when something is wrong. The happy path stays at normal indentation. |
| `return await _repo.GetByIdAsync(id);` | Fetch and return the updated record | After updating, fetch fresh data from the DB so the caller gets the latest version. |

---

## 6. Layer 3 — Infrastructure: Database Access

**Folder**: `Backend/StockDaddy.API/Infrastructure/`  
**Namespaces**: `StockDaddy.Infrastructure.Persistence` (DbContext), `StockDaddy.Infrastructure.Repositories` (repository implementations)

> **Note on folder vs namespace**: The repositories are stored in `Infrastructure/Persistence/Repositories/` on disk, but their namespace is `StockDaddy.Infrastructure.Repositories` (without `.Persistence`). In .NET, namespaces do not have to mirror folder paths — this is a deliberate choice by the author.

Infrastructure implements the interfaces defined in Application. This is where EF Core and PostgreSQL integration lives.

### 6.1 TenantRepository — The Full Implementation

```csharp
using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly ApplicationDbContext _context;

    public TenantRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TenantDto>> GetAllAsync()
    {
        return await _context.Tenants
            .Where(t => !t.IsDeleted)
            .Select(t => new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<TenantDto?> GetByIdAsync(int id)
    {
        return await _context.Tenants
            .Where(t => t.Id == id && !t.IsDeleted)
            .Select(t => new TenantDto
            {
                Id = t.Id,
                Name = t.Name,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CreateTenantRequest tenant)
    {
        var entity = new Tenant
        {
            Name = tenant.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _context.Tenants.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateTenantRequest tenant)
    {
        var entity = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (entity == null) return;

        entity.Name = tenant.Name;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Tenants.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Tenants.Update(entity);
        await _context.SaveChangesAsync();
    }
}
```

**Deep-dive explanation:**

| Code | WHAT | WHY |
|---|---|---|
| `public class TenantRepository : ITenantRepository` | Implements the interface | `: ITenantRepository` is a compiler promise — if you add a method to the interface and forget to add it here, the code will not compile. |
| `private readonly ApplicationDbContext _context;` | EF Core database session | `ApplicationDbContext` is the gateway to the database. It tracks changes and executes queries. |
| `_context.Tenants` | The Tenants table | `Tenants` is a `DbSet<Tenant>` property on the context — it represents the database table. You query it like a C# collection. |
| `.Where(t => !t.IsDeleted)` | Filter out soft-deleted records | `t => !t.IsDeleted` is a **lambda** — a short inline function. EF Core converts this to `WHERE "IsDeleted" = false`. |
| `.Select(t => new TenantDto { ... })` | **Projection** — transform entity to DTO | Instead of loading all columns, we select only what we need. This generates `SELECT "Id", "Name", ...` instead of `SELECT *`. More efficient. |
| `.ToListAsync()` | Execute the query and get all results | Up to this point, no SQL has been sent. LINQ builds a query description. `ToListAsync()` triggers execution. |
| `.FirstOrDefaultAsync()` | Get the first matching result, or null | Generates `LIMIT 1`. Returns `null` if nothing matches — hence the `?` nullable return type. |
| `await _context.Tenants.AddAsync(entity);` | Stage the new record | EF Core starts tracking this entity as "to be inserted" — no SQL yet. |
| `await _context.SaveChangesAsync();` | Write everything to the database | This is where SQL is actually executed. Generates `INSERT INTO "Tenants" (...)`. Always call this to persist changes. |
| `entity.IsDeleted = true;` + `SaveChangesAsync()` | Soft delete | We modify the entity (EF Core tracks the change), then `SaveChangesAsync()` generates `UPDATE "Tenants" SET "IsDeleted" = true WHERE "Id" = @id`. The row is never deleted. |

**How LINQ translates to SQL:**

```csharp
// C# LINQ:
_context.Tenants
    .Where(t => !t.IsDeleted)
    .Select(t => new TenantDto { Id = t.Id, Name = t.Name })
    .ToListAsync();

// Generated SQL (approximately):
SELECT "Id", "Name", "CreatedAt", "UpdatedAt"
FROM "Tenants"
WHERE "IsDeleted" = FALSE
```

EF Core builds this SQL from your C# code. You never write raw SQL for standard operations.

---

## 7. Layer 4 — API: HTTP Entry Point

**Folder (Controllers)**: `Backend/StockDaddy.API/Controllers/UserControllers/`  
**Folder (Background Services)**: `Backend/StockDaddy.API/BgServices/`  
**Namespace**: `StockDaddy.API.Controllers`, `StockDaddy.API.Services`

The API layer is the outer shell. It accepts HTTP requests, calls the appropriate service or repository, and sends back HTTP responses.

> **Note on Controllers location**: All controllers live inside `Controllers/UserControllers/` — the extra subfolder was added to group them together as the controller count grew.

### 7.1 Program.cs — Application Wiring

`Program.cs` is the startup file. It registers all dependencies in the DI container, configures the middleware pipeline, and starts the web server.

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Register Controllers
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));

// 2. Configure PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Register Repositories  (Interface → Implementation)
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
// ... 30+ more repositories

// 4. Register Services
builder.Services.AddScoped<TenantService>();
builder.Services.AddScoped<UserService>();
// ... 30+ more services

// 5. Background Service
builder.Services.AddHostedService<ScheduledPriceRevertBackgroundService>();

// 6. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 7. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
```

**Explanation:**

| Code | WHAT | WHY |
|---|---|---|
| `var builder = WebApplication.CreateBuilder(args);` | Creates the application builder | `var` is **type inference** — the compiler deduces the type. `args` is command-line arguments. |
| `builder.Services.AddControllers();` | Register all controllers | Tells ASP.NET Core to scan for classes ending in `Controller` and wire up routing. |
| `builder.Services.AddDbContext<ApplicationDbContext>(...)` | Register the database context | The DI container will provide this to repositories. Generics `<ApplicationDbContext>` specify which DbContext. |
| `options.UseNpgsql(...)` | Tell EF Core to use PostgreSQL | The connection string comes from `appsettings.json` — never hardcode credentials in code. |
| `builder.Services.AddScoped<ITenantRepository, TenantRepository>();` | Wire interface to implementation | "When code asks for `ITenantRepository`, create and provide a `TenantRepository`." |
| `AddScoped` | **Scoped lifetime** | One instance per HTTP request. All services in the same request share the same DbContext. |
| `builder.Services.AddHostedService<...>()` | Register a background service | Long-running service that starts with the app and runs independently of HTTP requests. |
| `app.UseHttpsRedirection();` | Middleware: redirect HTTP to HTTPS | Security — forces all traffic to be encrypted. |
| `app.MapControllers();` | Map routes to controllers | Reads `[HttpGet]`, `[Route]` attributes and builds the routing table. |
| `await app.RunAsync();` | Start the web server | Begin listening for HTTP connections. |

**Service Lifetimes:**

| Lifetime | Code | Behaviour | Use for |
|---|---|---|---|
| **Scoped** | `AddScoped` | One per HTTP request | Repositories, Services, DbContext |
| **Transient** | `AddTransient` | New instance every injection | Lightweight stateless helpers |
| **Singleton** | `AddSingleton` | One for entire app lifetime | Thread-safe caches, configuration |

### 7.2 TenantController — HTTP Endpoints

```csharp
using Microsoft.AspNetCore.Mvc;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.DTOs;

namespace StockDaddy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantController : ControllerBase
{
    private readonly ITenantRepository _tenantRepository;

    public TenantController(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    // GET api/tenant
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var tenants = await _tenantRepository.GetAllAsync();
            return Ok(tenants);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching tenants: {ex.Message}");
        }
    }

    // GET api/tenant/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant == null)
                return NotFound($"Tenant with ID {id} not found.");
            return Ok(tenant);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching tenant: {ex.Message}");
        }
    }

    // POST api/tenant
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _tenantRepository.AddAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating tenant: {ex.Message}");
        }
    }

    // PUT api/tenant/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTenantRequest request)
    {
        try
        {
            var existing = await _tenantRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Tenant with ID {id} not found.");

            await _tenantRepository.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating tenant: {ex.Message}");
        }
    }

    // DELETE api/tenant/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var existing = await _tenantRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound($"Tenant with ID {id} not found.");

            await _tenantRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting tenant: {ex.Message}");
        }
    }
}
```

**Explanation:**

| Code | WHAT | WHY |
|---|---|---|
| `[ApiController]` | **Attribute** — enables API features | Automatically returns 400 for invalid model state. An attribute is metadata applied to a class or method using `[...]`. |
| `[Route("api/[controller]")]` | Defines base URL route | `[controller]` is replaced with the class name minus "Controller" → `api/tenant`. |
| `public class TenantController : ControllerBase` | Inherits helper methods | `ControllerBase` provides `Ok()`, `NotFound()`, `BadRequest()`, `NoContent()`, `StatusCode()`. |
| `[HttpGet]` | Maps method to `GET /api/tenant` | HTTP verb attributes define which HTTP method triggers this action. |
| `public async Task<IActionResult> GetAll()` | Async method returning an HTTP response | `IActionResult` is any HTTP response — Ok, NotFound, 500, etc. |
| `return Ok(tenants);` | HTTP 200 with JSON body | Serializes `tenants` to JSON automatically. |
| `[HttpGet("{id}")]` | Maps to `GET /api/tenant/5` | `{id}` is a route parameter — extracted from the URL and bound to `int id`. |
| `return NotFound(...)` | HTTP 404 | Standard REST: if a resource does not exist, return 404. |
| `[HttpPost]` | Maps to `POST /api/tenant` | POST is used for creating new resources. |
| `[FromBody] CreateTenantRequest request` | Deserialize JSON body to object | Reads the HTTP request body as JSON and converts it to `CreateTenantRequest`. |
| `if (!ModelState.IsValid) return BadRequest(ModelState);` | Validate input | Checks data annotations like `[Required]` on the DTO. Returns 400 with validation errors if invalid. |
| `return NoContent();` | HTTP 204 | Standard REST for successful update or delete when there is nothing to return. |
| `$"An error occurred: {ex.Message}"` | **String interpolation** | The `$` prefix allows embedding C# expressions inside `{}`. |

**HTTP Methods and REST conventions:**

| Method | Route | Action | Success Response |
|---|---|---|---|
| `GET` | `/api/tenant` | Get all tenants | 200 + JSON array |
| `GET` | `/api/tenant/5` | Get tenant with ID 5 | 200 + JSON object, or 404 |
| `POST` | `/api/tenant` | Create new tenant | 200, or 400 |
| `PUT` | `/api/tenant/5` | Update tenant 5 | 204, or 404 |
| `DELETE` | `/api/tenant/5` | Delete tenant 5 | 204, or 404 |

---

## 8. Complete Request Flow Trace

**Scenario**: A client sends `GET /api/tenant/7`

```
Step 1:  HTTP Request arrives at the server
         GET /api/tenant/7

Step 2:  ASP.NET Core routing reads [Route] and [HttpGet("{id}")] attributes
         → Matches TenantController.GetById(int id)
         → Extracts id = 7 from the URL

Step 3:  DI container constructs TenantController
         → Constructor needs ITenantRepository
         → Looks up registration: ITenantRepository → TenantRepository
         → Creates TenantRepository, injecting ApplicationDbContext
         → Creates TenantController, injecting TenantRepository

Step 4:  GetById(7) executes
         → Calls: await _tenantRepository.GetByIdAsync(7)

Step 5:  TenantRepository.GetByIdAsync(7) executes
         → Builds LINQ query:
           _context.Tenants
             .Where(t => t.Id == 7 && !t.IsDeleted)
             .Select(t => new TenantDto { ... })
             .FirstOrDefaultAsync()

Step 6:  EF Core translates LINQ to SQL:
         SELECT "Id", "Name", "CreatedAt", "UpdatedAt"
         FROM "Tenants"
         WHERE "Id" = 7 AND "IsDeleted" = FALSE
         LIMIT 1

Step 7:  PostgreSQL executes the query
         → Returns the row, or empty if not found

Step 8:  EF Core maps the result to TenantDto
         → TenantDto { Id=7, Name="ACME Corp", ... }  or  null

Step 9:  Back in GetById(7):
         → if null:  return NotFound("Tenant with ID 7 not found.")
         → if found: return Ok(tenant)

Step 10: ASP.NET Core serializes TenantDto to JSON:
         { "id": 7, "name": "ACME Corp", "createdAt": "...", "updatedAt": "..." }

Step 11: HTTP Response sent to client
         HTTP/1.1 200 OK
         Content-Type: application/json
         { "id": 7, "name": "ACME Corp", ... }
```

**Each layer only talks to its adjacent layer. Nothing skips levels.**

---

## 9. Core C# Concepts Explained

### 9.1 async / await — Non-Blocking Code

**The Problem Without Async:**

Imagine your web server has 10 threads. 10 requests arrive simultaneously, each needing a 100ms database query. With synchronous code, all 10 threads are blocked waiting for the database. Request 11 has no thread and is queued or dropped.

With async/await, while a thread waits for the database, it is released back to the pool to handle other requests.

```csharp
// ❌ Synchronous — blocks thread while waiting for DB
public List<TenantDto> GetAll()
{
    return _context.Tenants.ToList(); // Thread sits idle for 50ms
}

// ✅ Asynchronous — thread is free while waiting for DB
public async Task<List<TenantDto>> GetAllAsync()
{
    return await _context.Tenants.ToListAsync(); // Thread released, resumes when DB responds
}
```

**Rules for async/await:**
1. If a method does I/O (database, file, HTTP call), make it `async`
2. Return `Task<T>` instead of `T`, `Task` instead of `void`
3. Use `await` when calling other async methods
4. Add `Async` suffix to method names by convention

### 9.2 Generics — Type-Safe Reusable Code

Generics let you write code that works with any type while being checked at compile time.

```csharp
// Without generics — impractical
class TenantList { void Add(Tenant t) { } }
class UserList    { void Add(User u)   { } }

// With generics — one class works for all types
class List<T>     { void Add(T item)  { } }

// Usage:
List<Tenant>  tenants  = new();
List<User>    users    = new();
List<Product> products = new();
```

**Common generic types in this codebase:**

| Type | Meaning |
|---|---|
| `List<TenantDto>` | A list of TenantDto objects |
| `Task<List<TenantDto>>` | An async operation that returns a list of TenantDto |
| `DbSet<Tenant>` | EF Core's representation of the Tenants table |
| `ICollection<User>` | A collection of User objects (navigation property) |
| `Dictionary<int, decimal>` | Maps integer keys to decimal values |

### 9.3 Interfaces — Programming to Abstractions

```csharp
// The contract — Application layer:
public interface ITenantRepository
{
    Task<List<TenantDto>> GetAllAsync();
}

// Real implementation — Infrastructure layer:
public class TenantRepository : ITenantRepository
{
    public async Task<List<TenantDto>> GetAllAsync()
        => await _context.Tenants.Select(...).ToListAsync();
}

// Fake implementation — for unit tests:
public class FakeTenantRepository : ITenantRepository
{
    private readonly List<TenantDto> _data = new();
    public async Task<List<TenantDto>> GetAllAsync()
        => await Task.FromResult(_data);
}

// Controller uses the interface — works with both real and fake:
public TenantController(ITenantRepository repo) { _repo = repo; }
```

**Why program to interfaces?**
- Swap implementations without changing callers
- Write unit tests without a real database
- Code is explicit about what capabilities it needs

### 9.4 LINQ — Query Language Built into C#

```csharp
var active  = tenants.Where(t => !t.IsDeleted);
var names   = tenants.Select(t => t.Name);
var first   = tenants.FirstOrDefault(t => t.Id == 5);
var count   = tenants.Count(t => !t.IsDeleted);
var ordered = tenants.OrderBy(t => t.Name);
var exists  = tenants.Any(t => t.Name == "ACME");
```

**Deferred Execution:**

```csharp
var query = _context.Tenants.Where(t => !t.IsDeleted);
// No SQL sent yet — query is just a description

var result = await query.ToListAsync();
// NOW the SQL is sent and results are returned
```

This lets EF Core see the full query and build the most efficient SQL before executing.

### 9.5 Dependency Injection — Inversion of Control

```csharp
// ❌ Without DI — tightly coupled, impossible to test:
public class TenantController
{
    public TenantController()
    {
        var context = new ApplicationDbContext(hardcodedOptions);
        var repo = new TenantRepository(context); // YOU create dependencies
    }
}

// ✅ With DI — loosely coupled, easily testable:
public class TenantController
{
    private readonly ITenantRepository _repo;

    public TenantController(ITenantRepository repo) // Framework provides it
    {
        _repo = repo;
    }
}
```

The controller declares what it needs. The framework provides it. The controller never knows if it got a real or fake repository.

---

## 10. Best Practices in This Codebase

### Naming Conventions

| Convention | Example | Reason |
|---|---|---|
| Interfaces start with `I` | `ITenantRepository` | Instantly identifiable as interface |
| Async methods end with `Async` | `GetAllAsync()` | Signals "use await when calling this" |
| Private fields start with `_` | `_repo`, `_context` | Distinguishes from parameters and locals |
| Classes end with their type | `TenantController`, `TenantService` | Clear, predictable naming |
| DTOs end with `Dto` or `Request` | `TenantDto`, `CreateTenantRequest` | Clear purpose |

### UTC for All Timestamps

```csharp
// ✅ UTC — consistent regardless of server location
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

// ❌ Local time — inconsistent if server moves timezone
public DateTime CreatedAt { get; set; } = DateTime.Now;
```

### Null Safety

```csharp
// ✅ Initialize strings to empty — never null
public string Name { get; set; } = string.Empty;

// ✅ Use nullable types only for truly optional values
public DateTime? DeletedAt { get; set; }

// ✅ Null-check before using
var tenant = await _repo.GetByIdAsync(id);
if (tenant == null) return NotFound();
// safe to use tenant below
```

### Guard Clauses — Early Return Pattern

```csharp
// ❌ Deeply nested — hard to read:
public async Task<bool> DeleteAsync(int id)
{
    var tenant = await _repo.GetByIdAsync(id);
    if (tenant != null)
    {
        await _repo.DeleteAsync(id);
        return true;
    }
    else { return false; }
}

// ✅ Guard clause — flat, readable:
public async Task<bool> DeleteAsync(int id)
{
    var tenant = await _repo.GetByIdAsync(id);
    if (tenant == null) return false; // fail fast

    await _repo.DeleteAsync(id);
    return true;
}
```

### Use `decimal` for Money — Always

```csharp
// ✅ Exact decimal arithmetic
public decimal Price { get; set; }
public decimal TotalAmount { get; set; }

// ❌ Floating-point errors — never for currency
public double Price { get; set; }
```

---

## 11. Background Services

**File**: `Backend/StockDaddy.API/BgServices/ScheduledPriceRevertBackgroundService.cs`  
**Namespace**: `StockDaddy.API.Services`

Background services run continuously, independent of HTTP requests. This one reverts product prices after a scheduled time — for example, "sale ends at midnight."

```csharp
public class ScheduledPriceRevertBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScheduledPriceRevertBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);

    public ScheduledPriceRevertBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ScheduledPriceRevertBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var revertService = scope.ServiceProvider
                        .GetRequiredService<ScheduledPriceRevertService>();
                    var productVariantService = scope.ServiceProvider
                        .GetRequiredService<ProductVariantService>();

                    var now = DateTime.UtcNow;
                    var dueReverts = await revertService.GetDueRevertsAsync(now);

                    foreach (var revert in dueReverts)
                    {
                        var priceMap = JsonSerializer
                            .Deserialize<Dictionary<int, decimal>>(revert.OriginalPricesJson);

                        if (priceMap != null)
                        {
                            foreach (var kvp in priceMap)
                            {
                                await productVariantService
                                    .RevertVariantPriceAsync(kvp.Key, kvp.Value);
                            }
                        }
                        await revertService.MarkAsCompletedAsync(revert.Id);
                        _logger.LogInformation(
                            "Reverted scheduled price for revertId={RevertId}", revert.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ScheduledPriceRevertBackgroundService");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}
```

**Explanation:**

| Code | WHAT | WHY |
|---|---|---|
| `: BackgroundService` | Inherit the hosted service base class | `BackgroundService` provides the plumbing. You only override `ExecuteAsync`. |
| `TimeSpan _interval = TimeSpan.FromSeconds(10)` | How often to check | `TimeSpan` represents a duration. Runs every 10 seconds. |
| `CancellationToken stoppingToken` | Signal to stop gracefully | When the app shuts down, `stoppingToken.IsCancellationRequested` becomes `true`, letting the loop exit cleanly. |
| `while (!stoppingToken.IsCancellationRequested)` | Run forever until app stops | The main background loop. |
| `using (var scope = _serviceProvider.CreateScope())` | Create a DI scope manually | Background services are **singleton** lifetime. Repositories are **scoped**. You cannot inject scoped into singleton — create a temporary scope per iteration. `using` disposes the DbContext when done. |
| `scope.ServiceProvider.GetRequiredService<T>()` | Resolve a scoped service manually | Equivalent to constructor injection but done manually inside the loop scope. |
| `JsonSerializer.Deserialize<Dictionary<int, decimal>>(json)` | Parse JSON string to C# object | Original prices stored as JSON in DB. `Dictionary<int, decimal>` maps VariantId → OriginalPrice. |
| `foreach (var kvp in priceMap)` | Iterate key-value pairs | `kvp.Key` = VariantId, `kvp.Value` = original price. |
| `await Task.Delay(_interval, stoppingToken)` | Wait 10 seconds (non-blocking) | Does not block the thread. Passing `stoppingToken` means the delay cancels immediately if the app stops. |
| `_logger.LogInformation("...", revert.Id)` | Structured logging | Named placeholder `{RevertId}` creates a queryable structured log entry. |

---

## 12. The Full Entity Map

All 34 entities in the system and how they relate:

```
Tenant ──┬── Store ──┬── Product ──┬── ProductVariant ──┬── SaleItem
         │           │             │                    └── PurchaseItem
         │           │             ├── ProductTag
         │           │             ├── ProductImage
         │           │             ├── ProductAttribute
         │           │             └── ProductBundle ── BundleItem
         │           │
         │           ├── Subcategory ── Category
         │           ├── StockItem
         │           └── AuditLog
         │
         ├── User ──── Role ── RolePermission ── Permission
         │
         ├── Customer ── Sale ──┬── SaleItem
         │                     ├── Invoice ── AdjustedInvoice
         │                     ├── GiftOption
         │                     ├── BundleSaleItem
         │                     ├── Return ── Refund
         │                     └── Payment
         │
         ├── Supplier ── PurchaseOrder ── PurchaseItem
         │
         ├── TaxRegion
         ├── HsnMaster
         ├── ProductRestockAlert
         ├── Shipment
         ├── IntegrationEvent
         └── ScheduledPriceRevert
```

---

## 13. Common Patterns Reference

### Every Repository Follows This 5-Method Shape

```csharp
Task<List<EntityDto>> GetAllAsync();
Task<EntityDto?> GetByIdAsync(int id);
Task AddAsync(CreateEntityRequest request);
Task UpdateAsync(int id, UpdateEntityRequest request);
Task DeleteAsync(int id);
```

Learn it once for `TenantRepository`, and every other repository works the same way.

### Every Controller Follows This 5-Endpoint Shape

```csharp
[HttpGet]              GetAll()         → 200 + list, or 500
[HttpGet("{id}")]      GetById(id)      → 200 + item, or 404, or 500
[HttpPost]             Create([body])   → 200, or 400, or 500
[HttpPut("{id}")]      Update(id, body) → 204, or 404, or 500
[HttpDelete("{id}")]   Delete(id)       → 204, or 404, or 500
```

### Soft Delete Is Applied Everywhere

Every read query filters deleted records:
```csharp
.Where(t => !t.IsDeleted)
```

Every single-record fetch checks both ID and deletion:
```csharp
.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted)
```

Every delete sets flags instead of removing the row:
```csharp
entity.IsDeleted = true;
entity.DeletedAt = DateTime.UtcNow;
entity.UpdatedAt = DateTime.UtcNow;
```

### Projection Is Always Done in the Query

```csharp
// ✅ Efficient — only selected columns loaded from DB
.Select(t => new TenantDto { Id = t.Id, Name = t.Name })
.ToListAsync()

// ❌ Inefficient — loads ALL columns, then maps in memory
var entities = await _context.Tenants.ToListAsync();
return entities.Select(t => new TenantDto { ... });
```

---

## Key Takeaways

| Concept | What You Learned |
|---|---|
| **Clean Architecture** | 4 logical layers — Domain, Application, Infrastructure, API — organized as folders within a single project, with inward-only namespace dependencies |
| **Domain Entities** | Pure C# classes with no external dependencies — the business heart of the system |
| **Interfaces** | Contracts that decouple callers from implementations, enabling testability and flexibility |
| **DTOs** | Data containers that control exactly what the API exposes — separate from DB entities |
| **Repository Pattern** | All database access isolated behind an interface, one class per entity |
| **Dependency Injection** | Framework-managed object creation and lifetime — no `new` in business code |
| **async/await** | Non-blocking I/O — threads are freed while waiting for DB, enabling high throughput |
| **Generics** | Type-safe reusable code — `List<T>`, `Task<T>`, `DbSet<T>` |
| **LINQ** | Compile-time-checked, composable database queries written in C# |
| **Soft Delete** | `IsDeleted` flag preserves data forever for audit trails and recovery |
| **Multi-tenancy** | `TenantId` on every entity ensures organizations never see each other's data |
| **Background Services** | Long-running tasks (price reverts) outside the HTTP request cycle |

---

*This codebase is your textbook. Every file demonstrates professional .NET development practices. Read it, run it, break it, fix it, and build on it.*
