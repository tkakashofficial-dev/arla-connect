<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { productsService } from '@/services/products.service'
import { useCartStore } from '@/stores/cart'
import { getApiErrorMessage } from '@/services/http'
import { formatCurrency } from '@/utils/format'
import type { Category, Product } from '@/types'

const cart = useCartStore()
const toast = useToast()
const router = useRouter()

function goToDetail(id: string) {
  router.push({ name: 'product-detail', params: { id } })
}

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
  <section class="hero">
    <h1>Order Arla products, the easy way.</h1>
    <p>Browse the catalogue, place orders, and manage claims — all from one self-service portal.</p>
    <div class="trust-row">
      <span class="trust-chip">🇩🇰 Danish dairy</span>
      <span class="trust-chip">🚚 Next-day delivery</span>
      <span class="trust-chip">🔒 Secure B2B ordering</span>
    </div>
  </section>

  <div class="filter-bar">
    <IconField class="search-field">
      <InputIcon class="pi pi-search" />
      <InputText v-model="search" placeholder="Search products…" class="full-width" />
    </IconField>
    <div class="cat-chips">
      <button class="chip" :class="{ active: categoryId === null }" @click="categoryId = null">All</button>
      <button
        v-for="c in categories"
        :key="c.id"
        class="chip"
        :class="{ active: categoryId === c.id }"
        @click="categoryId = c.id"
      >
        {{ c.name }}
      </button>
    </div>
  </div>

  <p v-if="!loading" class="result-count text-muted">{{ totalCount }} product{{ totalCount === 1 ? '' : 's' }}</p>

  <div v-if="loading" class="state"><ProgressSpinner style="width: 48px; height: 48px" /></div>

  <div v-else-if="products.length === 0" class="state">No products match your search.</div>

  <div v-else class="product-grid">
    <article
      v-for="product in products"
      :key="product.id"
      class="p-card clickable"
      @click="goToDetail(product.id)"
    >
      <div class="p-card__media">
        <ProductImage :sku="product.sku" :image-url="product.imageUrl" />
        <span class="p-card__cat">{{ product.categoryName }}</span>
      </div>
      <div class="p-card__body">
        <h3 class="p-card__name">{{ product.name }}</h3>
        <p class="p-card__sku">{{ product.sku }}</p>
        <p class="p-card__desc">{{ product.description }}</p>
        <div class="p-card__row">
          <span class="p-card__price">{{ formatCurrency(product.unitPrice, product.currency) }}</span>
          <span class="p-card__stock" :class="{ low: product.stockQuantity < 50 }">
            {{ product.stockQuantity }} in stock
          </span>
        </div>
        <Button
          label="Add to cart"
          icon="pi pi-shopping-cart"
          class="full-width"
          :disabled="product.stockQuantity === 0"
          @click.stop="addToCart(product)"
        />
      </div>
    </article>
  </div>

  <Paginator
    v-if="totalCount > pageSize"
    :rows="pageSize"
    :total-records="totalCount"
    :first="(page - 1) * pageSize"
    :rows-per-page-options="[12, 24, 48]"
    @page="onPage"
    style="margin-top: 1.5rem"
  />
</template>
