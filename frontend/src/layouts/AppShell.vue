<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'

const auth = useAuthStore()
const cart = useCartStore()
const router = useRouter()

const initials = computed(() =>
  (auth.user?.fullName ?? '?')
    .split(' ')
    .map((s) => s[0])
    .slice(0, 2)
    .join('')
    .toUpperCase(),
)

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
        <RouterLink to="/dashboard" class="side-link"><i class="pi pi-th-large" /> Dashboard</RouterLink>
        <RouterLink to="/products" class="side-link"><i class="pi pi-box" /> Products</RouterLink>
        <RouterLink to="/orders" class="side-link"><i class="pi pi-shopping-bag" /> Orders</RouterLink>
        <RouterLink to="/claims" class="side-link"><i class="pi pi-flag" /> Claims</RouterLink>
        <RouterLink to="/cart" class="side-link">
          <i class="pi pi-shopping-cart" /> Cart
          <Badge v-if="cart.count" :value="cart.count" severity="success" />
        </RouterLink>
      </nav>

      <div class="sidebar-footer">
        <div class="sidebar-user">
          <div class="avatar">{{ initials }}</div>
          <div class="user-meta">
            <strong>{{ auth.user?.fullName }}</strong>
            <span>{{ auth.user?.companyName }}</span>
          </div>
        </div>
        <Button label="Logout" size="small" severity="secondary" outlined class="full-width" @click="logout" />
      </div>
    </aside>

    <div class="shell-main">
      <main class="shell-content">
        <slot />
      </main>
    </div>
  </div>
</template>
