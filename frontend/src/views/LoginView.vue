<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { useAuthStore } from '@/stores/auth'
import { getApiErrorMessage } from '@/services/http'

const auth = useAuthStore()
const router = useRouter()
const route = useRoute()
const toast = useToast()

const email = ref('')
const password = ref('')
const loading = ref(false)

async function submit() {
  loading.value = true
  try {
    await auth.login({ email: email.value, password: password.value })
    toast.add({ severity: 'success', summary: 'Welcome back', life: 2500 })
    const fallback = auth.isAdmin ? '/admin/products' : '/dashboard'
    router.push((route.query.redirect as string) || fallback)
  } catch (e) {
    toast.add({ severity: 'error', summary: 'Login failed', detail: getApiErrorMessage(e), life: 4000 })
  } finally {
    loading.value = false
  }
}

function fillDemo() {
  email.value = 'demo@arla-connect.test'
  password.value = 'Password123!'
}
</script>

<template>
  <Card class="auth-shell">
    <template #title>Sign in</template>
    <template #content>
      <form @submit.prevent="submit">
        <div class="auth-field">
          <label for="email">Email</label>
          <InputText id="email" v-model="email" type="email" required class="full-width" />
        </div>
        <div class="auth-field">
          <label for="password">Password</label>
          <Password
            input-id="password"
            v-model="password"
            :feedback="false"
            toggle-mask
            class="full-width"
            input-class="full-width"
          />
        </div>
        <Button type="submit" label="Sign in" class="full-width" :loading="loading" />
      </form>
      <Button label="Use demo account" link class="full-width" @click="fillDemo" />
      <p class="text-muted" style="text-align: center; margin-top: 0.5rem">
        No account?
        <RouterLink to="/register" style="color: var(--brand-dark)">Create one</RouterLink>
      </p>
    </template>
  </Card>
</template>
