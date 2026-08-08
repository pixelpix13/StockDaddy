# Code Walkthrough — Line-by-Line

This document explains **important source files line by line** so someone learning C# or React can follow along.

> **Note:** We annotate the files every developer should read first. For patterns that repeat across hundreds of files, see [LEARNING-GUIDE.md](./LEARNING-GUIDE.md).

---

## Table of contents

1. [Frontend: api.client.ts](#1-frontend-apiclientts)
2. [Frontend: AuthContext.tsx](#2-frontend-authcontexttsx)
3. [Frontend: StoreContext.tsx (key parts)](#3-frontend-storecontexttsx)
4. [Backend: RequestContextMiddleware.cs](#4-backend-requestcontextmiddlewarecs)
5. [Backend: Program.cs (startup sections)](#5-backend-programcs)
6. [Backend: User entity](#6-backend-user-entity)
7. [Backend: UserStore entity](#7-backend-userstore-entity)

---

## 1. Frontend: `api.client.ts`

**Path:** `Frontend/src/services/api.client.ts`  
**Purpose:** Single HTTP client used by all services. Attaches auth headers and handles global errors.

```typescript
/**
 * Shared Axios instance for all API services.
 * - Attaches JWT from localStorage on every request.
 * - On 401, clears session and redirects to `/login`.
 * - On 403, dispatches a global event for toast display (permission denied).
 */
```
| Line | What it means |
|------|---------------|
| Block comment | Documentation for humans — describes the file's job |

```typescript
import axios from 'axios';
import { getStoredActiveStoreId, clearStoredActiveStoreId } from '@/lib/store';
```
| Line | What it means |
|------|---------------|
| `import axios` | Bring in Axios library — popular HTTP client for JavaScript |
| `import { ... } from '@/lib/store'` | Import helpers for reading/clearing active store ID from localStorage. `@/` is alias for `src/` |

```typescript
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '/api';
```
| Line | What it means |
|------|---------------|
| `import.meta.env` | Vite's way to read environment variables at build time |
| `VITE_API_BASE_URL` | Optional override for API URL |
| `\|\| '/api'` | Fallback: use relative `/api` (Vite dev server proxies to backend) |

```typescript
export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
});
```
| Line | What it means |
|------|---------------|
| `axios.create(...)` | Factory for a configured Axios instance |
| `baseURL` | Prefix added to every request path |
| `Content-Type: application/json` | Tell server we're sending JSON bodies |

```typescript
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('stockdaddy_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    const activeStoreId = getStoredActiveStoreId();
    if (activeStoreId) {
      config.headers['X-Store-Id'] = String(activeStoreId);
    }
    return config;
  },
  (error) => Promise.reject(error)
);
```
| Line | What it means |
|------|---------------|
| `interceptors.request.use` | Run this function **before every HTTP request** |
| `localStorage.getItem(...)` | Read JWT saved at login from browser storage |
| `Authorization: Bearer ...` | Standard header format for JWT authentication |
| `X-Store-Id` | Custom header telling backend which store user is working in |
| `return config` | Must return modified config so request proceeds |
| `Promise.reject(error)` | Pass request setup errors to caller |

```typescript
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 403) { ... }
    if (error.response?.status === 401) { ... }
    return Promise.reject(error);
  }
);
```
| Line | What it means |
|------|---------------|
| `interceptors.response.use` | Run after server responds |
| First callback | Success path — return response unchanged |
| Second callback | Error path — inspect HTTP status |
| `403` | Forbidden — user authenticated but lacks permission → show toast via custom event |
| `401` | Unauthorized — token invalid/expired → clear storage, redirect to login |
| `?.` optional chaining | Safely access nested properties if `error.response` exists |

---

## 2. Frontend: `AuthContext.tsx`

**Path:** `Frontend/src/context/AuthContext.tsx`  
**Purpose:** Global authentication state shared by entire app via React Context.

```typescript
interface AuthContextType {
  user: UserDto | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (request: LoginRequest) => Promise<void>;
  ...
  hasPermission: (module: string, action?: PermissionAction) => boolean;
}
```
| Line | What it means |
|------|---------------|
| `interface` | TypeScript contract describing shape of context value |
| `UserDto \| null` | User object or null when logged out |
| `Promise<void>` | Async function returning nothing meaningful |

```typescript
const AuthContext = createContext<AuthContextType | undefined>(undefined);
```
| Line | What it means |
|------|---------------|
| `createContext` | React API to create a context "channel" for passing data without prop drilling |
| `undefined` default | Consumer must be inside Provider or handle undefined |

```typescript
const [user, setUser] = useState<UserDto | null>(() => authService.getStoredUser());
const [token, setToken] = useState<string | null>(() => authService.getToken());
const [isLoading, setIsLoading] = useState<boolean>(true);
```
| Line | What it means |
|------|---------------|
| `useState` | React hook for component state |
| `() => authService.getStoredUser()` | Lazy initializer — read from localStorage once on mount |
| `isLoading: true` | Start true until we validate stored token |

```typescript
useEffect(() => {
  const initAuth = async () => {
    const storedToken = authService.getToken();
    if (storedToken) {
      try {
        const session = await authService.refreshSession();
        setUser(session.user);
        setToken(session.token);
      } catch (error) {
        authService.logout();
        setUser(null);
        setToken(null);
      }
    }
    setIsLoading(false);
  };
  initAuth();
}, []);
```
| Line | What it means |
|------|---------------|
| `useEffect(..., [])` | Run once when component mounts (empty dependency array) |
| `refreshSession()` | Calls GET `/auth/me` — validates token, may return fresh JWT |
| `catch` | If token expired/invalid, clear session |
| `setIsLoading(false)` | App can now render protected routes |

```typescript
const hasPermission = useCallback(
  (module: string, action: PermissionAction = 'Read') => {
    const key = permissionKey(module, action).toLowerCase();
    return (user?.permissions ?? []).some((p) => p.toLowerCase() === key);
  },
  [user?.permissions]
);
```
| Line | What it means |
|------|---------------|
| `useCallback` | Memoize function — only recreate when permissions change |
| `permissionKey(module, action)` | Builds `"Sales:Read"` string |
| `.some(...)` | Returns true if any permission in array matches |
| `?? []` | Default to empty array if permissions undefined |

```typescript
return (
  <AuthContext.Provider value={{ user, token, isAuthenticated: !!token, ... }}>
    {children}
  </AuthContext.Provider>
);
```
| Line | What it means |
|------|---------------|
| `Provider` | Makes context value available to all child components |
| `isAuthenticated: !!token` | Double-bang converts token to boolean (truthy string → true) |

```typescript
export function useAuth(): AuthContextType {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
}
```
| Line | What it means |
|------|---------------|
| Custom hook | Convenient way for any component to read auth state |
| Throw if missing Provider | Fail fast with clear error during development |

---

## 3. Frontend: `StoreContext.tsx`

**Path:** `Frontend/src/context/StoreContext.tsx`

Key flow in `setActiveStore`:

```typescript
const setActiveStore = useCallback(async (storeId: number) => {
  setActiveStoreId(storeId);
  setStoredActiveStoreId(storeId);
  try {
    await authService.switchStore(storeId);
    await refreshSession();
  } catch { /* JWT may lag */ }
  await reloadStores();
  window.dispatchEvent(new CustomEvent('stockdaddy:store-changed', { detail: storeId }));
}, [refreshSession, reloadStores]);
```

| Line | What it means |
|------|---------------|
| Update local state + localStorage immediately | UI feels responsive |
| `switchStore` | POST to backend — updates user's active store server-side |
| `refreshSession` | Get new JWT with permissions for that store's role |
| `reloadStores` | Refresh store list (role names may update) |
| `CustomEvent` | Notify pages (Sales, Dashboard, …) to reload store-scoped data |

---

## 4. Backend: `RequestContextMiddleware.cs`

**Path:** `Backend/StockDaddy.API/Application/Authorization/RequestContextMiddleware.cs`  
**Purpose:** After JWT validation, figure out which store this request operates in.

```csharp
public sealed class RequestContextMiddleware
{
    public const string StoreIdHeader = "X-Store-Id";
    private readonly RequestDelegate _next;
```
| Line | What it means |
|------|---------------|
| `sealed class` | Cannot be inherited — middleware is final |
| `RequestDelegate _next` | Reference to next middleware in pipeline |
| `StoreIdHeader` | Constant for header name — avoids typos |

```csharp
public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext db, RequestContextHolder holder)
```
| Line | What it means |
|------|---------------|
| `InvokeAsync` | Entry point ASP.NET calls for each request |
| `HttpContext` | Current HTTP request/response + user |
| `ApplicationDbContext db` | EF Core database — injected by DI |
| `RequestContextHolder holder` | Scoped object where we store resolved context |

```csharp
if (httpContext.User.Identity?.IsAuthenticated == true)
```
| Line | What it means |
|------|---------------|
| Check authenticated | Only resolve store context for logged-in users |
| `?.` | Null-safe — if Identity is null, expression is null (not true) |

```csharp
var userId = ParseIntClaim(httpContext.User, ClaimTypes.NameIdentifier);
var tenantId = ParseIntClaim(httpContext.User, "tenantId");
var jwtStoreId = ParseIntClaim(httpContext.User, "storeId");
```
| Line | What it means |
|------|---------------|
| Read JWT claims | Extract IDs embedded at login time |
| `ClaimTypes.NameIdentifier` | Standard claim type for user ID |

```csharp
var permissions = httpContext.User
    .FindAll(PermissionKeys.ClaimType)
    .Select(c => c.Value)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
```
| Line | What it means |
|------|---------------|
| `FindAll` | JWT can have multiple claims with same type |
| `ToHashSet` | Fast lookup when checking permissions |
| `OrdinalIgnoreCase` | "Sales:Read" equals "sales:read" |

```csharp
var canAccessAll = PermissionKeys.GrantsAccessToAllStores(roleName, permissions);
```
| Line | What it means |
|------|---------------|
| Admin or Settings:AccessAllStores | User can see every store in tenant |

```csharp
if (canAccessAll) {
    allowedStoreIds = await db.Stores.Where(...).Select(s => s.Id).ToListAsync();
} else {
    var assignments = await db.UserStores.Where(...).Join(...).ToListAsync();
    ...
}
```
| Line | What it means |
|------|---------------|
| Branch on access level | Admins get all store IDs; others get assigned stores only |
| `ToListAsync()` | EF Core executes SQL asynchronously |

```csharp
if (httpContext.Request.Headers.TryGetValue(StoreIdHeader, out var headerValue) &&
    int.TryParse(headerValue.FirstOrDefault(), out var headerStoreId))
{
    if (canAccessAll || allowedStoreIds.Contains(headerStoreId))
        activeStoreId = headerStoreId;
}
activeStoreId ??= jwtStoreId ?? defaultStoreId;
```
| Line | What it means |
|------|---------------|
| Prefer frontend header | User's UI store selection takes priority |
| Validate access | Reject header store if not in allowed list |
| `??=` null coalescing assignment | Use JWT store, then default, if header not valid |

```csharp
holder.Context = new RequestContext { ... };
await _next(httpContext);
```
| Line | What it means |
|------|---------------|
| Store in scoped holder | Repositories read this later in same request |
| `_next(httpContext)` | Pass request to rest of pipeline (authorization, controller) |

---

## 5. Backend: `Program.cs`

**Path:** `Backend/StockDaddy.API/Program.cs`  
**Purpose:** Application entry point — register services and configure middleware.

```csharp
var builder = WebApplication.CreateBuilder(args);
```
| Line | What it means |
|------|---------------|
| `WebApplication.CreateBuilder` | .NET 6+ minimal hosting model — creates app builder |
| `args` | Command-line arguments |

```csharp
builder.Services.AddControllers(options => {
    options.Filters.Add<PermissionAuthorizationFilter>();
    options.Filters.Add<ActivityAuditFilter>();
})
```
| Line | What it means |
|------|---------------|
| `AddControllers` | Enable MVC-style API controllers |
| `Filters.Add<...>` | Global filters run on every controller action |

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```
| Line | What it means |
|------|---------------|
| Register DbContext | EF Core database session (scoped per request) |
| `UseNpgsql` | PostgreSQL provider |
| `GetConnectionString` | Read from appsettings.json |

```csharp
builder.Services.AddScoped<IUserRepository, UserRepository>();
```
| Line | What it means |
|------|---------------|
| `AddScoped` | Create one instance per HTTP request |
| Interface → Implementation | Dependency inversion — code depends on `IUserRepository`, not concrete class |

```csharp
builder.Services.AddAuthentication(...).AddJwtBearer(options => { ... });
builder.Services.AddAuthorization(options => {
    options.FallbackPolicy = ...RequireAuthenticatedUser();
});
```
| Line | What it means |
|------|---------------|
| JWT Bearer | Validate tokens on `[Authorize]` endpoints |
| Fallback policy | All endpoints require auth unless marked `[AllowAnonymous]` |

```csharp
app.UseAuthentication();
app.UseMiddleware<RequestContextMiddleware>();
app.UseAuthorization();
app.MapControllers();
```
| Line | What it means |
|------|---------------|
| Order matters | Auth first, then store context, then authorization check |
| `MapControllers` | Wire up route table from controller attributes |

```csharp
await DbInitializer.InitializeAsync(context);
```
| Line | What it means |
|------|---------------|
| On startup | Run migrations + seed default data |

---

## 6. Backend: User entity

**Path:** `Backend/StockDaddy.API/Domain/Entities/User.cs`

```csharp
public class User
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int RoleId { get; set; }
    public int? StoreId { get; set; }
```
| Line | What it means |
|------|---------------|
| `public class` | C# class — maps to database table via EF Core |
| `{ get; set; }` | Auto-property — EF reads/writes column |
| `int?` | Nullable int — user may not have default store |

```csharp
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
```
| Line | What it means |
|------|---------------|
| `= string.Empty` | Default value — avoids null reference warnings |
| PasswordHash | Never store plain passwords |

```csharp
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
```
| Line | What it means |
|------|---------------|
| Soft delete | Row stays in DB but excluded from normal queries |

```csharp
    public Tenant? Tenant { get; set; }
    public ICollection<UserStore> UserStores { get; set; } = new List<UserStore>();
```
| Line | What it means |
|------|---------------|
| Navigation properties | EF Core uses these for JOINs / Include() |
| `ICollection<UserStore>` | One user → many store assignments |

---

## 7. Backend: UserStore entity

**Path:** `Backend/StockDaddy.API/Domain/Entities/UserStore.cs`

```csharp
public class UserStore
{
    public int UserId { get; set; }
    public int StoreId { get; set; }
    public int RoleId { get; set; }
    public bool IsDefault { get; set; }
```
| Line | What it means |
|------|---------------|
| Composite key (UserId + StoreId) | Configured in DbContext — one row per user-store pair |
| `RoleId` | Role effective when user works in this store |
| `IsDefault` | Login / landing store |

---

## Files to read next

| File | Why |
|------|-----|
| `Application/Services/OrchestrationService.cs` | Checkout transaction logic |
| `Application/Services/AuthService.cs` | JWT generation, store switching |
| `Application/Authorization/PermissionAuthorizationFilter.cs` | How API enforces RBAC |
| `Frontend/src/pages/SalesPage.tsx` | Full POS UI flow |
| `Frontend/src/pages/AccessControlPage.tsx` | RBAC management UI |
| `Frontend/src/hooks/usePagedList.ts` | Standard list page state machine |
| `Infrastructure/Persistence/Repositories/CustomerRepository.cs` | Store-scoped query example |

Use [LEARNING-GUIDE.md](./LEARNING-GUIDE.md) to understand patterns before reading these files.

---

## How to extend this document

When adding a new major file, copy the table format above:

1. File path and one-sentence purpose
2. Code block with line groups
3. Table explaining each line or logical group
4. "Connects to" section linking related files

Keep walkthroughs focused on **one concern per file** — auth, store context, checkout, etc.
