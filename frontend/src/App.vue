<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'

const auth = useAuthStore()
const cart = useCartStore()
const router = useRouter()

const isAuthenticated = computed(() => auth.isAuthenticated)

function logout() {
  auth.logout()
  router.push({ name: 'login' })
}
</script>

<template>
  <div class="app">
    <header class="site-header">
      <div class="header-inner">
        <RouterLink to="/products" class="brand">
          <span class="brand-mark">Arla</span> Connect
        </RouterLink>

        <nav class="nav">
          <RouterLink to="/products" class="nav-link">Products</RouterLink>
          <RouterLink v-if="isAuthenticated" to="/orders" class="nav-link">Orders</RouterLink>
          <RouterLink v-if="isAuthenticated" to="/claims" class="nav-link">Claims</RouterLink>

          <RouterLink to="/cart" class="nav-link cart-link" aria-label="Cart">
            <i class="pi pi-shopping-cart" style="font-size: 1.2rem"></i>
            <Badge v-if="cart.count" :value="cart.count" severity="success" />
          </RouterLink>

          <template v-if="isAuthenticated">
            <span class="company">{{ auth.user?.companyName }}</span>
            <Button label="Logout" size="small" severity="secondary" outlined @click="logout" />
          </template>
          <template v-else>
            <Button label="Login" size="small" @click="router.push({ name: 'login' })" />
          </template>
        </nav>
      </div>
    </header>

    <main class="content">
      <RouterView />
    </main>

    <footer class="site-footer">
      <div class="footer-inner">
        <span>Arla Connect — B2B webshop demo</span>
        <span>Built with Vue 3 · .NET 10 · SQL Server</span>
      </div>
    </footer>

    <Toast position="top-right" />
  </div>
</template>
