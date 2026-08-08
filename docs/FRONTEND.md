# Frontend Reference

**Project:** `Frontend/`  
**Entry point:** `src/main.tsx` → `src/App.tsx`

**Stack:** React 19, TypeScript, Vite 6, Tailwind CSS, Radix UI, Axios, React Router 7

---

## Folder map

| Path | Purpose |
|------|---------|
| `src/pages/` | Route-level screens |
| `src/features/` | Optional pluggable modules (bill adjustment) |
| `src/components/layout/` | App shell, guards, header, sidebar |
| `src/components/common/` | Domain-aware reusable UI (tables, filters, gates) |
| `src/components/ui/` | shadcn/Radix primitives (button, dialog, select, …) |
| `src/components/auth/` | Login/register forms |
| `src/components/catalog/` | Catalog tab panels |
| `src/components/access-control/` | RBAC UI components |
| `src/context/` | Global React state providers |
| `src/services/` | HTTP API layer |
| `src/dtos/` | TypeScript interfaces matching backend |
| `src/hooks/` | Shared React hooks |
| `src/config/` | Static app configuration |
| `src/lib/` | Utilities (api helpers, theme, store persistence) |
| `public/` | Static assets, PWA icons, `theme-init.js` |

**Path alias:** `@/` → `src/` (configured in `vite.config.ts`)

---

## Provider tree

```
ThemeProvider (main.tsx)
└── BrowserRouter
    └── AuthProvider
        └── StoreProvider
            └── ToastProvider
                └── Routes / Pages
```

| Provider | File | State |
|----------|------|-------|
| ThemeProvider | `context/ThemeContext.tsx` | light/dark/system preference |
| AuthProvider | `context/AuthContext.tsx` | user, token, login/logout, permissions |
| StoreProvider | `context/StoreContext.tsx` | store list, active store, switcher |
| ToastProvider | `context/ToastContext.tsx` | notification toasts |

---

## Routes (`App.tsx`)

### Public

| Path | Page |
|------|------|
| `/login` | LoginPage |
| `/register` | RegisterPage |

### Protected (require JWT)

All use: `ProtectedRoute` → `MainLayout` → `PermissionRoute(module)` → Page

| Path | Page | Permission Module |
|------|------|-------------------|
| `/` | DashboardPage | Dashboard |
| `/catalog` | CatalogPage | Catalog |
| `/products` | ProductsPage | Product |
| `/inventory` | InventoryPage | Inventory |
| `/sales` | SalesPage | Sales |
| `/purchases` | PurchasesPage | Purchase |
| `/suppliers` | SuppliersPage | Supplier |
| `/companies` | CompaniesPage | Company |
| `/customers` | CustomersPage | Customer |
| `/activity` | ActivityPage | Activity |
| `/credit` | CreditRemindersPage | Credit |
| `/users` | UsersPage | Users |
| `/access-control` | AccessControlPage | AccessControl |
| `/settings` | SettingsPage | Settings |
| `/x/sd-ba-8k2m`* | BillAdjustmentPage | BillAdjustment |

\* Hidden route when `FEATURES.billAdjustment` is enabled.

---

## Route guards

### ProtectedRoute

Redirects to `/login` if no JWT token.

### PermissionRoute

Checks `hasPermission(module, 'Read')` (or custom action). Shows access denied if missing.

---

## API layer

### api.client.ts

Shared Axios instance:

- **Request:** adds `Authorization: Bearer <token>` and `X-Store-Id: <activeStoreId>`
- **403:** dispatches `stockdaddy:forbidden` event → toast
- **401:** clears session, redirects to login

### Services (one per domain)

| Service | File | Backend prefix |
|---------|------|----------------|
| authService | `auth.service.ts` | `/auth` |
| productService | `product.service.ts` | `/product`, `/productvariant` |
| catalogService | `catalog.service.ts` | `/category`, `/subcategory`, `/hsnmaster`, `/taxregion` |
| inventoryService | `inventory.service.ts` | `/stockitem`, `/productrestockalert` |
| saleService | `sale.service.ts` | `/sale`, `/saleitem`, `/invoice` |
| purchaseService | `purchase.service.ts` | `/purchaseorder`, `/supplier` |
| customerService | `customer.service.ts` | `/customer` |
| companyService | `company.service.ts` | `/company` |
| userService | `user.service.ts` | `/user` |
| tenantService | `tenant.service.ts` | `/tenant`, `/store`, `/role` |
| orchestrationService | `orchestration.service.ts` | `/orchestration` |
| rbacService | `rbac.service.ts` | `/rbac` |
| activityService | `activity.service.ts` | `/auditlog` |
| creditService | `credit.service.ts` | `/creditledger` |

Paged lists use `fetchPaged()` from `lib/fetch-paged.ts` with `PagedQuery` params.

---

## Permission system (frontend)

**Format:** `"Module:Action"` (must match backend JWT claims)

### Three enforcement layers

1. **Route:** `PermissionRoute` blocks entire page
2. **Navigation:** `Sidebar` filters `NAV_ITEMS` by Read permission
3. **UI:** `PermissionGate` hides buttons/forms

```tsx
<PermissionGate module={APP_MODULES.Sales} action="Write">
  <Button onClick={checkout}>Complete Sale</Button>
</PermissionGate>
```

### Config: `config/permissions.ts`

- `APP_MODULES` — module keys (must match backend seeder)
- `MODULE_LABELS` — human names for RBAC matrix
- `NAV_ITEMS` — sidebar links with required module

---

## Component library

### `components/ui/` — primitives

Low-level, styled Radix wrappers: `button`, `dialog`, `select`, `tabs`, `card`, `input`, `combobox`, `date-picker`, …

Uses `cn()` from `lib/utils.ts` (clsx + tailwind-merge).

### `components/common/` — app components

| Component | Use |
|-----------|-----|
| `PagedDataTable` | Standard list pages with sort/search/pagination |
| `ListFilters`, `FilterSelect` | Dropdown filters |
| `PageHeader` | Title + description + icon |
| `PermissionGate` | Conditional render by permission |
| `CrudRowActions` | Edit/delete row buttons |
| `StatCard` | Dashboard metrics |
| `Modal` | Legacy modal (older pages) |

**Convention:** newer pages use `ui/` primitives; some older pages still use `common/Button`, `common/Card`.

### `components/layout/`

| Component | Role |
|-----------|------|
| `MainLayout` | Sidebar + Header + `<Outlet />` |
| `Sidebar` | Permission-filtered navigation |
| `Header` | Store switcher, tenant badge, mobile menu |
| `ProtectedRoute` | Auth guard |
| `PermissionRoute` | Permission guard |

---

## Hooks

| Hook | Purpose |
|------|---------|
| `useAuth()` | User, token, login/logout, hasPermission |
| `useStoreContext()` | Stores list, active store, setActiveStore |
| `useActiveStoreId()` | Returns active store ID (fallback 1) |
| `useTheme()` | Theme preference + setter |
| `useToast()` | Show notifications |
| `usePermissions()` | Wrapper around hasPermission + roleName |
| `useTenantScope()` | `{ tenantId, storeId, userId }` for create requests |
| `usePagedList<T>()` | Full paginated list state machine |
| `useDebouncedSearch()` | Search with debounce |

---

## Store context (multi-store)

1. On login, `StoreProvider` loads stores via `GET /auth/me/stores`
2. Active store persisted in `localStorage` (`stockdaddy-active-store`)
3. Header dropdown calls `setActiveStore(id)`:
   - POST `/auth/switch-store`
   - Refresh JWT via `refreshSession()`
   - Dispatch `stockdaddy:store-changed` event
4. Pages listen for store change and reload data with new `storeId`

---

## Theme system

- Preferences: `light`, `dark`, `system`
- Stored in `localStorage` (`stockdaddy-theme`)
- `theme-init.js` in `index.html` prevents flash of wrong theme
- Settings page has appearance picker
- Tailwind `dark:` variants + CSS variables in `index.css`

---

## DTOs (`src/dtos/`)

Barrel export via `index.ts`. Each domain file exports:

- `*Dto` — read models from API
- `Create*Request` / `Update*Request` — write models
- Domain enums/unions

---

## Page → service map

| Page | Primary services |
|------|------------------|
| Dashboard | orchestration, sale, inventory |
| Catalog | catalog |
| Products | product, orchestration, catalog |
| Inventory | inventory, orchestration |
| Sales (POS) | orchestration, sale, customer, company |
| Purchases | purchase, orchestration |
| Suppliers | purchase |
| Companies | company |
| Customers | customer |
| Credit | credit |
| Activity | activity |
| Users | user |
| Access Control | rbac, user, tenant |
| Settings | tenant |

---

## Environment variables

| Variable | Purpose |
|----------|---------|
| `VITE_API_BASE_URL` | API base (default `/api`, proxied to localhost:5215 in dev) |
| `VITE_ENABLE_BILL_ADJUSTMENT` | Enable bill adjustment feature |
| `VITE_BILL_ADJUSTMENT_SECRET_PATH` | Hidden route path |

See `.env.example` in Frontend folder.

---

## Adding a new page (checklist)

1. Add module to `config/permissions.ts` (match backend seeder)
2. Create DTOs in `src/dtos/`
3. Create service in `src/services/`
4. Create page in `src/pages/`
5. Add route in `App.tsx` wrapped with `PermissionRoute`
6. Add nav item to `NAV_ITEMS` in `permissions.ts`
7. Pass `storeId` from `useActiveStoreId()` on paged queries
8. Listen for `stockdaddy:store-changed` if data is store-scoped
