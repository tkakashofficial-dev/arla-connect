# 07 — Project structure (annotated)

```
arla-connect/
├── run.ps1                      # one command to start DB + API + frontend
├── docker-compose.yml           # SQL Server 2022 container
├── README.md                    # quick start + demo logins
├── .github/workflows/ci.yml     # CI: build+test backend, build frontend on every push
├── docs/                        # ← this learning guide
│
├── backend/                     # the .NET 10 solution (Connect.slnx)
│   ├── Directory.Build.props     # shared settings for all projects (net10, nullable, warnings-as-errors)
│   ├── src/
│   │   ├── Connect.Domain/                 # LAYER 1 — entities, no dependencies
│   │   │   ├── Common/BaseEntity.cs        #   Id (GUID v7) + audit timestamps
│   │   │   ├── Enums/                       #   OrderStatus, ClaimStatus, ClaimReason
│   │   │   └── Entities/                     #   BusinessCustomer, User, Category, Product, Order, OrderLine, Claim
│   │   │
│   │   ├── Connect.Application/             # LAYER 2 — business logic, DTOs, interfaces
│   │   │   ├── Common/
│   │   │   │   ├── Interfaces/               #   IAppDbContext, ICurrentUser, IPasswordHasher, IJwtTokenGenerator
│   │   │   │   ├── Models/                   #   PagedResult, PagedRequest (clamps page size)
│   │   │   │   ├── Exceptions/               #   NotFoundException (404), ConflictException (409)
│   │   │   │   └── Roles.cs                  #   Buyer, PlatformAdmin
│   │   │   ├── Features/                     #   one folder per feature (vertical slices)
│   │   │   │   ├── Auth/ Products/ Orders/ Claims/
│   │   │   │   └── Admin/ { Products, Orders, Claims, Dashboard, Customers, Staff }
│   │   │   └── DependencyInjection.cs        #   AddApplication() — registers services + validators
│   │   │
│   │   ├── Connect.Infrastructure/          # LAYER 3 — implementations (the "how")
│   │   │   ├── Persistence/
│   │   │   │   ├── AppDbContext.cs           #   EF Core context (implements IAppDbContext)
│   │   │   │   ├── Configurations/           #   one IEntityTypeConfiguration per entity
│   │   │   │   ├── Migrations/               #   EF Core schema migrations
│   │   │   │   ├── DbSeeder.cs               #   seeds catalogue, images, demo accounts, orders
│   │   │   │   └── AppDbContextFactory.cs    #   design-time factory for `dotnet ef`
│   │   │   ├── Identity/                     #   PasswordHasher (BCrypt), JwtTokenGenerator, CurrentUser
│   │   │   └── DependencyInjection.cs        #   AddInfrastructure() — DbContext + identity
│   │   │
│   │   └── Connect.Api/                     # LAYER 4 — the web edge
│   │       ├── Program.cs                    #   wires everything; auth, CORS, OpenAPI, migrate+seed
│   │       ├── Controllers/                  #   thin controllers (call a service, return result)
│   │       │   ├── AuthController, ProductsController        # public
│   │       │   ├── OrdersController, ClaimsController        # [Authorize] (buyer)
│   │       │   └── Admin/*Controller                        # [Authorize(Roles=PlatformAdmin)]
│   │       ├── Common/GlobalExceptionHandler.cs             # exceptions → ProblemDetails
│   │       ├── Common/StartupExtensions.cs                  # MigrateAndSeedAsync()
│   │       ├── appsettings.json              #   connection string, JWT, CORS, Serilog
│   │       └── wwwroot/uploads/              #   uploaded product images (served static)
│   │
│   └── tests/Connect.Tests/                 # xUnit: OrderTests, PagedRequestTests, PasswordHasherTests
│
└── frontend/                    # the Vue 3 app (Vite + TypeScript)
    ├── index.html                # the single HTML page the SPA mounts into
    ├── vite.config.ts            # Vite config + the "@" → src alias
    ├── .env(.example)            # VITE_API_BASE_URL (where the API is)
    └── src/
        ├── main.ts               # bootstrap: Pinia, Router, PrimeVue, global components
        ├── App.vue               # chooses PublicShell vs AppShell, hosts <RouterView/>
        ├── layouts/              # PublicShell (guest top-nav), AppShell (sidebar + topbar, role-aware)
        ├── router/index.ts       # routes + auth/role guards
        ├── stores/               # auth.ts (login/token/isAdmin), cart.ts
        ├── services/             # http.ts (axios + JWT) + one service per API area
        ├── views/                # pages (Login, Products, Cart, Orders, Claims, Dashboard, admin/*)
        ├── components/           # ProductImage, QuantityStepper
        ├── utils/                # format (currency/date/status), productImage (fallback tile)
        ├── types/index.ts        # TypeScript interfaces matching API DTOs
        └── style.css             # the whole design system
```

## How to read a feature top-to-bottom (example: Orders)
1. Frontend page → `frontend/src/views/OrdersView.vue` (+ `OrderDetailView.vue`)
2. Frontend API call → `frontend/src/services/orders.service.ts`
3. API endpoint → `backend/src/Connect.Api/Controllers/OrdersController.cs`
4. Business logic → `backend/src/Connect.Application/Features/Orders/OrderService.cs`
5. Data shape → `OrderDtos.cs`; entity → `backend/src/Connect.Domain/Entities/Order.cs`
6. DB mapping → `backend/src/Connect.Infrastructure/Persistence/Configurations/OrderConfiguration.cs`

Trace any feature the same way: **view → service → controller → app service → entity → EF config.**
