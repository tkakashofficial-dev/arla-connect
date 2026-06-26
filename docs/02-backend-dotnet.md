# 02 — Backend (.NET 10) explained

Folder: `backend/` — solution `Connect.slnx`, four projects under `src/` plus `tests/`.

## Connect.Domain (the core — no dependencies)

The business model. Pure C# classes, no EF Core, no ASP.NET.

- `Common/BaseEntity.cs` — base class every entity inherits: `Id` (a **sequential GUID v7** — globally
  unique but time-ordered so it's index-friendly), `CreatedAtUtc`, `UpdatedAtUtc`.
- `Enums/` — `OrderStatus` (Pending→Confirmed→Shipped→Delivered/Cancelled), `ClaimStatus`, `ClaimReason`.
- `Entities/`:
  - `BusinessCustomer` — a company (the B2B customer). Has many `Users` and many `Orders`.
  - `User` — a login. Belongs to one `BusinessCustomer`. Has `Email`, `PasswordHash`, `FullName`, `Role`.
  - `Category`, `Product` — the catalogue (Product has `Sku`, `UnitPrice`, `StockQuantity`, `ImageUrl`…).
  - `Order` + `OrderLine` — an order and its line items. **`Order` is an aggregate root**: lines are
    added only through `order.AddLine(product, qty)`, and the total is *derived* from the lines, never stored
    out of sync. This encapsulation (private setters, behaviour methods) is the **DDD-lite** pattern.
  - `Claim` — a complaint raised against an order.

**Interview point:** "I used encapsulated aggregates so the `Order` is always consistent — you can't add an
orphan line or set a wrong total."

## Connect.Application (business logic + contracts)

Organised by **feature folder** (vertical slices). Each feature has DTOs, a service interface, an
implementation, and validators.

- `Common/`:
  - `Models/PagedResult.cs`, `PagedRequest.cs` — paging. `PagedRequest` **clamps** page size (max 100) so a
    client can't ask for 10,000 rows — a scalability safeguard.
  - `Exceptions/` — `NotFoundException` (→404), `ConflictException` (→409).
  - `Interfaces/` — `IAppDbContext` (the database abstraction), `ICurrentUser` (who's logged in),
    `IPasswordHasher`, `IJwtTokenGenerator`. **The Application layer depends on these interfaces, not on EF
    Core/JWT directly** — that's what keeps it testable and decoupled.
  - `Roles.cs` — `Buyer`, `PlatformAdmin`.
- `Features/`:
  - `Auth/` — register (join a company by customer number) + login (issue JWT).
  - `Products/` — browse catalogue (search, filter, paginate); reads use **`AsNoTracking` + DTO projection**.
  - `Orders/` — place an order (stock check), list/view orders **scoped to the caller's company**.
  - `Claims/` — raise/list/view claims, scoped to the company.
  - `Admin/` — back-office: `Products` (CRUD + image upload), `Orders`/`Claims` (status updates),
    `Dashboard` (summary metrics), `Customers` (list + onboard), `Staff` (add admins).
- `DependencyInjection.cs` — `AddApplication()` registers all services + validators.

**Read vs write (interview):** reads project straight to DTOs (`SELECT` only needed columns, no change
tracking → fast). Writes load the entity, call domain behaviour, then `SaveChanges`.

## Connect.Infrastructure (the "how")

Implements the Application interfaces using real technology.

- `Persistence/`:
  - `AppDbContext.cs` — the EF Core context; implements `IAppDbContext`. Auto-applies all entity
    configurations and **stamps audit timestamps** in `SaveChangesAsync`.
  - `Configurations/` — one `IEntityTypeConfiguration` per entity: max lengths, **unique indexes**
    (email, SKU, order number), `decimal(18,2)` precision for money, foreign-key delete rules, enum-as-string.
  - `Migrations/` — EF Core migrations (the versioned DB schema).
  - `DbSeeder.cs` — seeds categories, products (+ real image URLs), demo buyer, demo orders/claims, and the
    Arla admin — idempotent (only fills what's missing).
  - `AppDbContextFactory.cs` — lets `dotnet ef` build the context at design time.
- `Identity/`:
  - `PasswordHasher.cs` — **BCrypt** (per-hash salt; passwords never stored in plain text).
  - `JwtTokenGenerator.cs` — builds the signed JWT (claims: user id, email, role, business-customer id).
  - `CurrentUser.cs` — reads those claims from the current request (`IHttpContextAccessor`).
  - `JwtSettings.cs` — bound from config (`Jwt` section).
- `DependencyInjection.cs` — `AddInfrastructure()` registers the DbContext (SQL Server) + identity services.

## Connect.Api (the web edge)

- `Program.cs` — wires everything: Serilog, `AddApplication()`, `AddInfrastructure()`, controllers (+ enum-as-string JSON),
  ProblemDetails, **JWT bearer auth**, **authorization**, CORS (for the Vue dev server), OpenAPI + Scalar docs,
  health checks, static files (uploaded images), and **migrate + seed on startup**.
- `Controllers/` — thin: they call a service and return the result.
  - Public: `AuthController`, `ProductsController` (`[AllowAnonymous]`).
  - Customer: `OrdersController`, `ClaimsController` (`[Authorize]`).
  - Admin: `Admin/*Controller` (`[Authorize(Roles = Roles.PlatformAdmin)]`).
- `Common/GlobalExceptionHandler.cs` — maps exceptions → ProblemDetails (validation→400, not-found→404,
  conflict→409, unauthorized→401, anything else→500 with details hidden).
- `Common/StartupExtensions.cs` — `MigrateAndSeedAsync()`.

## Security model (how auth + RBAC actually work)

1. **Login** — `AuthController` → `AuthService` checks the password with BCrypt → `JwtTokenGenerator` issues a
   signed JWT containing the user's id, email, **role**, and **business-customer id**.
2. **Every request** — the browser sends `Authorization: Bearer <token>`. The JWT middleware validates the
   signature + expiry. `CurrentUser` exposes the claims to the services.
3. **Endpoint guard** — `[Authorize]` requires a valid token; `[Authorize(Roles = "PlatformAdmin")]` requires the
   admin role. **Enforced server-side**, so even if someone bypasses the UI they get 401/403.
4. **Data scoping** — order/claim queries filter by the caller's `BusinessCustomerId`, so a customer can never
   read another company's data, even by guessing IDs.

## Tests (`tests/Connect.Tests`)

xUnit. Examples: `OrderTests` (AddLine merges quantities, total is correct, rejects qty ≤ 0),
`PagedRequestTests` (clamping), `PasswordHasherTests` (hash ≠ plaintext, verify works). Run: `dotnet test`.

## A real bug we fixed (great interview story)

Order totals once returned **0**. Cause: the read query used a **static mapper method inside a LINQ
projection**, which made EF Core evaluate it **client-side without loading the line data**. Fix: explicit
`Include(o => o.Lines).ThenInclude(l => l.Product)` then map in memory. *Lesson: know how your ORM actually
executes a query, not just that it compiles.*
