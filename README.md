# StockDaddy

**StockDaddy** is a multi-tenant retail management platform for inventory, point-of-sale (POS), purchases, and store operations. It helps businesses run multiple store locations from one system—with role-based access, per-store data, and wholesale (B2B) sales support.

## What it does

- **Catalog & inventory** — products, variants, barcodes, HSN tax codes, stock levels, and alerts  
- **Point of sale** — checkout with retail or wholesale buyers, discounts, and credit sales  
- **Purchasing** — suppliers, purchase orders, and partial goods receiving  
- **Parties** — customers, suppliers, and B2B companies (scoped per store)  
- **Credit & reminders** — track receivables and payments  
- **Access control** — roles, permissions, and different roles per store for each user  
- **Activity log** — audit trail of changes  
- **Multi-store** — switch stores in the UI; data and permissions follow the active store  

## Tech stack

| Layer | Technologies |
|-------|----------------|
| Frontend | React 19, TypeScript, Vite, Tailwind CSS, PWA |
| Backend | ASP.NET Core (.NET 9), JWT, RBAC |
| Database | PostgreSQL, Entity Framework Core |

## Quick start

**Backend** (API at `http://localhost:5215`):

```bash
cd Backend/StockDaddy.API
dotnet run
```

**Frontend** (app at `http://localhost:5173`):

```bash
cd Frontend
npm install
npm run dev
```

Default login after first run: `admin` / `Admin@123` (change in production).

## Documentation

Full guide (architecture, database, code patterns, production build, interview prep): **[docs/GUIDE.md](./docs/GUIDE.md)**

## Repository layout

```
Backend/StockDaddy.API/   ASP.NET Core REST API
Frontend/                 React SPA
Website/                  Marketing landing page (optional)
docs/GUIDE.md             Complete project documentation
```
