# Arla Connect — B2B Webshop (Demo)

A headless **B2B e-commerce** application built to mirror the architecture of *Arla Connect*:
a **Vue 3** single-page app talking to a **.NET 10** REST API over **SQL Server**, where business
customers browse products, place orders, and raise claims.

> Portfolio project — not affiliated with Arla Foods. Built to demonstrate the exact
> stack and architecture used by Arla's e-commerce team (Vue + .NET + SQL Server + Azure + CI/CD).

---

## Tech stack

| Layer    | Technology |
|----------|------------|
| Frontend | Vue 3, TypeScript, Vite, Pinia, Vue Router, PrimeVue |
| Backend  | .NET 10, ASP.NET Core Web API, Clean Architecture, FluentValidation |
| Auth     | JWT bearer tokens, BCrypt password hashing |
| Data     | SQL Server 2022, Entity Framework Core |
| DevOps   | Docker Compose, GitHub Actions CI, Azure-ready |

## Why this architecture

Arla Connect's real backend runs on **.NET + Optimizely (Episerver)** with a **headless Vue**
frontend. Optimizely is licensed enterprise software that can't be cloned, so this project
mirrors the *architecture pattern* rather than the product:

```
Vue 3 SPA  ──►  .NET 10 REST API  ──►  Application (services)  ──►  Infrastructure (EF Core)  ──►  SQL Server
   (PrimeVue)        (controllers)         (business logic)            (data + integrations)
```

This is **Clean Architecture** — dependencies point inward, so business rules never depend on
the database or the web framework:

- **Connect.Domain** — entities, value objects, business rules (zero dependencies)
- **Connect.Application** — feature services, DTOs, validation, interfaces
- **Connect.Infrastructure** — EF Core, SQL Server, JWT, password hashing
- **Connect.Api** — thin controllers, auth, global error handling

## Features

- 🔐 **Auth** — register a company account, log in, JWT-secured endpoints
- 🧀 **Catalogue** — browse products with search, category filter, and paging
- 🛒 **Cart & checkout** — place an order (with stock validation)
- 📦 **Orders** — view order history and details, scoped per customer
- 🚩 **Claims** — raise a claim against an order and track its status

## Engineering practices

- Clean Architecture with one feature folder per use case
- Read paths use `AsNoTracking` + DTO projection + clamped pagination (no `SELECT *`, no unbounded queries)
- Every error returns RFC-7807 **ProblemDetails** via a global exception handler
- Customer-scoped data access — a customer can never read another customer's orders/claims
- Sequential GUID (v7) primary keys, audit timestamps, unique indexes, explicit FK delete rules
- Structured logging (Serilog), health checks, CORS, OpenAPI docs
- Unit tests (xUnit) + GitHub Actions CI on every push

## Getting started

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download), [Node 20+](https://nodejs.org),
[Docker Desktop](https://www.docker.com/products/docker-desktop/).

```bash
# 1. Start SQL Server (Docker)
docker compose up -d

# 2. Run the API  (auto-applies migrations + seeds the catalogue)
cd backend/src/Connect.Api
dotnet run
#   API:        http://localhost:5136
#   API docs:   http://localhost:5136/scalar/v1
#   Health:     http://localhost:5136/health

# 3. Run the frontend
cd frontend
npm install
npm run dev
#   App: http://localhost:5173
```

### Demo logins

```
Buyer (customer):   demo@arla-connect.test  /  Password123!
Arla admin (staff): admin@arla.com          /  Admin123!
```

The buyer sees the webshop (browse, order, claims); the admin sees the back-office
(manage products). Roles are enforced server-side — a buyer gets HTTP 403 on admin endpoints.

## Project structure

```
arla-connect/
├── backend/
│   ├── src/
│   │   ├── Connect.Domain/          # entities, enums, aggregates
│   │   ├── Connect.Application/     # services, DTOs, validators, interfaces
│   │   ├── Connect.Infrastructure/  # EF Core, migrations, JWT, hashing, seed
│   │   └── Connect.Api/             # controllers, Program.cs, error handling
│   └── tests/Connect.Tests/         # xUnit tests
├── frontend/                        # Vue 3 + TypeScript SPA
├── docs/deployment-azure.md         # how this maps to Azure
├── docker-compose.yml               # local SQL Server 2022
└── .github/workflows/ci.yml         # build + test pipeline
```

## Testing

```bash
cd backend
dotnet test
```

## Deployment

See [docs/deployment-azure.md](docs/deployment-azure.md) for how the project maps onto Azure
(App Service / Container Apps, Azure SQL Database, Static Web Apps).
