<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import { adminStaffService } from '@/services/admin.staff.service'
import { getApiErrorMessage } from '@/services/http'
import { formatDate } from '@/utils/format'
import type { AdminStaff } from '@/types'

const toast = useToast()
const staff = ref<AdminStaff[]>([])
const loading = ref(false)

const dialogVisible = ref(false)
const saving = ref(false)
const form = ref({ fullName: '', email: '', password: '' })

async function load() {
  loading.value = true
  try {
    staff.value = await adminStaffService.getAll()
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Could not load staff', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    loading.value = false
  }
}

function openCreate() {
  form.value = { fullName: '', email: '', password: '' }
  dialogVisible.value = true
}

async function save() {
  saving.value = true
  try {
    await adminStaffService.create({ ...form.value })
    toast.add({ severity: 'success', summary: 'Staff member added', life: 2500 })
    dialogVisible.value = false
    await load()
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Could not add staff', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    saving.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="dash-head">
    <p class="text-muted" style="margin: 0">{{ staff.length }} Arla staff member{{ staff.length === 1 ? '' : 's' }}</p>
    <Button label="Add staff" icon="pi pi-user-plus" @click="openCreate" />
  </div>

  <div v-if="loading" class="state"><ProgressSpinner style="width: 42px; height: 42px" /></div>

  <DataTable v-else :value="staff" data-key="id" striped-rows>
    <Column field="fullName" header="Name" />
    <Column field="email" header="Email" />
    <Column header="Role"><template #body="{ data }"><Tag :value="data.role" severity="info" /></template></Column>
    <Column header="Added"><template #body="{ data }">{{ formatDate(data.createdAtUtc) }}</template></Column>
  </DataTable>

  <Dialog v-model:visible="dialogVisible" modal header="Add staff member" :style="{ width: '460px' }">
    <div class="auth-field">
      <label>Full name</label>
      <InputText v-model="form.fullName" class="full-width" />
    </div>
    <div class="auth-field">
      <label>Email</label>
      <InputText v-model="form.email" type="email" class="full-width" />
    </div>
    <div class="auth-field">
      <label>Temporary password</label>
      <Password v-model="form.password" toggle-mask :feedback="false" class="full-width" input-class="full-width" />
    </div>
    <template #footer>
      <Button label="Cancel" text @click="dialogVisible = false" />
      <Button
        label="Add staff"
        :loading="saving"
        :disabled="!form.fullName || !form.email || form.password.length < 8"
        @click="save"
      />
    </template>
  </Dialog>
</template>
