<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { ordersService } from '@/services/orders.service'
import { claimsService } from '@/services/claims.service'
import { formatCurrency, formatDate, orderStatusSeverity } from '@/utils/format'
import type { Claim, Order } from '@/types'

const auth = useAuthStore()
const router = useRouter()

const orders = ref<Order[]>([])
const ordersTotal = ref(0)
const claims = ref<Claim[]>([])
const loading = ref(true)

const openClaims = computed(() => claims.value.filter((c) => c.status === 'Open' || c.status === 'UnderReview').length)
const totalSpend = computed(() => orders.value.reduce((sum, o) => sum + o.totalAmount, 0))
const recentOrders = computed(() => orders.value.slice(0, 5))

// --- Monthly spend (bar) ---
const monthlySpend = computed(() => {
  const map = new Map<string, number>()
  for (const o of orders.value) {
    const d = new Date(o.createdAtUtc)
    const key = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}`
    map.set(key, (map.get(key) ?? 0) + o.totalAmount)
  }
  const keys = [...map.keys()].sort()
  return {
    labels: keys.map((k) => {
      const [y, m] = k.split('-')
      return new Date(Number(y), Number(m) - 1).toLocaleString('en', { month: 'short' })
    }),
    values: keys.map((k) => Math.round(map.get(k)!)),
  }
})

// --- Orders by status (doughnut) ---
const byStatus = computed(() => {
  const map = new Map<string, number>()
  for (const o of orders.value) map.set(o.status, (map.get(o.status) ?? 0) + 1)
  return { labels: [...map.keys()], values: [...map.values()] }
})

const barData = computed(() => ({
  labels: monthlySpend.value.labels,
  datasets: [{ label: 'Spend', data: monthlySpend.value.values, backgroundColor: '#00a651', borderRadius: 6, maxBarThickness: 38 }],
}))
const barOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: { legend: { display: false } },
  scales: {
    y: { ticks: { callback: (v: number) => `${v} kr` }, grid: { color: '#eef2f7' }, border: { display: false } },
    x: { grid: { display: false }, border: { display: false } },
  },
}

const doughnutData = computed(() => ({
  labels: byStatus.value.labels,
  datasets: [{ data: byStatus.value.values, backgroundColor: ['#00a651', '#34d399', '#60a5fa', '#fbbf24', '#f87171'], borderWidth: 0 }],
}))
const doughnutOptions = {
  responsive: true,
  maintainAspectRatio: false,
  cutout: '62%',
  plugins: { legend: { position: 'bottom', labels: { usePointStyle: true, padding: 16 } } },
}

onMounted(async () => {
  try {
    const [o, c] = await Promise.all([
      ordersService.getMyOrders({ page: 1, pageSize: 50 }),
      claimsService.getMyClaims({ page: 1, pageSize: 50 }),
    ])
    orders.value = o.items
    ordersTotal.value = o.totalCount
    claims.value = c.items
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div class="dash-head">
    <div>
      <h2 style="margin: 0 0 0.25rem; font-size: 1.4rem">Welcome back, {{ auth.user?.fullName?.split(' ')[0] }}</h2>
      <p class="text-muted" style="margin: 0">{{ auth.user?.companyName }}</p>
    </div>
    <Button label="Browse products" icon="pi pi-box" @click="router.push('/products')" />
  </div>

  <div class="stat-grid">
    <div class="stat-card">
      <div class="stat-icon" style="background: #e7f6ee; color: #00833f"><i class="pi pi-shopping-bag" /></div>
      <div><div class="stat-value">{{ ordersTotal }}</div><div class="stat-label">Total orders</div></div>
    </div>
    <div class="stat-card">
      <div class="stat-icon" style="background: #ede9fe; color: #6d28d9"><i class="pi pi-wallet" /></div>
      <div><div class="stat-value">{{ formatCurrency(totalSpend) }}</div><div class="stat-label">Total spend</div></div>
    </div>
    <div class="stat-card">
      <div class="stat-icon" style="background: #fff4e5; color: #c2410c"><i class="pi pi-flag" /></div>
      <div><div class="stat-value">{{ openClaims }}</div><div class="stat-label">Open claims</div></div>
    </div>
    <div class="stat-card">
      <div class="stat-icon" style="background: #e8f1fb; color: #1d4ed8"><i class="pi pi-box" /></div>
      <div><div class="stat-value">{{ orders.length ? Math.round(totalSpend / orders.length) : 0 }} kr</div><div class="stat-label">Avg. order value</div></div>
    </div>
  </div>

  <div v-if="loading" class="state"><ProgressSpinner style="width: 42px; height: 42px" /></div>

  <template v-else>
    <div class="chart-grid">
      <div class="panel">
        <h3 class="section-title">Monthly spend</h3>
        <div class="chart-box"><Chart type="bar" :data="barData" :options="barOptions" /></div>
      </div>
      <div class="panel">
        <h3 class="section-title">Orders by status</h3>
        <div class="chart-box"><Chart type="doughnut" :data="doughnutData" :options="doughnutOptions" /></div>
      </div>
    </div>

    <div class="panel">
      <div class="dash-section-head">
        <h3 class="section-title">Recent orders</h3>
        <RouterLink to="/orders" class="text-muted" style="font-weight: 600">View all →</RouterLink>
      </div>
      <div v-if="recentOrders.length === 0" class="state">
        No orders yet. <RouterLink to="/products" style="color: var(--brand-dark)">Place your first order</RouterLink>
      </div>
      <DataTable v-else :value="recentOrders" data-key="id" striped-rows>
        <Column field="orderNumber" header="Order #" />
        <Column header="Placed"><template #body="{ data }">{{ formatDate(data.createdAtUtc) }}</template></Column>
        <Column header="Status">
          <template #body="{ data }"><Tag :value="data.status" :severity="orderStatusSeverity(data.status)" /></template>
        </Column>
        <Column header="Total"><template #body="{ data }">{{ formatCurrency(data.totalAmount, data.currency) }}</template></Column>
        <Column style="width: 6rem">
          <template #body="{ data }">
            <Button label="View" text @click="router.push({ name: 'order-detail', params: { id: data.id } })" />
          </template>
        </Column>
      </DataTable>
    </div>
  </template>
</template>
