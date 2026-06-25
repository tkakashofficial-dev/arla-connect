<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import { adminProductsService } from '@/services/admin.products.service'
import { productsService } from '@/services/products.service'
import { getApiErrorMessage } from '@/services/http'
import { formatCurrency } from '@/utils/format'
import { productVisual } from '@/utils/productImage'
import type { Category, Product } from '@/types'

const toast = useToast()
const confirm = useConfirm()

const products = ref<Product[]>([])
const categories = ref<Category[]>([])
const search = ref('')
const loading = ref(false)
let searchTimer: ReturnType<typeof setTimeout> | undefined

const dialogVisible = ref(false)
const saving = ref(false)
const editingId = ref<string | null>(null)
const form = ref({
  sku: '',
  name: '',
  description: '',
  unitPrice: 0,
  currency: 'DKK',
  stockQuantity: 0,
  categoryId: '' as string,
  isActive: true,
})

async function load() {
  loading.value = true
  try {
    const result = await adminProductsService.getAll({ search: search.value || undefined, pageSize: 100 })
    products.value = result.items
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Could not load products', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    loading.value = false
  }
}

function openCreate() {
  editingId.value = null
  form.value = { sku: '', name: '', description: '', unitPrice: 0, currency: 'DKK', stockQuantity: 0, categoryId: categories.value[0]?.id ?? '', isActive: true }
  dialogVisible.value = true
}

function openEdit(p: Product) {
  editingId.value = p.id
  form.value = {
    sku: p.sku,
    name: p.name,
    description: p.description ?? '',
    unitPrice: p.unitPrice,
    currency: p.currency,
    stockQuantity: p.stockQuantity,
    categoryId: p.categoryId,
    isActive: p.isActive,
  }
  dialogVisible.value = true
}

async function save() {
  saving.value = true
  try {
    if (editingId.value) {
      await adminProductsService.update(editingId.value, {
        name: form.value.name,
        description: form.value.description || null,
        unitPrice: form.value.unitPrice,
        currency: form.value.currency,
        stockQuantity: form.value.stockQuantity,
        categoryId: form.value.categoryId,
        isActive: form.value.isActive,
      })
      toast.add({ severity: 'success', summary: 'Product updated', life: 2500 })
    } else {
      await adminProductsService.create({
        sku: form.value.sku,
        name: form.value.name,
        description: form.value.description || null,
        unitPrice: form.value.unitPrice,
        currency: form.value.currency,
        stockQuantity: form.value.stockQuantity,
        categoryId: form.value.categoryId,
      })
      toast.add({ severity: 'success', summary: 'Product created', life: 2500 })
    }
    dialogVisible.value = false
    await load()
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Could not save', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    saving.value = false
  }
}

function confirmDelete(p: Product) {
  confirm.require({
    header: 'Deactivate product',
    message: `Deactivate "${p.name}"? It will be hidden from the catalogue.`,
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Deactivate',
    acceptProps: { severity: 'danger' },
    rejectLabel: 'Cancel',
    accept: async () => {
      try {
        await adminProductsService.remove(p.id)
        toast.add({ severity: 'success', summary: 'Product deactivated', life: 2500 })
        await load()
      } catch (e) {
        toast.add({ severity: 'error', summary: 'Could not delete', detail: getApiErrorMessage(e), life: 4000 })
      }
    },
  })
}

function onSearch() {
  clearTimeout(searchTimer)
  searchTimer = setTimeout(load, 300)
}

onMounted(async () => {
  categories.value = await productsService.getCategories().catch(() => [])
  await load()
})
</script>

<template>
  <div class="dash-head">
    <p class="text-muted" style="margin: 0">{{ products.length }} products in the catalogue</p>
    <Button label="New product" icon="pi pi-plus" @click="openCreate" />
  </div>

  <div class="filter-bar">
    <IconField class="search-field">
      <InputIcon class="pi pi-search" />
      <InputText v-model="search" placeholder="Search by name or SKU…" class="full-width" @input="onSearch" />
    </IconField>
  </div>

  <div v-if="loading" class="state"><ProgressSpinner style="width: 42px; height: 42px" /></div>

  <DataTable v-else :value="products" data-key="id" striped-rows paginator :rows="15">
    <Column header="" style="width: 4rem">
      <template #body="{ data }">
        <span class="cart-thumb" :style="{ background: productVisual(data.sku).background }">
          {{ productVisual(data.sku).emoji }}
        </span>
      </template>
    </Column>
    <Column field="sku" header="SKU" />
    <Column field="name" header="Name" />
    <Column field="categoryName" header="Category" />
    <Column header="Price">
      <template #body="{ data }">{{ formatCurrency(data.unitPrice, data.currency) }}</template>
    </Column>
    <Column field="stockQuantity" header="Stock" />
    <Column header="Status">
      <template #body="{ data }">
        <Tag :value="data.isActive ? 'Active' : 'Inactive'" :severity="data.isActive ? 'success' : 'secondary'" />
      </template>
    </Column>
    <Column header="" style="width: 7rem">
      <template #body="{ data }">
        <div class="row-actions">
          <Button icon="pi pi-pencil" text rounded aria-label="Edit" @click="openEdit(data)" />
          <Button icon="pi pi-trash" text rounded severity="danger" aria-label="Delete" @click="confirmDelete(data)" />
        </div>
      </template>
    </Column>
  </DataTable>

  <Dialog v-model:visible="dialogVisible" modal :header="editingId ? 'Edit product' : 'New product'" :style="{ width: '560px' }">
    <div class="form-grid">
      <div class="auth-field span-2">
        <label>Name</label>
        <InputText v-model="form.name" class="full-width" />
      </div>
      <div class="auth-field">
        <label>SKU</label>
        <InputText v-model="form.sku" class="full-width" :disabled="!!editingId" />
      </div>
      <div class="auth-field">
        <label>Category</label>
        <Select v-model="form.categoryId" :options="categories" option-label="name" option-value="id" class="full-width" />
      </div>
      <div class="auth-field">
        <label>Unit price ({{ form.currency }})</label>
        <InputNumber v-model="form.unitPrice" :min-fraction-digits="2" :max-fraction-digits="2" class="full-width" />
      </div>
      <div class="auth-field">
        <label>Stock quantity</label>
        <InputNumber v-model="form.stockQuantity" :min="0" class="full-width" />
      </div>
      <div class="auth-field span-2">
        <label>Description</label>
        <Textarea v-model="form.description" rows="3" auto-resize class="full-width" />
      </div>
      <div v-if="editingId" class="auth-field">
        <label>Status</label>
        <Select
          v-model="form.isActive"
          :options="[{ label: 'Active', value: true }, { label: 'Inactive', value: false }]"
          option-label="label"
          option-value="value"
          class="full-width"
        />
      </div>
    </div>
    <template #footer>
      <Button label="Cancel" text @click="dialogVisible = false" />
      <Button
        :label="editingId ? 'Save changes' : 'Create product'"
        :loading="saving"
        :disabled="!form.name || !form.sku || !form.categoryId"
        @click="save"
      />
    </template>
  </Dialog>
</template>
