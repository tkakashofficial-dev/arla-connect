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
    <Toolbar class="app-bar">
      <template #start>
        <RouterLink to="/products" class="brand">
          <span class="brand-mark">Arla</span> Connect
        </RouterLink>
      </template>

      <template #end>
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
      </template>
    </Toolbar>

    <main class="content">
      <RouterView />
    </main>

    <Toast position="top-right" />
  </div>
</template>
