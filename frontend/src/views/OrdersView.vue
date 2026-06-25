<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { ordersService } from '@/services/orders.service'
import { getApiErrorMessage } from '@/services/http'
import { formatCurrency, formatDate, orderStatusSeverity } from '@/utils/format'
import type { Order } from '@/types'

const router = useRouter()
const toast = useToast()

const orders = ref<Order[]>([])
const loading = ref(false)

async function load() {
  loading.value = true
  try {
    const result = await ordersService.getMyOrders({ page: 1, pageSize: 50 })
    orders.value = result.items
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Could not load orders', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    loading.value = false
  }
}

function open(order: Order) {
  router.push({ name: 'order-detail', params: { id: order.id } })
}

onMounted(load)
</script>

<template>
  <h1 class="page-title">Orders</h1>

  <div v-if="loading" class="state"><ProgressSpinner style="width: 48px; height: 48px" /></div>

  <div v-else-if="orders.length === 0" class="state">
    You have no orders yet.
    <RouterLink to="/products" style="color: var(--brand-dark)">Start shopping</RouterLink>
  </div>

  <DataTable v-else :value="orders" data-key="id" @row-click="(e: { data: Order }) => open(e.data)">
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
        <Button label="View" text @click.stop="open(data)" />
      </template>
    </Column>
  </DataTable>
</template>
