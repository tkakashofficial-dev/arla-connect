<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { adminCustomersService } from '@/services/admin.customers.service'
import { getApiErrorMessage } from '@/services/http'
import { formatCurrency, formatDate, orderStatusSeverity } from '@/utils/format'
import type { AdminCustomerDetail, AdminCustomerListItem } from '@/types'

const toast = useToast()
const customers = ref<AdminCustomerListItem[]>([])
const search = ref('')
const loading = ref(false)
let searchTimer: ReturnType<typeof setTimeout> | undefined

const detail = ref<AdminCustomerDetail | null>(null)
const detailVisible = ref(false)
const detailLoading = ref(false)

async function load() {
  loading.value = true
  try {
    customers.value = (await adminCustomersService.getAll({ search: search.value || undefined, pageSize: 100 })).items
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Could not load customers', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    loading.value = false
  }
}

async function openDetail(c: AdminCustomerListItem) {
  detailVisible.value = true
  detailLoading.value = true
  detail.value = null
  try {
    detail.value = await adminCustomersService.getById(c.id)
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Could not load customer', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    detailLoading.value = false
  }
}

function onSearch() {
  clearTimeout(searchTimer)
  searchTimer = setTimeout(load, 300)
}

onMounted(load)
</script>

<template>
  <div class="filter-bar">
    <IconField class="search-field">
      <InputIcon class="pi pi-search" />
      <InputText v-model="search" placeholder="Search by name or customer number…" class="full-width" @input="onSearch" />
    </IconField>
  </div>

  <div v-if="loading" class="state"><ProgressSpinner style="width: 42px; height: 42px" /></div>

  <DataTable v-else :value="customers" data-key="id" striped-rows paginator :rows="15">
    <Column field="name" header="Company" />
    <Column field="customerNumber" header="Customer #" />
    <Column field="userCount" header="Users" />
    <Column field="orderCount" header="Orders" />
    <Column header="Total spend">
      <template #body="{ data }"><strong>{{ formatCurrency(data.totalSpend) }}</strong></template>
    </Column>
    <Column header="Customer since">
      <template #body="{ data }">{{ formatDate(data.createdAtUtc) }}</template>
    </Column>
    <Column style="width: 6rem">
      <template #body="{ data }"><Button label="View" text @click="openDetail(data)" /></template>
    </Column>
  </DataTable>

  <Dialog v-model:visible="detailVisible" modal :header="detail?.name ?? 'Customer'" :style="{ width: '640px' }">
    <div v-if="detailLoading" class="state"><ProgressSpinner style="width: 36px; height: 36px" /></div>

    <template v-else-if="detail">
      <div class="stat-grid" style="margin-bottom: 1.25rem">
        <div class="stat-card">
          <div class="stat-icon" style="background: #ede9fe; color: #6d28d9"><i class="pi pi-wallet" /></div>
          <div><div class="stat-value">{{ formatCurrency(detail.totalSpend) }}</div><div class="stat-label">Total spend</div></div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background: #e7f6ee; color: #00833f"><i class="pi pi-shopping-bag" /></div>
          <div><div class="stat-value">{{ detail.orderCount }}</div><div class="stat-label">Orders</div></div>
        </div>
      </div>

      <h3 class="section-title" style="margin-bottom: 0.5rem">Users</h3>
      <DataTable :value="detail.users" data-key="id" striped-rows class="mb-section">
        <Column field="fullName" header="Name" />
        <Column field="email" header="Email" />
        <Column header="Role"><template #body="{ data }"><Tag :value="data.role" severity="info" /></template></Column>
      </DataTable>

      <h3 class="section-title" style="margin: 1.25rem 0 0.5rem">Recent orders</h3>
      <DataTable :value="detail.recentOrders" data-key="id" striped-rows>
        <Column field="orderNumber" header="Order #" />
        <Column header="Placed"><template #body="{ data }">{{ formatDate(data.createdAtUtc) }}</template></Column>
        <Column header="Status">
          <template #body="{ data }"><Tag :value="data.status" :severity="orderStatusSeverity(data.status)" /></template>
        </Column>
        <Column header="Total"><template #body="{ data }">{{ formatCurrency(data.totalAmount) }}</template></Column>
      </DataTable>
    </template>
  </Dialog>
</template>
