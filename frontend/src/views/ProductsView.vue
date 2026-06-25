<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useToast } from 'primevue/usetoast'
import { productsService } from '@/services/products.service'
import { useCartStore } from '@/stores/cart'
import { getApiErrorMessage } from '@/services/http'
import { formatCurrency } from '@/utils/format'
import type { Category, Product } from '@/types'

const cart = useCartStore()
const toast = useToast()

const products = ref<Product[]>([])
const categories = ref<Category[]>([])
const search = ref('')
const categoryId = ref<string | null>(null)
const page = ref(1)
const pageSize = ref(12)
const totalCount = ref(0)
const loading = ref(false)

let searchTimer: ReturnType<typeof setTimeout> | undefined

async function load() {
  loading.value = true
  try {
    const result = await productsService.getProducts({
      search: search.value || undefined,
      categoryId: categoryId.value || undefined,
      page: page.value,
      pageSize: pageSize.value,
    })
    products.value = result.items
    totalCount.value = result.totalCount
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Could not load products', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    loading.value = false
  }
}

function addToCart(product: Product) {
  cart.add(product)
  toast.add({ severity: 'success', summary: 'Added to cart', detail: product.name, life: 1800 })
}

function onPage(event: { page: number; rows: number }) {
  page.value = event.page + 1
  pageSize.value = event.rows
  load()
}

watch([search, categoryId], () => {
  clearTimeout(searchTimer)
  searchTimer = setTimeout(() => {
    page.value = 1
    load()
  }, 300)
})

onMounted(async () => {
  categories.value = await productsService.getCategories().catch(() => [])
  await load()
})
</script>

<template>
  <h1 class="page-title">Products</h1>

  <div class="toolbar-row">
    <InputText v-model="search" placeholder="Search products…" style="min-width: 240px" />
    <Select
      v-model="categoryId"
      :options="categories"
      option-label="name"
      option-value="id"
      placeholder="All categories"
      show-clear
      style="min-width: 220px"
    />
  </div>

  <div v-if="loading" class="state"><ProgressSpinner style="width: 48px; height: 48px" /></div>

  <div v-else-if="products.length === 0" class="state">No products match your search.</div>

  <div v-else class="product-grid">
    <Card v-for="product in products" :key="product.id" class="product-card">
      <template #title>{{ product.name }}</template>
      <template #subtitle><span class="sku">{{ product.sku }} · {{ product.categoryName }}</span></template>
      <template #content>
        <p class="desc">{{ product.description }}</p>
        <p class="price">{{ formatCurrency(product.unitPrice, product.currency) }}</p>
        <p class="text-muted" style="font-size: 0.85rem">{{ product.stockQuantity }} in stock</p>
      </template>
      <template #footer>
        <Button
          label="Add to cart"
          icon="pi pi-shopping-cart"
          class="full-width"
          :disabled="product.stockQuantity === 0"
          @click="addToCart(product)"
        />
      </template>
    </Card>
  </div>

  <Paginator
    v-if="totalCount > pageSize"
    :rows="pageSize"
    :total-records="totalCount"
    :first="(page - 1) * pageSize"
    :rows-per-page-options="[12, 24, 48]"
    @page="onPage"
  />
</template>
