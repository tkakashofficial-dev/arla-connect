<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { adminClaimsService } from '@/services/admin.claims.service'
import { getApiErrorMessage } from '@/services/http'
import { formatDate } from '@/utils/format'
import type { AdminClaim, ClaimStatus } from '@/types'

const toast = useToast()
const claims = ref<AdminClaim[]>([])
const loading = ref(false)

const statuses: ClaimStatus[] = ['Open', 'UnderReview', 'Approved', 'Rejected', 'Resolved']

async function load() {
  loading.value = true
  try {
    claims.value = (await adminClaimsService.getAll({ pageSize: 100 })).items
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Could not load claims', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    loading.value = false
  }
}

async function changeStatus(claim: AdminClaim, status: ClaimStatus) {
  const previous = claim.status
  claim.status = status
  try {
    await adminClaimsService.updateStatus(claim.id, status)
    toast.add({ severity: 'success', summary: 'Claim updated', detail: `${claim.claimNumber} → ${status}`, life: 2000 })
  } catch (e) {
    claim.status = previous
    toast.add({ severity: 'error', summary: 'Update failed', detail: getApiErrorMessage(e), life: 4000 })
  }
}

onMounted(load)
</script>

<template>
  <div v-if="loading" class="state"><ProgressSpinner style="width: 42px; height: 42px" /></div>

  <DataTable v-else :value="claims" data-key="id" striped-rows paginator :rows="15">
    <Column field="claimNumber" header="Claim #" />
    <Column field="customerName" header="Customer" />
    <Column field="orderNumber" header="Order #" />
    <Column field="reason" header="Reason" />
    <Column field="description" header="Description" />
    <Column header="Raised"><template #body="{ data }">{{ formatDate(data.createdAtUtc) }}</template></Column>
    <Column header="Status" style="width: 13rem">
      <template #body="{ data }">
        <Select
          :model-value="data.status"
          :options="statuses"
          class="full-width"
          @update:model-value="(s: ClaimStatus) => changeStatus(data, s)"
        />
      </template>
    </Column>
  </DataTable>
</template>
