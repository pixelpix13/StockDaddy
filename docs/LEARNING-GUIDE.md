# Learning Guide — How to Read This Codebase

This guide is for developers **new to C#, ASP.NET, React, or TypeScript** who want to understand StockDaddy without reading every file.

---

## How this project is organized (mental model)

Think of StockDaddy as three layers:

```
┌─────────────────────────────────────┐
│  React UI (what user sees/clicks)   │
├─────────────────────────────────────┤
│  API Services (HTTP + JSON)         │
├─────────────────────────────────────┤
│  Database (PostgreSQL tables)       │
└─────────────────────────────────────┘
```

When a user clicks "Complete Sale":
1. **React** collects cart data → calls `orchestrationService.checkout()`
2. **Axios** sends POST with JWT + store header
3. **Controller** receives JSON → calls `OrchestrationService`
4. **Service** starts DB transaction → creates Sale, SaleItems, updates stock
5. **Repository/EF** runs SQL
6. **JSON response** flows back → UI shows success toast

---

## C# concepts used in this backend

### Classes and properties

```csharp
public class Sale
{
    public int Id { get; set; }           // Integer column
    public decimal TotalAmount { get; set; }  // Money
    public int? CustomerId { get; set; }  // Nullable — may be null
}
```

- `public` — visible outside the class
- `{ get; set; }` — auto-property (compiler creates backing field)
- `?` after type — value can be null

### Namespaces

```csharp
namespace StockDaddy.Domain.Entities;
```

Groups related types. `using StockDaddy.Domain.Entities;` imports them elsewhere.

### Async / await

```csharp
public async Task<UserDto?> GetByIdAsync(int id)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    return MapToDto(user);
}
```

- `async` method — can pause without blocking thread
- `await` — wait for database/network
- `Task<T>` — promise of future result (like Promise in JavaScript)
- `UserDto?` — may return null

### Dependency injection (DI)

```csharp
public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
}
```

.NET creates `UserService` and passes `UserRepository` automatically. Registered in `Program.cs`:

```csharp
builder.Services.AddScoped<IUserRepository, UserRepository>();
```

**Why:** Easier testing, loose coupling, single place to swap implementations.

### LINQ (Language Integrated Query)

```csharp
var activeUsers = await _context.Users
    .Where(u => !u.IsDeleted && u.TenantId == tenantId)
    .OrderBy(u => u.Username)
    .ToListAsync();
```

Reads like SQL but in C#. EF Core translates to SQL.

### Attributes

```csharp
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
```

Metadata that configures framework behavior.

Common ones in this project:
- `[AllowAnonymous]` — skip auth
- `[SkipPermissionCheck]` — skip RBAC filter
- `[HttpGet]`, `[HttpPost]` — HTTP method routing

---

## React / TypeScript concepts used in this frontend

### Components

```tsx
export const SalesPage: React.FC = () => {
  return <div>...</div>;
};
```

Function that returns JSX (HTML-like syntax in JavaScript).

### Props and state

```tsx
interface ButtonProps {
  onClick: () => void;
  children: React.ReactNode;
}

const [count, setCount] = useState(0);
```

- **Props** — inputs from parent (read-only)
- **State** — data that changes over time; re-render when updated

### Hooks

| Hook | Purpose |
|------|---------|
| `useState` | Local component state |
| `useEffect` | Side effects (fetch on mount, subscriptions) |
| `useCallback` | Memoized function |
| `useMemo` | Memoized computed value |
| `useContext` | Read global context |

Custom hooks (`useAuth`, `usePagedList`) bundle reusable logic.

### TypeScript interfaces

```typescript
export interface SaleDto {
  id: number;
  totalAmount: number;
  paymentMethod: PaymentMethod;
}
```

Compile-time contracts — catches typos before runtime.

### Optional chaining and nullish coalescing

```typescript
user?.permissions ?? []
```

- `?.` — if `user` is null, stop (don't crash)
- `??` — use right side if left is null/undefined

---

## Recurring backend patterns

### Pattern 1: Controller → Service → Repository

```
ProductController.GetPaged()
  → ProductService.GetPagedAsync()
    → ProductRepository.GetPagedAsync()
      → EF Core SQL query
```

Controllers should stay thin — HTTP in/out only.

### Pattern 2: DTO mapping

Never return entity classes directly (may expose PasswordHash, internal fields):

```
Entity (User) → Map → UserDto (safe for JSON)
```

### Pattern 3: Soft delete

```csharp
entity.IsDeleted = true;
entity.DeletedAt = DateTime.UtcNow;
// Don't DELETE FROM Users
```

Queries filter: `.Where(x => !x.IsDeleted)`

### Pattern 4: Paged queries

```csharp
public class PagedQuery {
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public string? SortDir { get; set; }
    public int? StoreId { get; set; }
}
```

Same shape on frontend in `types/paging.ts`.

### Pattern 5: Store scoping

```csharp
var storeId = QueryScope.ResolveStoreFilter(query, _requestContext);
query = query.Where(x => x.StoreId == storeId);
```

Every store-scoped repository does this.

---

## Recurring frontend patterns

### Pattern 1: Page + usePagedList + PagedDataTable

```tsx
const storeId = useActiveStoreId();
const list = usePagedList({
  fetchFn: useCallback((q) => service.getPaged({ ...q, storeId }), [storeId]),
});

return <PagedDataTable columns={columns} list={list} />;
```

Copy this skeleton for new list pages.

### Pattern 2: Permission gating

```tsx
<PermissionGate module={APP_MODULES.Sales} action="Write">
  <Button>Checkout</Button>
</PermissionGate>
```

Always gate destructive/ write actions, not just routes.

### Pattern 3: Store change reload

```tsx
useEffect(() => {
  const reload = () => list.reload();
  window.addEventListener('stockdaddy:store-changed', reload);
  return () => window.removeEventListener('stockdaddy:store-changed', reload);
}, [list]);
```

Store-scoped pages must reload when user switches store.

### Pattern 4: Service layer

Pages never call `apiClient` directly — always go through `services/*.service.ts`:

```tsx
// Good
await customerService.createCustomer(data);

// Avoid in pages
await apiClient.post('/customer', data);
```

---

## Glossary

| Term | Meaning |
|------|---------|
| **Tenant** | Organization / business account |
| **Store** | Branch or location within tenant |
| **DTO** | Data Transfer Object — JSON-friendly shape |
| **Entity** | Database row model (C# class) |
| **JWT** | JSON Web Token — signed auth token |
| **RBAC** | Role-Based Access Control |
| **Claim** | Key-value pair inside JWT (userId, permissions) |
| **Middleware** | Code that runs on every HTTP request |
| **EF Core** | Entity Framework — C# ORM for database |
| **Scoped (DI)** | One instance per HTTP request |
| **Orchestration** | Multi-step business workflow in one transaction |
| **POS** | Point of Sale — checkout screen |
| **HSN** | Harmonized System Nomenclature — Indian tax product code |

---

## Suggested learning path

### Week 1 — Orientation

1. Read [ARCHITECTURE.md](./ARCHITECTURE.md)
2. Read [FEATURES-AND-FLOWS.md](./FEATURES-AND-FLOWS.md)
3. Run app locally, log in as admin, click through every sidebar item

### Week 2 — Frontend

1. Read [FRONTEND.md](./FRONTEND.md)
2. Trace: `LoginPage` → `AuthContext` → `auth.service.ts` → `api.client.ts`
3. Read [CODE-WALKTHROUGH.md](./CODE-WALKTHROUGH.md) sections 1–3
4. Pick one simple page (`CustomersPage`) and trace every import

### Week 3 — Backend

1. Read [BACKEND.md](./BACKEND.md) and [DATABASE-SCHEMA.md](./DATABASE-SCHEMA.md)
2. Read [CODE-WALKTHROUGH.md](./CODE-WALKTHROUGH.md) sections 4–7
3. Open Swagger, call GET `/api/customer`, find code path in CustomerController → Repository
4. Set breakpoint in `RequestContextMiddleware` and inspect resolved store

### Week 4 — Advanced flows

1. Trace checkout: `SalesPage` → `OrchestrationService.CheckoutAsync`
2. Trace RBAC: change permission in UI → inspect JWT claims → hit blocked endpoint
3. Trace multi-store: assign user to two stores with different roles → switch store → watch permissions change

---

## FAQ

### Why one backend project instead of multiple?

Simpler for current team size. Layers (Domain/Application/Infrastructure) still separate concerns within the project.

### Why JWT instead of server sessions?

Stateless API scales easily; permissions travel with token. Trade-off: permission changes require re-login or refresh.

### Why `X-Store-Id` header if JWT has storeId?

User can switch store in UI without waiting for token refresh on every click. Middleware validates header against allowed stores.

### Why soft delete?

Preserves referential integrity and audit history. Sales still reference "deleted" customers.

### Where should I add business logic?

- **Simple CRUD** → Service or Repository
- **Multi-entity workflow** → OrchestrationService
- **Authorization rule** → Permission filter or PermissionKeys helper
- **UI validation** → Page component (duplicate critical rules on backend too)

---

## Reading any unfamiliar file — checklist

1. **What layer?** Entity, DTO, Controller, Service, Repository, Page, Component, Hook?
2. **What feature?** Cross-reference [FEATURES-AND-FLOWS.md](./FEATURES-AND-FLOWS.md)
3. **What tables?** Cross-reference [DATABASE-SCHEMA.md](./DATABASE-SCHEMA.md)
4. **Who calls this?** Use IDE "Find references"
5. **What does it call?** Follow imports / injected dependencies

If the file follows an existing pattern (CRUD repository, paged list page), 80% of the code is boilerplate you already understand from this guide.
