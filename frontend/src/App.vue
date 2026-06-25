<script setup lang="ts">
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import PublicShell from '@/layouts/PublicShell.vue'
import AppShell from '@/layouts/AppShell.vue'

const auth = useAuthStore()

// Logged-in users get the app-shell (sidebar, full width); guests get the
// public marketing layout (centered).
const layout = computed(() => (auth.isAuthenticated ? AppShell : PublicShell))
</script>

<template>
  <component :is="layout">
    <RouterView />
  </component>
  <Toast position="top-right" />
</template>
