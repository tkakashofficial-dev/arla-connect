<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { ordersService } from '@/services/orders.service'
import { claimsService } from '@/services/claims.service'
import { getApiErrorMessage } from '@/services/http'
import { formatCurrency, formatDate, orderStatusSeverity } from '@/utils/format'
import type { ClaimReason, Order } from '@/types'

const props = defineProps<{ id: string }>()
const toast = useToast()

const order = ref<Order | null>(null)
const loading = ref(false)

// Claim dialog state
const claimVisible = ref(false)
const claimReason = ref<ClaimReason | null>(null)
const claimDescription = ref('')
const submittingClaim = ref(false)

const reasonOptions: { label: string; value: ClaimReason }[] = [
  { label: 'Damaged goods', value: 'DamagedGoods' },
  { label: 'Wrong item', value: 'WrongItem' },
  { label: 'Missing item', value: 'MissingItem' },
  { label: 'Quality issue', value: 'QualityIssue' },
  { label: 'Other', value: 'Other' },
]

async function load() {
  loading.value = true
  try {
    order.value = await ordersService.getOrder(props.id)
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Could not load order', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    loading.value = false
  }
}

async function submitClaim() {
  if (!order.value || !claimReason.value) return
  submittingClaim.value = true
  try {
    const claim = await claimsService.createClaim({
      orderId: order.value.id,
      reason: claimReason.value,
      description: claimDescription.value,
    })
    toast.add({ severity: 'success', summary: 'Claim submitted', detail: claim.claimNumber, life: 3000 })
    claimVisible.value = false
    claimReason.value = null
    claimDescription.value = ''
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Could not submit claim', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    submittingClaim.value = false
  }
}

onMounted(load)
</script>

<template>
  <div v-if="loading" class="state"><ProgressSpinner style="width: 48px; height: 48px" /></div>

  <template v-else-if="order">
    <RouterLink to="/orders" class="text-muted">&larr; Back to orders</RouterLink>

    <div style="display: flex; align-items: center; gap: 1rem; margin: 0.5rem 0 1rem">
      <h1 class="page-title" style="margin: 0">{{ order.orderNumber }}</h1>
      <Tag :value="order.status" :severity="orderStatusSeverity(order.status)" />
    </div>
    <p class="text-muted">Placed {{ formatDate(order.createdAtUtc) }}</p>

    <DataTable :value="order.lines" data-key="productId">
      <Column header="Product">
        <template #body="{ data }">
          <div class="cart-product">
            <span class="cart-thumb"><ProductImage :sku="data.sku" :image-url="data.imageUrl" /></span>
            <div>
              <strong>{{ data.productName }}</strong>
              <div class="text-muted" style="font-size: 0.8rem">{{ data.sku }}</div>
            </div>
          </div>
        </template>
      </Column>
      <Column header="Unit price">
        <template #body="{ data }">{{ formatCurrency(data.unitPrice, order!.currency) }}</template>
      </Column>
      <Column field="quantity" header="Qty" />
      <Column header="Line total">
        <template #body="{ data }">{{ formatCurrency(data.lineTotal, order!.currency) }}</template>
      </Column>
    </DataTable>

    <div class="summary-bar">
      <span class="text-muted">Order total</span>
      <strong style="font-size: 1.25rem">{{ formatCurrency(order.totalAmount, order.currency) }}</strong>
      <Button label="Raise a claim" icon="pi pi-flag" severity="secondary" @click="claimVisible = true" />
    </div>

    <Dialog v-model:visible="claimVisible" modal header="Raise a claim" :style="{ width: '460px' }">
      <div class="auth-field">
        <label>Reason</label>
        <Select
          v-model="claimReason"
          :options="reasonOptions"
          option-label="label"
          option-value="value"
          placeholder="Select a reason"
          class="full-width"
        />
      </div>
      <div class="auth-field">
        <label>Description</label>
        <Textarea v-model="claimDescription" rows="4" auto-resize class="full-width" />
      </div>
      <template #footer>
        <Button label="Cancel" text @click="claimVisible = false" />
        <Button
          label="Submit claim"
          :loading="submittingClaim"
          :disabled="!claimReason || !claimDescription"
          @click="submitClaim"
        />
      </template>
    </Dialog>
  </template>

  <div v-else class="state">Order not found.</div>
</template>
