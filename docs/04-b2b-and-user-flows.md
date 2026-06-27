# 04 — B2B concepts + user flows

## B2B concepts

- **B2B vs B2C** — B2C (Amazon) sells to anyone who signs up and pays by card. **B2B** sells to
  *businesses*: accounts are controlled, prices are often negotiated, and orders are usually **invoiced**
  (paid on terms), **not** paid by card at checkout. *That's why this app has no payment gateway — it's correct
  for B2B.*
- **Self-service** — customers do orders/claims/lookups themselves instead of phoning a rep.
- **Cost-to-serve** — the operational cost of serving a customer. Self-service lowers it (the Arla job ad's
  exact goal). Say: *"the webshop reduces cost-to-serve by letting customers self-place orders and claims."*
- **Headless commerce** — the UI is decoupled from the commerce engine and talks over a REST API; that's why
  a Vue front end can sit on a .NET backend.
- **Multi-tenant data scoping** — each company sees only its own orders/claims, enforced in the backend queries.
- **RBAC** — Buyer (shop) vs PlatformAdmin (back-office), enforced server-side.

## The two user types and how accounts are created

| User | Portal | How the account is created |
|------|--------|----------------------------|
| **Buyer** (customer staff) | Customer webshop | Admin onboards the company → issues a **customer number** → buyer registers with it |
| **Arla admin** (PlatformAdmin) | Admin back-office | An existing admin adds them on the **Staff** page |

> **No email is sent** on registration — the email is just the login ID. The **customer number** is what
> controls who may register (real apps could also send a verification email via SendGrid/SMTP — not built here).

A company (`BusinessCustomer`) can have **many** users (buyers). Arla staff sit under an internal staff org.

---

## Flow 1 — Onboard a company + buyer registration (the B2B way)

```
ADMIN
  login (admin@arla.com) → Customers → "Add customer" → type company name
     → API: AdminCustomerService.CreateAsync → creates BusinessCustomer + generates customer number (AC-2606-XXXX)
     → admin shares the number with the company

BUYER
  Login page → "Create one" → name + email + password + customer number
     → API: AuthService.RegisterAsync
          - checks the email isn't taken
          - finds the BusinessCustomer by customer number (invalid → 400 field error)
          - creates a User (role = Buyer) under that company
          - issues a JWT
     → frontend stores the token (auth store) → lands on the Dashboard
```

## Flow 2 — Login

```
Login → email + password
  → API: AuthService.LoginAsync
       - find user by email
       - verify password with BCrypt (same error whether email or password is wrong → no account enumeration)
       - issue JWT (contains user id, email, role, business-customer id)
  → frontend stores it; isAdmin decides: buyer → /dashboard, admin → /admin/dashboard
```

## Flow 3 — Browse → cart → place order (buyer)

```
Products page  → GET /api/products (search/filter/paginate)  → grid of cards (real images)
Click a card   → /products/:id → GET /api/products/{id}       → detail with quantity stepper
"Add to cart"  → cartStore.add(product, qty)                  → stored in the browser (Pinia)
Cart page      → review items, change quantities (−/＋)
"Place order"  → POST /api/orders { lines:[{productId, quantity}] }
     → API: OrderService.CreateAsync
          - must be authenticated (JWT) → scoped to the caller's company
          - loads products, checks stock (insufficient → 409), reduces stock
          - builds the Order via order.AddLine(...), saves
     → returns the order → frontend navigates to the order detail
```

## Flow 4 — Raise a claim (buyer)

```
Orders → open an order → "Raise a claim" → choose reason + description
  → POST /api/claims { orderId, reason, description }
     → API: ClaimService.CreateAsync
          - verifies the order belongs to the caller's company (else 404)
          - creates the Claim (status = Open)
  → toast confirms; the claim appears on the Claims page
```

## Flow 5 — Admin manages everything (back-office)

```
admin login → AppShell shows the back-office sidebar (Dashboard, Customers, Products, Orders, Claims, Staff)

Dashboard  → GET /api/admin/dashboard → metric cards (orders, revenue, open claims, products, customers)
              + an "orders by status" doughnut
Customers  → GET /api/admin/customers → each company with users / orders / total spend; click "View" for detail;
              "Add customer" to onboard a new company
Products   → list all (incl. inactive); create/edit; UPLOAD an image (POST /api/admin/products/image);
              "delete" = soft delete (IsActive=false, hidden from the shop)
Orders     → GET /api/admin/orders (all companies) → change an order's status inline (Pending→Shipped→Delivered…)
Claims     → GET /api/admin/claims (all companies) → change a claim's status inline (Open→Approved/Resolved…)
Staff      → list Arla admins; "Add staff" creates a new PlatformAdmin account
```

Every `/api/admin/*` endpoint is `[Authorize(Roles = "PlatformAdmin")]` — a buyer calling them gets **403**.

## Demo logins
- **Buyer:** `demo@arla-connect.test` / `Password123!`
- **Admin:** `admin@arla.com` / `Admin123!`
- **Register a new buyer:** use customer number `AC-DEMO-001` (Demo Foodservice A/S)
