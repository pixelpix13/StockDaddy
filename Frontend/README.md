# StockDaddy Frontend

React 19 + TypeScript + Vite inventory and POS UI for the StockDaddy API.

## Folder map

| Path | Purpose |
|------|---------|
| `src/pages/` | Route-level screens. Keep pages thin; move reusable UI to `components/`. |
| `src/components/ui/` | shadcn/Radix primitives (Button, Dialog, Select, …). Do not add business logic here. |
| `src/components/common/` | Shared app components (CrudRowActions, PageHeader, BrandLogo). Legacy `Button`/`Modal` remain until Dashboard/Users migrate. |
| `src/components/layout/` | App shell: sidebar, header, auth guard, main layout. |
| `src/components/catalog/` | Catalog-domain widgets (variant picker, catalog tabs). |
| `src/components/auth/` | Login and register forms. |
| `src/features/` | Optional/removable modules (e.g. bill adjustment). Not exported from `services/index.ts`. |
| `src/services/` | Axios API clients — one file per backend area. |
| `src/dtos/` | TypeScript types matching backend request/response shapes. |
| `src/context/` | Global React context (auth session, toasts). |
| `src/hooks/` | Reusable React hooks (tenant scope, catalog data). |
| `src/lib/` | Pure utilities (`cn`, API error parsing). |
| `src/config/` | Feature flags from `VITE_*` env vars. |

## Data flow

1. **Pages** call **services** (never `apiClient` directly except in services).
2. **Services** use **DTOs** and return typed promises.
3. **Auth**: `auth.service` stores JWT in `localStorage`; `api.client` attaches `Authorization` header; 401 clears session and redirects to `/login`.
4. **Multi-step workflows** (checkout, create product+variant, PO with lines) go through `orchestration.service`, not raw CRUD endpoints.

## Conventions

- Import with `@/` alias (e.g. `@/services`, `@/components/ui/button`).
- Use `getApiErrorMessage()` in catch blocks for consistent error toasts.
- Use `useTenantScope()` for `tenantId` / `storeId` instead of repeating `user?.tenantId || 1`.
- New CRUD screens: shadcn `Dialog` + `CrudRowActions`, not legacy `Modal`.

## Scripts

```bash
npm run dev      # Vite dev server (proxies /api → backend)
npm run build    # tsc + production bundle
```
