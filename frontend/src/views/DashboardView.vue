<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'
import { ordersService } from '@/services/orders.service'
import { claimsService } from '@/services/claims.service'
import { formatCurrency, formatDate, orderStatusSeverity } from '@/utils/format'
import type { Claim, Order } from '@/types'

const auth = useAuthStore()
const cart = useCartStore()
const router = useRouter()

const orders = ref<Order[]>([])
const ordersTotal = ref(0)
const claims = ref<Claim[]>([])
const claimsTotal = ref(0)
const loading = ref(true)

const openClaims = computed(() => claims.value.filter((c) => c.status === 'Open' || c.status === 'UnderReview').length)
const recentOrders = computed(() => orders.value.slice(0, 5))

onMounted(async () => {
  try {
    const [o, c] = await Promise.all([
      ordersService.getMyOrders({ page: 1, pageSize: 5 }),
      claimsService.getMyClaims({ page: 1, pageSize: 50 }),
    ])
    orders.value = o.items
    ordersTotal.value = o.totalCount
    claims.value = c.items
    claimsTotal.value = c.totalCount
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div class="dash-head">
    <div>
      <h1 class="page-title" style="margin-bottom: 0.25rem">
        Welcome back, {{ auth.user?.fullName?.split(' ')[0] }}
      </h1>
      <p class="text-muted">{{ auth.user?.companyName }}</p>
    </div>
    <Button label="Browse products" icon="pi pi-box" @click="router.push('/products')" />
  </div>

  <div class="stat-grid">
    <div class="stat-card">
      <div class="stat-icon" style="background: #e7f6ee; color: #00833f"><i class="pi pi-shopping-bag" /></div>
      <div>
        <div class="stat-value">{{ ordersTotal }}</div>
        <div class="stat-label">Total orders</div>
      </div>
    </div>
    <div class="stat-card">
      <div class="stat-icon" style="background: #fff4e5; color: #c2410c"><i class="pi pi-flag" /></div>
      <div>
        <div class="stat-value">{{ openClaims }}</div>
        <div class="stat-label">Open claims</div>
      </div>
    </div>
    <div class="stat-card">
      <div class="stat-icon" style="background: #e8f1fb; color: #1d4ed8"><i class="pi pi-shopping-cart" /></div>
      <div>
        <div class="stat-value">{{ cart.count }}</div>
        <div class="stat-label">Items in cart</div>
      </div>
    </div>
    <div class="stat-card">
      <div class="stat-icon" style="background: #f3e8ff; color: #7e22ce"><i class="pi pi-wallet" /></div>
      <div>
        <div class="stat-value">{{ formatCurrency(cart.total) }}</div>
        <div class="stat-label">Cart total</div>
      </div>
    </div>
  </div>

  <div class="dash-section">
    <div class="dash-section-head">
      <h2 class="section-title">Recent orders</h2>
      <RouterLink to="/orders" class="text-muted" style="font-weight: 600">View all →</RouterLink>
    </div>

    <div v-if="loading" class="state"><ProgressSpinner style="width: 42px; height: 42px" /></div>

    <div v-else-if="recentOrders.length === 0" class="state">
      No orders yet.
      <RouterLink to="/products" style="color: var(--brand-dark)">Place your first order</RouterLink>
    </div>

    <DataTable v-else :value="recentOrders" data-key="id">
      <Column field="orderNumber" header="Order #" />
      <Column header="Placed">
        <template #body="{ data }">{{ formatDate(data.createdAtUtc) }}</template>
      </Column>
      <Column header="Status">
        <template #body="{ data }">
          <Tag :value="data.status" :severity="orderStatusSeverity(data.status)" />
        </template>
      </Column>
      <Column header="Total">
        <template #body="{ data }">{{ formatCurrency(data.totalAmount, data.currency) }}</template>
      </Column>
      <Column style="width: 6rem">
        <template #body="{ data }">
          <Button label="View" text @click="router.push({ name: 'order-detail', params: { id: data.id } })" />
        </template>
      </Column>
    </DataTable>
  </div>
</template>
