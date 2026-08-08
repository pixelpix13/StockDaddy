# StockDaddy Documentation

Welcome to the StockDaddy documentation. This folder explains **what the system does**, **how the pieces connect**, and **how to read the code** — including beginner-friendly, line-by-line walkthroughs of important files.

---

## Who this is for

| Audience | Start here |
|----------|------------|
| New team member / product owner | [FEATURES-AND-FLOWS.md](./FEATURES-AND-FLOWS.md) |
| Backend developer (C# / ASP.NET) | [ARCHITECTURE.md](./ARCHITECTURE.md) → [BACKEND.md](./BACKEND.md) → [DATABASE-SCHEMA.md](./DATABASE-SCHEMA.md) |
| Frontend developer (React / TypeScript) | [ARCHITECTURE.md](./ARCHITECTURE.md) → [FRONTEND.md](./FRONTEND.md) |
| Beginner learning C# or React | [LEARNING-GUIDE.md](./LEARNING-GUIDE.md) → [CODE-WALKTHROUGH.md](./CODE-WALKTHROUGH.md) |

---

## Documentation map

| Document | Contents |
|----------|----------|
| **[ARCHITECTURE.md](./ARCHITECTURE.md)** | System overview, tech stack, layered design, request lifecycle, multi-tenant + multi-store model, RBAC, why we chose this architecture |
| **[DATABASE-SCHEMA.md](./DATABASE-SCHEMA.md)** | Every PostgreSQL table, columns, relationships, enums, and how data is scoped by tenant/store |
| **[BACKEND.md](./BACKEND.md)** | Controllers, services, repositories, middleware, DTOs, migrations, API conventions |
| **[FRONTEND.md](./FRONTEND.md)** | Pages, routes, components, contexts, services, hooks, permissions UI, theme system |
| **[FEATURES-AND-FLOWS.md](./FEATURES-AND-FLOWS.md)** | Feature-by-feature guide: POS, purchases, credit, companies, access control, activity log, and how they interconnect |
| **[CODE-WALKTHROUGH.md](./CODE-WALKTHROUGH.md)** | Line-by-line annotated explanations of key source files (backend + frontend) |
| **[LEARNING-GUIDE.md](./LEARNING-GUIDE.md)** | How to read *any* file in this repo; C# and React patterns used throughout; glossary |

---

## Quick system summary

**StockDaddy** is a multi-tenant retail inventory and POS system:

```
Browser (React SPA)
    │  JWT + X-Store-Id header
    ▼
ASP.NET Core API (StockDaddy.API)
    │  EF Core
    ▼
PostgreSQL
```

**Hierarchy:** `Tenant` → `Store` → store-scoped data (sales, customers, inventory, …)

**Security:** JWT authentication + claim-based RBAC (`Module:Action` permissions). Users can have **different roles per store**.

**Frontend:** React 19 + TypeScript + Vite + Tailwind + Radix/shadcn UI.

**Backend:** .NET 9 monolithic API with Domain / Application / Infrastructure layers inside one project.

---

## About “line-by-line” documentation

A literal annotation of **every line in the entire codebase** would be hundreds of thousands of lines and impossible to keep in sync with code changes.

Instead, this documentation provides:

1. **Complete structural coverage** — all tables, routes, components, and features
2. **Representative line-by-line walkthroughs** — the files every developer should read first
3. **Pattern guides** — so you can understand new files without a separate doc for each one

If you need a walkthrough for a specific file not yet covered, open an issue or extend [CODE-WALKTHROUGH.md](./CODE-WALKTHROUGH.md) using the same format.

---

## Local development

| Service | URL / command |
|---------|----------------|
| Backend API | `dotnet run` in `Backend/StockDaddy.API` → `http://localhost:5215` |
| Frontend | `npm run dev` in `Frontend` → `http://localhost:5173` |
| Swagger (dev) | `http://localhost:5215/swagger` |
| Default admin | `admin` / `Admin@123` (seeded on first run) |

---

## Related files in the repo

| Path | Purpose |
|------|---------|
| `Backend/StockDaddy.API/Program.cs` | API startup, DI, middleware |
| `Frontend/src/App.tsx` | Route table |
| `Frontend/src/config/permissions.ts` | Frontend permission modules (must match backend) |
| `Backend/StockDaddy.API/Infrastructure/Persistence/PermissionSeeder.cs` | Default roles and permissions |
