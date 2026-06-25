<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { adminDashboardService } from '@/services/admin.dashboard.service'
import { formatCurrency } from '@/utils/format'
import type { AdminSummary } from '@/types'

const summary = ref<AdminSummary | null>(null)
const loading = ref(true)

const doughnutData = computed(() => ({
  labels: summary.value?.ordersByStatus.map((s) => s.status) ?? [],
  datasets: [
    {
      data: summary.value?.ordersByStatus.map((s) => s.count) ?? [],
      backgroundColor: ['#fbbf24', '#60a5fa', '#34d399', '#00a651', '#f87171'],
      borderWidth: 0,
    },
  ],
}))
const doughnutOptions = {
  responsive: true,
  maintainAspectRatio: false,
  cutout: '62%',
  plugins: { legend: { position: 'bottom', labels: { usePointStyle: true, padding: 16 } } },
}

onMounted(async () => {
  try {
    summary.value = await adminDashboardService.getSummary()
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div v-if="loading" class="state"><ProgressSpinner style="width: 42px; height: 42px" /></div>

  <template v-else-if="summary">
    <div class="stat-grid">
      <div class="stat-card">
        <div class="stat-icon" style="background: #e7f6ee; color: #00833f"><i class="pi pi-shopping-bag" /></div>
        <div><div class="stat-value">{{ summary.totalOrders }}</div><div class="stat-label">Total orders</div></div>
      </div>
      <div class="stat-card">
        <div class="stat-icon" style="background: #ede9fe; color: #6d28d9"><i class="pi pi-wallet" /></div>
        <div><div class="stat-value">{{ formatCurrency(summary.totalRevenue) }}</div><div class="stat-label">Revenue</div></div>
      </div>
      <div class="stat-card">
        <div class="stat-icon" style="background: #fff4e5; color: #c2410c"><i class="pi pi-flag" /></div>
        <div><div class="stat-value">{{ summary.openClaims }}</div><div class="stat-label">Open claims</div></div>
      </div>
      <div class="stat-card">
        <div class="stat-icon" style="background: #e8f1fb; color: #1d4ed8"><i class="pi pi-box" /></div>
        <div><div class="stat-value">{{ summary.activeProducts }}</div><div class="stat-label">Active products</div></div>
      </div>
      <div class="stat-card">
        <div class="stat-icon" style="background: #fef2f2; color: #dc2626"><i class="pi pi-exclamation-triangle" /></div>
        <div><div class="stat-value">{{ summary.lowStockProducts }}</div><div class="stat-label">Low stock</div></div>
      </div>
      <div class="stat-card">
        <div class="stat-icon" style="background: #ecfeff; color: #0891b2"><i class="pi pi-building" /></div>
        <div><div class="stat-value">{{ summary.totalCustomers }}</div><div class="stat-label">Customers</div></div>
      </div>
    </div>

    <div class="panel" style="max-width: 520px">
      <h3 class="section-title">Orders by status</h3>
      <div class="chart-box"><Chart type="doughnut" :data="doughnutData" :options="doughnutOptions" /></div>
    </div>
  </template>
</template>
