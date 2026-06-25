import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', redirect: '/products' },
    { path: '/login', name: 'login', component: () => import('@/views/LoginView.vue'), meta: { public: true } },
    { path: '/register', name: 'register', component: () => import('@/views/RegisterView.vue'), meta: { public: true } },
    { path: '/dashboard', name: 'dashboard', component: () => import('@/views/DashboardView.vue') },
    { path: '/products', name: 'products', component: () => import('@/views/ProductsView.vue'), meta: { public: true } },
    { path: '/products/:id', name: 'product-detail', component: () => import('@/views/ProductDetailView.vue'), props: true, meta: { public: true } },
    { path: '/cart', name: 'cart', component: () => import('@/views/CartView.vue') },
    { path: '/orders', name: 'orders', component: () => import('@/views/OrdersView.vue') },
    { path: '/orders/:id', name: 'order-detail', component: () => import('@/views/OrderDetailView.vue'), props: true },
    { path: '/claims', name: 'claims', component: () => import('@/views/ClaimsView.vue') },
    { path: '/:pathMatch(.*)*', name: 'not-found', component: () => import('@/views/NotFoundView.vue'), meta: { public: true } },
  ],
})

// Guard: anything not marked `public` requires an authenticated user.
router.beforeEach((to) => {
  const auth = useAuthStore()
  if (!to.meta.public && !auth.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }
})

export default router
