# 03 — Frontend (Vue 3) explained

Folder: `frontend/`. Built with **Vite** (dev server + bundler), **Vue 3 Composition API** with
`<script setup>`, and **TypeScript**.

## The libraries (and why each)

| Library | Why it's here |
|---------|---------------|
| **Vue 3** | the UI framework — builds the components/pages |
| **TypeScript** | types catch mistakes before runtime (e.g. wrong API field) |
| **Vite** | instant dev server with Hot Module Reload + fast production build |
| **Vue Router** | maps URLs to pages and **guards** protected/admin routes |
| **Pinia** | global state store — keeps the logged-in user and the cart |
| **PrimeVue** (Aura theme) | ready-made UI: tables, dialogs, inputs, toasts, charts |
| **Axios** | HTTP client — calls the API and attaches the JWT automatically |
| **Chart.js** | the dashboard graphs (via PrimeVue's `Chart`) |
| **@fontsource/inter** | the Inter font, bundled (no external request) |

## Folder structure (`frontend/src/`)

```
src/
├── main.ts                 # app bootstrap: install Pinia, Router, PrimeVue, register components
├── App.vue                 # picks the layout (public vs app-shell) and hosts <RouterView/>
├── layouts/
│   ├── PublicShell.vue      # top-nav layout for guests (login/register/browse)
│   └── AppShell.vue         # sidebar + topbar layout for logged-in users (role-aware)
├── router/index.ts          # all routes + navigation guards (auth + role)
├── stores/
│   ├── auth.ts              # login/register/logout, token, isAdmin (persisted to localStorage)
│   └── cart.ts              # cart items, count, total
├── services/                # one file per API area; all use the shared axios client
│   ├── http.ts              # axios instance + JWT interceptor + error message helper
│   ├── auth.service.ts
│   ├── products.service.ts
│   ├── orders.service.ts
│   ├── claims.service.ts
│   └── admin.*.service.ts   # dashboard, products, orders, claims, customers, staff
├── views/                   # one component per page
│   ├── LoginView, RegisterView
│   ├── ProductsView, ProductDetailView, CartView
│   ├── OrdersView, OrderDetailView, ClaimsView, DashboardView, NotFoundView
│   └── admin/               # AdminDashboard/Products/Orders/Claims/Customers/Staff View
├── components/
│   ├── ProductImage.vue     # shows the photo, falls back to a category tile on error
│   └── QuantityStepper.vue  # the −/＋ quantity control
├── utils/
│   ├── format.ts            # currency (DKK), dates, status colours
│   └── productImage.ts      # category tile (emoji + gradient) used as image fallback
├── types/index.ts           # TypeScript interfaces mirroring the API DTOs
└── style.css                # the design system (colours, layout, tables, components)
```

## Key concepts

### Composition API + `<script setup>`
Every component uses `<script setup lang="ts">`. You declare reactive state with `ref()`, computed values
with `computed()`, and lifecycle with `onMounted()`. It's less boilerplate than the old Options API.

```ts
const products = ref<Product[]>([])      // reactive list
const loading = ref(false)
onMounted(async () => { products.value = (await productsService.getProducts()).items })
```

### Reactivity
`ref(x)` makes a value reactive — when `.value` changes, the template re-renders automatically. In the
template you use it without `.value` (Vue unwraps it).

### Components & props
Small reusable pieces (e.g. `ProductImage`, `QuantityStepper`) take **props** and emit **events**
(`v-model` is "prop + update event"). Example: `QuantityStepper` takes `modelValue`/`min`/`max` and emits
`update:modelValue`.

### Pinia stores (global state)
- `auth.ts` — holds the logged-in user + token, exposes `isAuthenticated` and `isAdmin`, and **persists to
  `localStorage`** so a refresh keeps you logged in.
- `cart.ts` — holds cart items; `count` and `total` are computed.
Any component can read/update a store; changes are reactive everywhere.

### Vue Router + guards
`router/index.ts` lists routes (URL → view). A **navigation guard** runs before each route:
```ts
router.beforeEach((to) => {
  const auth = useAuthStore()
  if (!to.meta.public && !auth.isAuthenticated) return { name: 'login' }       // must be logged in
  if (to.meta.role && auth.user?.role !== to.meta.role) return { name: 'products' } // must be admin
})
```
This is **frontend** protection (nice UX). The real security is the **backend** `[Authorize]` — the guard
just hides what you can't use.

### Services + the axios interceptor
All API calls go through `services/http.ts`, which:
- attaches `Authorization: Bearer <token>` to every request (from the auth store),
- on a `401` response, logs the user out and redirects to login,
- has `getApiErrorMessage()` to pull a friendly message out of the API's ProblemDetails.

So a view never builds a URL with a token by hand — it calls e.g. `ordersService.createOrder(payload)`.

### Layout switching (App.vue)
`App.vue` shows `PublicShell` when logged out and `AppShell` (sidebar + topbar) when logged in. For an
admin, the sidebar shows the **back-office** nav instead of the shop nav — driven by `auth.isAdmin`.

### PrimeVue
Components are registered globally in `main.ts` (`Button`, `DataTable`, `Dialog`, `Select`, `Chart`,
`FileUpload`, `Toast`…). The **Aura theme** + our `style.css` give the look. Toasts (`useToast()`) show the
top-right confirmations; `ConfirmDialog` (`useConfirm()`) handles "are you sure?" deletes.

## How one page works end-to-end (ProductsView)

1. `onMounted` → calls `productsService.getProducts({ search, categoryId, page })`.
2. `http.ts` adds the JWT (if any) and sends `GET /api/products?...`.
3. The API returns `PagedResult<Product>` as JSON; axios parses it.
4. The component stores it in `products` (a `ref`) → the template renders a grid of cards.
5. Each card uses `ProductImage` (real photo or fallback) and an **Add to cart** button → `cartStore.add(product)`.
6. Typing in the search box updates `search`; a `watch` debounces (300ms) and reloads.

## Build & run
- Dev: `npm run dev` → Vite serves at `http://localhost:5173` with hot reload.
- Type-check + production build: `npm run build` (runs `vue-tsc` then `vite build`).
- API base URL comes from `frontend/.env` (`VITE_API_BASE_URL`).
