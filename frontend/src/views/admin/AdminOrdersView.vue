<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { adminOrdersService } from '@/services/admin.orders.service'
import { getApiErrorMessage } from '@/services/http'
import { formatCurrency, formatDate } from '@/utils/format'
import type { AdminOrder, OrderStatus } from '@/types'

const toast = useToast()
const orders = ref<AdminOrder[]>([])
const loading = ref(false)

const statuses: OrderStatus[] = ['Pending', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled']

async function load() {
  loading.value = true
  try {
    orders.value = (await adminOrdersService.getAll({ pageSize: 100 })).items
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Could not load orders', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    loading.value = false
  }
}

async function changeStatus(order: AdminOrder, status: OrderStatus) {
  const previous = order.status
  order.status = status
  try {
    await adminOrdersService.updateStatus(order.id, status)
    toast.add({ severity: 'success', summary: 'Order updated', detail: `${order.orderNumber} → ${status}`, life: 2000 })
  } catch (e) {
    order.status = previous
    toast.add({ severity: 'error', summary: 'Update failed', detail: getApiErrorMessage(e), life: 4000 })
  }
}

onMounted(load)
</script>

<template>
  <div v-if="loading" class="state"><ProgressSpinner style="width: 42px; height: 42px" /></div>

  <DataTable v-else :value="orders" data-key="id" striped-rows paginator :rows="15">
    <Column field="orderNumber" header="Order #" />
    <Column field="customerName" header="Customer" />
    <Column header="Placed"><template #body="{ data }">{{ formatDate(data.createdAtUtc) }}</template></Column>
    <Column header="Total"><template #body="{ data }">{{ formatCurrency(data.totalAmount, data.currency) }}</template></Column>
    <Column header="Status" style="width: 13rem">
      <template #body="{ data }">
        <Select
          :model-value="data.status"
          :options="statuses"
          class="full-width"
          @update:model-value="(s: OrderStatus) => changeStatus(data, s)"
        />
      </template>
    </Column>
  </DataTable>
</template>
