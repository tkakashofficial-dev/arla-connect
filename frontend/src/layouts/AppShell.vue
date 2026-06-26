<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'

const auth = useAuthStore()
const cart = useCartStore()
const router = useRouter()
const route = useRoute()

const isAdmin = computed(() => auth.isAdmin)

const initials = computed(() =>
  (auth.user?.fullName ?? '?')
    .split(' ')
    .map((s) => s[0])
    .slice(0, 2)
    .join('')
    .toUpperCase(),
)

const titles: Record<string, string> = {
  dashboard: 'Dashboard',
  products: 'Products',
  orders: 'Orders',
  'order-detail': 'Order details',
  claims: 'Claims',
  cart: 'Your cart',
  'admin-dashboard': 'Back-office dashboard',
  'admin-customers': 'Customers',
  'admin-products': 'Manage products',
  'admin-orders': 'All orders',
  'admin-claims': 'All claims',
  'admin-staff': 'Staff',
}
const pageTitle = computed(() => titles[route.name as string] ?? 'Arla Connect')

const menu = ref()
const menuItems = [{ label: 'Logout', icon: 'pi pi-sign-out', command: () => logout() }]
function toggleMenu(event: Event) {
  menu.value.toggle(event)
}
function logout() {
  auth.logout()
  router.push({ name: 'login' })
}
</script>

<template>
  <div class="shell">
    <aside class="shell-sidebar">
      <RouterLink to="/dashboard" class="brand sidebar-brand">
        <span class="brand-mark">Arla</span> Connect
      </RouterLink>

      <nav class="side-nav">
        <template v-if="isAdmin">
          <div class="side-section">Back-office</div>
          <RouterLink to="/admin/dashboard" class="side-link"><i class="pi pi-th-large" /> Dashboard</RouterLink>
          <RouterLink to="/admin/customers" class="side-link"><i class="pi pi-building" /> Customers</RouterLink>
          <RouterLink to="/admin/products" class="side-link"><i class="pi pi-box" /> Products</RouterLink>
          <RouterLink to="/admin/orders" class="side-link"><i class="pi pi-shopping-bag" /> Orders</RouterLink>
          <RouterLink to="/admin/claims" class="side-link"><i class="pi pi-flag" /> Claims</RouterLink>
          <RouterLink to="/admin/staff" class="side-link"><i class="pi pi-users" /> Staff</RouterLink>
        </template>
        <template v-else>
          <RouterLink to="/dashboard" class="side-link"><i class="pi pi-th-large" /> Dashboard</RouterLink>
          <RouterLink to="/products" class="side-link"><i class="pi pi-box" /> Products</RouterLink>
          <RouterLink to="/orders" class="side-link"><i class="pi pi-shopping-bag" /> Orders</RouterLink>
          <RouterLink to="/claims" class="side-link"><i class="pi pi-flag" /> Claims</RouterLink>
          <RouterLink to="/cart" class="side-link">
            <i class="pi pi-shopping-cart" /> Cart
            <Badge v-if="cart.count" :value="cart.count" severity="success" />
          </RouterLink>
        </template>
      </nav>
    </aside>

    <div class="shell-main">
      <header class="topbar">
        <h1 class="topbar-title">{{ pageTitle }}</h1>
        <div class="topbar-right">
          <RouterLink v-if="!isAdmin" to="/cart" class="topbar-cart" aria-label="Cart">
            <i class="pi pi-shopping-cart" />
            <Badge v-if="cart.count" :value="cart.count" severity="success" />
          </RouterLink>

          <button class="account-btn" aria-haspopup="true" @click="toggleMenu">
            <span class="avatar sm">{{ initials }}</span>
            <span class="account-meta">
              <strong>{{ auth.user?.fullName }}</strong>
              <small>{{ auth.user?.companyName }}</small>
            </span>
            <i class="pi pi-chevron-down" style="font-size: 0.7rem" />
          </button>
          <Menu ref="menu" :model="menuItems" popup />
        </div>
      </header>

      <main class="shell-content">
        <slot />
      </main>
    </div>
  </div>
</template>
