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
    { path: '/admin/dashboard', name: 'admin-dashboard', component: () => import('@/views/admin/AdminDashboardView.vue'), meta: { role: 'PlatformAdmin' } },
    { path: '/admin/products', name: 'admin-products', component: () => import('@/views/admin/AdminProductsView.vue'), meta: { role: 'PlatformAdmin' } },
    { path: '/admin/orders', name: 'admin-orders', component: () => import('@/views/admin/AdminOrdersView.vue'), meta: { role: 'PlatformAdmin' } },
    { path: '/admin/claims', name: 'admin-claims', component: () => import('@/views/admin/AdminClaimsView.vue'), meta: { role: 'PlatformAdmin' } },
    { path: '/:pathMatch(.*)*', name: 'not-found', component: () => import('@/views/NotFoundView.vue'), meta: { public: true } },
  ],
})

// Guard: enforce login + role on protected routes.
router.beforeEach((to) => {
  const auth = useAuthStore()
  if (!to.meta.public && !auth.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }
  if (to.meta.role && auth.user?.role !== to.meta.role) {
    return { name: auth.isAdmin ? 'admin-dashboard' : 'products' }
  }
})

export default router
