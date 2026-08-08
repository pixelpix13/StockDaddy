# Backend Reference

**Project:** `Backend/StockDaddy.API`  
**Entry point:** `Program.cs`

---

## Folder map

| Path | Purpose |
|------|---------|
| `Domain/Entities/` | 39 POCO entity classes (database tables) |
| `Domain/Enums/` | Domain enumerations |
| `Application/DTOs/` | Request/response models for API |
| `Application/Interfaces/` | Repository interfaces (`I*Repository`) |
| `Application/Services/` | Business logic services |
| `Application/Authorization/` | RBAC filters, request context, permission keys |
| `Application/Helpers/` | QueryScope, paging, password hash, store assignment normalization |
| `Infrastructure/Persistence/` | DbContext, seeders, initializer |
| `Infrastructure/Persistence/Repositories/` | EF Core repository implementations |
| `Infrastructure/Migrations/` | EF Core migration files |
| `Controllers/UserControllers/` | REST API controllers |
| `Controllers/Optional/` | Feature-flagged controllers |
| `BgServices/` | Background hosted services |

---

## Dependency injection (Program.cs)

Registration order:

1. **Controllers** + global filters (`PermissionAuthorizationFilter`, `ActivityAuditFilter`)
2. **AutoMapper**
3. **ApplicationDbContext** (PostgreSQL)
4. **37 repositories** (scoped)
5. **39 services** (scoped)
6. **JWT authentication**
7. **Authorization** (fallback: authenticated user required)
8. **Swagger** (development only)
9. **CORS** (localhost:5173)

### Scoped services worth knowing

| Service | Role |
|---------|------|
| `AuthService` | Login, register, JWT, store switch, session refresh |
| `OrchestrationService` | Multi-step transactions (checkout, PO, stock adjust) |
| `RbacService` | Permission matrix, roles, user store assignments |
| `RequestContextHolder` | Per-request tenant/store context (implements `IRequestContext`) |

---

## Middleware pipeline

```
HTTPS → CORS → Authentication (JWT) → RequestContextMiddleware → Authorization → Controllers
```

### RequestContextMiddleware

Reads from JWT + optional `X-Store-Id` header:

- `UserId`, `TenantId`
- `AllowedStoreIds` (from UserStores or all stores if admin)
- `ActiveStoreId` (header → JWT → default assignment)
- `CanAccessAllTenantStores`

Repositories inject `IRequestContext` to filter queries.

---

## Global action filters

### PermissionAuthorizationFilter

Runs before every controller action:

1. Skip if `[AllowAnonymous]` or `[SkipPermissionCheck]`
2. Resolve required permission from controller name + HTTP method
3. Check JWT `permission` claims
4. Return **403 Forbidden** if missing

### ActivityAuditFilter

On POST/PUT/PATCH/DELETE:

- Writes row to `AuditLogs` with user, store, table, old/new JSON

---

## Controller conventions

- Route: `[Route("api/[controller]")]` → `/api/Product`, `/api/Sale`, etc.
- Standard CRUD: `GET`, `GET {id}`, `POST`, `PUT {id}`, `DELETE {id}`
- Paged lists: query params `page`, `pageSize`, `search`, `sortBy`, `sortDir`, `storeId`

### Special controllers

| Controller | Notes |
|------------|-------|
| `AuthController` | login/register anonymous; me/stores/switch-store skip permission check |
| `OrchestrationController` | Composite workflows |
| `RbacController` | Matrix, role CRUD, store assignments |
| `CreditLedgerController` | Payments sub-route, no delete |
| `AuditLogController` | Read + manual create only |
| `BillAdjustmentController` | Feature-flagged optional module |

Full endpoint list: see exploration in repo or Swagger at `/swagger`.

---

## Repository pattern

**Interface:** `Application/Interfaces/IProductRepository.cs`  
**Implementation:** `Infrastructure/Persistence/Repositories/ProductRepository.cs`

Typical methods:

```csharp
Task<PagedResult<ProductDto>> GetPagedAsync(PagedQuery query, IRequestContext ctx);
Task<ProductDto?> GetByIdAsync(int id, IRequestContext ctx);
Task<ProductDto> CreateAsync(CreateProductRequest request);
Task UpdateAsync(int id, UpdateProductRequest request);
Task SoftDeleteAsync(int id);
```

### QueryScope helper

```csharp
// Resolves storeId from query param or active store; validates user access
var storeId = QueryScope.ResolveStoreFilter(query, requestContext);

// LINQ extensions
query = query.ApplyTenantFilter(tenantId);
query = query.ApplyStoreFilter(storeId);
```

Used by: Customer, Supplier, Company, CreditLedger, AuditLog, User repositories.

---

## DTOs

Located in `Application/DTOs/`. Naming:

| Pattern | Example |
|---------|---------|
| Read model | `ProductDto`, `SaleDto` |
| Create | `CreateProductRequest` |
| Update | `UpdateProductRequest` |
| Paged query | `PagedQuery` (+ optional `StoreId`) |

Enums serialize as **strings** in JSON (`JsonStringEnumConverter`).

---

## OrchestrationService (critical workflows)

| Method | What it does atomically |
|--------|------------------------|
| `CreateProductWithVariantAsync` | Product + variant + initial stock |
| `CheckoutAsync` | Sale + items + stock decrement + credit ledger (if credit payment) |
| `AdjustStockAsync` | Stock quantity adjustment + audit |
| `CreatePurchaseOrderWithItemsAsync` | PO + line items |
| `ReceivePurchaseOrderAsync` | Update received qty, increase stock, mark fully received |
| `GetVariantStockAsync` | POS inventory view with filters |
| `GetVariantByBarcodeAsync` | Barcode scan lookup |

Uses EF Core transactions — all succeed or all roll back.

---

## AuthService (JWT contents)

Claims issued on login / store switch:

| Claim | Value |
|-------|-------|
| `NameIdentifier` | User ID |
| `Name` | Username |
| `Email` | Email |
| `tenantId` | Tenant ID |
| `storeId` | Active store ID |
| `roleId` | Effective role for active store |
| `Role` | Role name |
| `permission` (multiple) | e.g. `Sales:Read`, `Sales:Write` |

Effective role resolved from `UserStores` for active store, else profile `RoleId`.

---

## Startup seeding

`DbInitializer.InitializeAsync()` on startup:

1. Applies pending migrations
2. `PermissionSeeder` — modules × actions, default role mappings
3. Default tenant, store, categories, HSN
4. Bootstrap admin user (`admin` / `Admin@123`)

---

## Configuration

| Setting | Location | Purpose |
|---------|----------|---------|
| Connection string | `appsettings.json` → `DefaultConnection` | PostgreSQL |
| JWT secret/issuer/audience | `appsettings.json` → `Jwt` | Token signing |
| Feature flags | `appsettings.json` → `Features` | Bill adjustment module |

---

## Adding a new entity (checklist)

1. Create entity in `Domain/Entities/`
2. Add `DbSet<>` in `ApplicationDbContext`
3. Configure relationships in `OnModelCreating` if needed
4. `dotnet ef migrations add YourMigrationName`
5. Create DTOs in `Application/DTOs/`
6. Create `IYourRepository` + `YourRepository`
7. Create `YourService` (optional) or use repository in controller
8. Register in `Program.cs`
9. Create controller in `Controllers/UserControllers/`
10. Add permissions to `PermissionSeeder` + frontend `permissions.ts`
11. Add frontend service + page

See [LEARNING-GUIDE.md](./LEARNING-GUIDE.md) for pattern explanations.
