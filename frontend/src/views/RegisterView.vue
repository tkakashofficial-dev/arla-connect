<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { useAuthStore } from '@/stores/auth'
import { getApiErrorMessage } from '@/services/http'

const auth = useAuthStore()
const router = useRouter()
const toast = useToast()

const fullName = ref('')
const companyName = ref('')
const email = ref('')
const password = ref('')
const loading = ref(false)

async function submit() {
  loading.value = true
  try {
    await auth.register({
      fullName: fullName.value,
      companyName: companyName.value,
      email: email.value,
      password: password.value,
    })
    toast.add({ severity: 'success', summary: 'Account created', life: 2500 })
    router.push('/dashboard')
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Registration failed', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <Card class="auth-shell">
    <template #title>Create a company account</template>
    <template #content>
      <form @submit.prevent="submit">
        <div class="auth-field">
          <label for="fullName">Your name</label>
          <InputText id="fullName" v-model="fullName" required class="full-width" />
        </div>
        <div class="auth-field">
          <label for="companyName">Company name</label>
          <InputText id="companyName" v-model="companyName" required class="full-width" />
        </div>
        <div class="auth-field">
          <label for="email">Email</label>
          <InputText id="email" v-model="email" type="email" required class="full-width" />
        </div>
        <div class="auth-field">
          <label for="password">Password</label>
          <Password
            input-id="password"
            v-model="password"
            toggle-mask
            class="full-width"
            input-class="full-width"
          />
        </div>
        <Button type="submit" label="Create account" class="full-width" :loading="loading" />
      </form>
      <p class="text-muted" style="text-align: center; margin-top: 0.5rem">
        Already have an account?
        <RouterLink to="/login" style="color: var(--brand-dark)">Sign in</RouterLink>
      </p>
    </template>
  </Card>
</template>
