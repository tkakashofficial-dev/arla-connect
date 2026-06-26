# 01 — Architecture (the big picture)

## The three parts

```
┌─────────────┐      HTTPS/JSON      ┌──────────────┐      SQL       ┌──────────────┐
│  Vue 3 SPA  │ ───────────────────► │  .NET 10 API │ ─────────────► │  SQL Server  │
│ (frontend)  │ ◄─────────────────── │  (backend)   │ ◄───────────── │  (database)  │
└─────────────┘   JSON + JWT token   └──────────────┘   EF Core      └──────────────┘
  localhost:5173                       localhost:5136                   Docker :1433
```

- **Frontend** runs in the browser. It never talks to the database directly — only to the API.
- **API** holds all business rules and security. It's the only thing that touches the database.
- **Database** stores data. Reached only through EF Core from the API.

This separation is **headless commerce** — the same shape as the real Arla Connect (Vue front end on a
.NET/Optimizely backend).

## Backend layering — Clean Architecture

The backend is one solution (`backend/Connect.slnx`) split into **4 projects (layers)**. The golden rule:
**dependencies point inward.** Inner layers know nothing about outer layers.

```
        ┌──────────────────────────────────────────────┐
        │                Connect.Api                     │  ← controllers, Program.cs, auth, errors
        │   depends on Application + Infrastructure       │
        ├──────────────────────────────────────────────┤
        │            Connect.Infrastructure              │  ← EF Core, SQL Server, JWT, hashing
        │            depends on Application               │
        ├──────────────────────────────────────────────┤
        │             Connect.Application                │  ← business logic (services), DTOs, validation
        │              depends on Domain                  │
        ├──────────────────────────────────────────────┤
        │               Connect.Domain                   │  ← entities, enums (NO dependencies)
        └──────────────────────────────────────────────┘
```

**Why this matters (interview answer):** business rules live in Domain/Application and don't depend on
EF Core or ASP.NET. You could swap SQL Server for Postgres, or the web framework, without touching the
business logic. It's also easy to unit-test because the Application layer depends on *interfaces*
(like `IAppDbContext`), not concrete classes.

| Layer | Project | Contains | Depends on |
|-------|---------|----------|-----------|
| Domain | `Connect.Domain` | Entities (Product, Order…), enums, base entity | nothing |
| Application | `Connect.Application` | Feature services, DTOs, validators, interfaces | Domain |
| Infrastructure | `Connect.Infrastructure` | EF Core `AppDbContext`, configs, migrations, JWT, BCrypt, seed | Application |
| API | `Connect.Api` | Controllers, `Program.cs`, error handler, JWT setup | Application + Infrastructure |

## How a request flows (example: "show me products")

```
1. Browser → GET http://localhost:5136/api/products?search=milk
2. ProductsController.GetPaged()                 [Connect.Api]
3.   → IProductService.GetPagedAsync(query)      [Connect.Application]  (business logic)
4.     → IAppDbContext.Products (EF Core query)  [Connect.Infrastructure runs the SQL]
5.   ← returns PagedResult<ProductDto>           (a clean shape, not the raw entity)
6. Controller returns JSON → browser renders it with Vue
```

For a **secured** request (e.g. place an order):
```
Browser sends "Authorization: Bearer <JWT>"  →  API validates the token  →
  ICurrentUser reads the user/customer from the token  →  service scopes data to that customer
```

## Cross-cutting concerns (applied everywhere)

- **Validation** — FluentValidation checks inputs before logic runs.
- **Error handling** — one `GlobalExceptionHandler` turns exceptions into consistent JSON (RFC-7807 ProblemDetails).
- **Auth** — JWT validated at the API; `[Authorize]` and `[Authorize(Roles=...)]` guard endpoints.
- **Logging** — Serilog writes structured logs.
- **Pagination** — list endpoints return pages, never unbounded result sets.

## Why these choices (not over-engineered)

- **Modular monolith, not microservices** — one deployable API. For an app this size microservices would
  add complexity (network calls, distributed transactions) for no benefit. A senior knows when *not* to split.
- **Services, not heavy CQRS libraries** — feature services (e.g. `OrderService`) keep it readable; reads
  use lightweight DTO projections, writes go through the domain entities.
