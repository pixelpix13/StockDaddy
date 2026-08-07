# StockDaddy API

ASP.NET Core 9 Web API for multi-tenant inventory, POS, and purchasing.

## Folder map

| Path | Purpose |
|------|---------|
| `Controllers/UserControllers/` | Standard CRUD HTTP endpoints (one controller per entity). |
| `Controllers/Optional/` | Feature-flagged modules safe to delete (bill adjustment). |
| `Application/Services/` | Business logic. **Actively used:** `AuthService`, `OrchestrationService`, `ScheduledPriceRevertService`. Other `*Service` classes are legacy pass-through wrappers — controllers call repositories directly. |
| `Application/Interfaces/` | Repository contracts + `IAuthService`. |
| `Application/DTOs/` | Request/response models for API bodies. |
| `Application/Helpers/` | Shared helpers (`PasswordHasher`). |
| `Configuration/` | Feature flags bound from `appsettings.json` → `Features` section. |
| `Domain/Entities/` | EF Core entity classes. |
| `Domain/Enums/` | Shared enums (`PurchaseOrderStatus`, `PaymentMethod`, …). |
| `Infrastructure/Persistence/` | `ApplicationDbContext`, `DbInitializer`, repositories. |
| `BgServices/` | Hosted background workers. |

## Which endpoint should I use?

| Use case | Endpoint | Why |
|----------|----------|-----|
| Login / register | `POST /api/auth/login`, `/register` | Issues JWT |
| Simple CRUD on one table | `/api/{entity}` controllers | Direct repository access |
| POS checkout, stock adjust, PO+lines | `/api/orchestration/*` | Single DB transaction, stock sync |
| Correct a posted sale total | `/api/bill-adjustment/*` | Optional module; disabled via `Features:BillAdjustment:Enabled` |

## Security

- Global JWT required on all routes except login/register (`FallbackPolicy` in `Program.cs`).
- CORS allows `http://localhost:5173` (Vite dev server).
- Passwords hashed with BCrypt in `UserRepository` via `PasswordHasher`.

## Local run

```bash
dotnet run --launch-profile http   # http://localhost:5215
```

On first boot, `DbInitializer` migrates PostgreSQL and seeds tenant/store/roles/catalog data.
