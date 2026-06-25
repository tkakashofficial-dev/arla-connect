<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { claimsService } from '@/services/claims.service'
import { getApiErrorMessage } from '@/services/http'
import { claimStatusSeverity, formatDate } from '@/utils/format'
import type { Claim } from '@/types'

const toast = useToast()
const claims = ref<Claim[]>([])
const loading = ref(false)

async function load() {
  loading.value = true
  try {
    const result = await claimsService.getMyClaims({ page: 1, pageSize: 50 })
    claims.value = result.items
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Could not load claims', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <h1 class="page-title">Claims</h1>

  <div v-if="loading" class="state"><ProgressSpinner style="width: 48px; height: 48px" /></div>

  <div v-else-if="claims.length === 0" class="state">
    No claims yet. You can raise a claim from an order's detail page.
  </div>

  <DataTable v-else :value="claims" data-key="id">
    <Column field="claimNumber" header="Claim #" />
    <Column field="orderNumber" header="Order #" />
    <Column field="reason" header="Reason" />
    <Column header="Status">
      <template #body="{ data }">
        <Tag :value="data.status" :severity="claimStatusSeverity(data.status)" />
      </template>
    </Column>
    <Column header="Raised">
      <template #body="{ data }">{{ formatDate(data.createdAtUtc) }}</template>
    </Column>
    <Column field="description" header="Description" />
  </DataTable>
</template>
