<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/cart'

const router = useRouter()
const cart = useCartStore()
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
          <RouterLink to="/cart" class="nav-link cart-link" aria-label="Cart">
            <i class="pi pi-shopping-cart" style="font-size: 1.2rem"></i>
            <Badge v-if="cart.count" :value="cart.count" severity="success" />
          </RouterLink>
          <Button label="Login" size="small" @click="router.push({ name: 'login' })" />
        </nav>
      </div>
    </header>

    <main class="content">
      <slot />
    </main>

    <footer class="site-footer">
      <div class="footer-inner">
        <span>Arla Connect — B2B webshop demo</span>
        <span>Built with Vue 3 · .NET 10 · SQL Server</span>
      </div>
    </footer>
  </div>
</template>
