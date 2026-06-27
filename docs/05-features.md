# 05 — Every feature, and the files that build it

For each feature: what it does, the **backend** files, and the **frontend** files. This is the
map of how each feature is built and works.

## Auth (register + login)
- **What:** customer-number-gated registration (join a company) + JWT login.
- **Backend:** `Application/Features/Auth/` (`AuthService`, `AuthDtos`, `AuthValidators`),
  `Infrastructure/Identity/` (`JwtTokenGenerator`, `PasswordHasher`, `CurrentUser`), `Api/Controllers/AuthController.cs`.
- **Frontend:** `views/LoginView.vue`, `views/RegisterView.vue`, `stores/auth.ts`, `services/auth.service.ts`.

## Product catalogue (browse + detail)
- **What:** search, category filter, paging, product cards with images, detail page.
- **Backend:** `Application/Features/Products/` (`ProductService` — `AsNoTracking` + DTO projection),
  `Api/Controllers/ProductsController.cs` (`[AllowAnonymous]`).
- **Frontend:** `views/ProductsView.vue`, `views/ProductDetailView.vue`, `components/ProductImage.vue`,
  `services/products.service.ts`.

## Cart + checkout
- **What:** add to cart (browser state), change quantities, place an order (stock check).
- **Backend:** `Application/Features/Orders/OrderService.CreateAsync` (auth + stock + scope), `OrdersController`.
- **Frontend:** `stores/cart.ts`, `views/CartView.vue`, `components/QuantityStepper.vue`.

## Orders (history + detail)
- **What:** list my company's orders, view one with its line items (and images).
- **Backend:** `OrderService.GetMyOrdersAsync` / `GetByIdAsync` (scoped to the company; `Include` lines+product).
- **Frontend:** `views/OrdersView.vue`, `views/OrderDetailView.vue`.

## Claims
- **What:** raise a claim against an order, list/track claims.
- **Backend:** `Application/Features/Claims/ClaimService`, `ClaimsController` (`[Authorize]`).
- **Frontend:** `views/OrderDetailView.vue` (raise dialog), `views/ClaimsView.vue`, `services/claims.service.ts`.

## Dashboard (buyer)
- **What:** stat cards (orders, spend, open claims, cart) + charts (monthly spend, orders by status) + recent orders.
- **Backend:** reuses orders/claims endpoints.
- **Frontend:** `views/DashboardView.vue` (Chart.js via PrimeVue `Chart`).

## Admin — Dashboard
- **What:** platform-wide metrics + orders-by-status doughnut.
- **Backend:** `Application/Features/Admin/Dashboard/AdminDashboardService`, `Api/Controllers/Admin/AdminDashboardController.cs`.
- **Frontend:** `views/admin/AdminDashboardView.vue`.

## Admin — Products (CRUD + image upload)
- **What:** list all products, create/edit, soft-delete, **upload an image file**.
- **Backend:** `Application/Features/Admin/Products/ProductAdminService`, `AdminProductsController`
  (upload endpoint saves to `wwwroot/uploads`, served as static files).
- **Frontend:** `views/admin/AdminProductsView.vue` (PrimeVue `FileUpload` with a custom uploader),
  `services/admin.products.service.ts`.

## Admin — Orders & Claims management
- **What:** see every company's orders/claims; change status inline.
- **Backend:** `Admin/Orders/AdminOrderService`, `Admin/Claims/AdminClaimService` (+ controllers).
- **Frontend:** `views/admin/AdminOrdersView.vue`, `views/admin/AdminClaimsView.vue` (a `Select` per row).

## Admin — Customers (list + onboard)
- **What:** every company with users / orders / total spend; view detail; **add a company** (issues a customer number).
- **Backend:** `Admin/Customers/AdminCustomerService` (`GetAll`, `GetById`, `CreateAsync`), `AdminCustomersController`.
- **Frontend:** `views/admin/AdminCustomersView.vue`.

## Admin — Staff (add admins)
- **What:** list Arla staff; add a new PlatformAdmin.
- **Backend:** `Admin/Staff/AdminStaffService`, `AdminStaffController`.
- **Frontend:** `views/admin/AdminStaffView.vue`.

## Cross-cutting
- **Validation** — FluentValidation validators per feature.
- **Errors** — `Api/Common/GlobalExceptionHandler.cs` → ProblemDetails; frontend `getApiErrorMessage()`.
- **Auth/RBAC** — JWT + `[Authorize(Roles=...)]`; frontend route guard + role-aware nav.
- **Seed data** — `Infrastructure/Persistence/DbSeeder.cs`.
- **Docs/API explorer** — Scalar at `http://localhost:5136/scalar/v1`.
- **CI** — `.github/workflows/ci.yml` (build+test backend, build frontend on every push).
- **One-command run** — `run.ps1`.
