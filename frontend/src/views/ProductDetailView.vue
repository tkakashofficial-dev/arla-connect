<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { productsService } from '@/services/products.service'
import { useCartStore } from '@/stores/cart'
import { getApiErrorMessage } from '@/services/http'
import { formatCurrency } from '@/utils/format'
import { productVisual } from '@/utils/productImage'
import type { Product } from '@/types'

const props = defineProps<{ id: string }>()
const cart = useCartStore()
const toast = useToast()

const product = ref<Product | null>(null)
const loading = ref(true)
const quantity = ref(1)

async function load() {
  loading.value = true
  try {
    product.value = await productsService.getProduct(props.id)
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Could not load product', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    loading.value = false
  }
}

function addToCart() {
  if (!product.value) return
  cart.add(product.value, quantity.value)
  toast.add({ severity: 'success', summary: 'Added to cart', detail: `${quantity.value} × ${product.value.name}`, life: 2000 })
}

onMounted(load)
</script>

<template>
  <div v-if="loading" class="state"><ProgressSpinner style="width: 48px; height: 48px" /></div>

  <template v-else-if="product">
    <RouterLink to="/products" class="text-muted">&larr; Back to products</RouterLink>

    <div class="product-detail">
      <div class="pd-media" :style="{ background: productVisual(product.sku).background }">
        <span class="pd-emoji">{{ productVisual(product.sku).emoji }}</span>
      </div>

      <div class="pd-info">
        <span class="pd-cat">{{ product.categoryName }}</span>
        <h1 class="page-title" style="margin: 0.4rem 0 0.25rem">{{ product.name }}</h1>
        <p class="text-muted" style="margin: 0">{{ product.sku }}</p>
        <p style="margin: 1rem 0">{{ product.description }}</p>

        <p class="pd-price">{{ formatCurrency(product.unitPrice, product.currency) }}</p>
        <p class="p-card__stock" :class="{ low: product.stockQuantity < 50 }">
          {{ product.stockQuantity }} in stock
        </p>

        <div class="pd-actions">
          <InputNumber
            v-model="quantity"
            :min="1"
            :max="product.stockQuantity"
            show-buttons
            button-layout="horizontal"
            style="width: 9rem"
          />
          <Button
            label="Add to cart"
            icon="pi pi-shopping-cart"
            :disabled="product.stockQuantity === 0"
            @click="addToCart"
          />
        </div>
      </div>
    </div>
  </template>

  <div v-else class="state">Product not found.</div>
</template>
