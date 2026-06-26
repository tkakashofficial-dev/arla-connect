<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { useCartStore } from '@/stores/cart'
import { useAuthStore } from '@/stores/auth'
import { ordersService } from '@/services/orders.service'
import { getApiErrorMessage } from '@/services/http'
import { formatCurrency } from '@/utils/format'

const cart = useCartStore()
const auth = useAuthStore()
const router = useRouter()
const toast = useToast()

const placing = ref(false)

async function placeOrder() {
  if (!auth.isAuthenticated) {
    router.push({ name: 'login', query: { redirect: '/cart' } })
    return
  }

  placing.value = true
  try {
    const order = await ordersService.createOrder({
      lines: cart.items.map((i) => ({ productId: i.product.id, quantity: i.quantity })),
    })
    cart.clear()
    toast.add({ severity: 'success', summary: 'Order placed', detail: order.orderNumber, life: 3000 })
    router.push({ name: 'order-detail', params: { id: order.id } })
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Could not place order', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    placing.value = false
  }
}
</script>

<template>
  <h1 class="page-title">Your cart</h1>

  <div v-if="cart.isEmpty" class="state">
    Your cart is empty.
    <RouterLink to="/products" style="color: var(--brand-dark)">Browse products</RouterLink>
  </div>

  <template v-else>
    <DataTable :value="cart.items" data-key="product.id">
      <Column header="Product">
        <template #body="{ data }">
          <div class="cart-product">
            <span class="cart-thumb">
              <ProductImage :sku="data.product.sku" :image-url="data.product.imageUrl" />
            </span>
            <div>
              <strong>{{ data.product.name }}</strong>
              <div class="text-muted" style="font-size: 0.8rem">{{ data.product.sku }}</div>
            </div>
          </div>
        </template>
      </Column>
      <Column header="Unit price">
        <template #body="{ data }">{{ formatCurrency(data.product.unitPrice, data.product.currency) }}</template>
      </Column>
      <Column header="Qty" style="width: 9rem">
        <template #body="{ data }">
          <InputNumber
            :model-value="data.quantity"
            :min="1"
            :max="data.product.stockQuantity"
            show-buttons
            button-layout="horizontal"
            style="width: 8rem"
            @update:model-value="(v: number) => cart.updateQuantity(data.product.id, v)"
          />
        </template>
      </Column>
      <Column header="Line total">
        <template #body="{ data }">
          {{ formatCurrency(data.product.unitPrice * data.quantity, data.product.currency) }}
        </template>
      </Column>
      <Column style="width: 4rem">
        <template #body="{ data }">
          <Button icon="pi pi-trash" severity="danger" text rounded @click="cart.remove(data.product.id)" />
        </template>
      </Column>
    </DataTable>

    <div class="summary-bar">
      <span class="text-muted">Total</span>
      <strong style="font-size: 1.25rem">{{ formatCurrency(cart.total) }}</strong>
      <Button label="Place order" icon="pi pi-check" :loading="placing" @click="placeOrder" />
    </div>
  </template>
</template>
