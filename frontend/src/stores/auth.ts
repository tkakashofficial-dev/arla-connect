import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { authService, type LoginRequest, type RegisterRequest } from '@/services/auth.service'
import type { AuthResponse } from '@/types'

const STORAGE_KEY = 'arla-connect-auth'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<AuthResponse | null>(loadFromStorage())

  const token = computed(() => user.value?.token ?? null)
  const isAuthenticated = computed(() => {
    if (!user.value) return false
    return new Date(user.value.expiresAtUtc) > new Date()
  })
  const isAdmin = computed(() => user.value?.role === 'PlatformAdmin')

  async function login(request: LoginRequest) {
    setUser(await authService.login(request))
  }

  async function register(request: RegisterRequest) {
    setUser(await authService.register(request))
  }

  function logout() {
    user.value = null
    localStorage.removeItem(STORAGE_KEY)
  }

  function setUser(value: AuthResponse) {
    user.value = value
    localStorage.setItem(STORAGE_KEY, JSON.stringify(value))
  }

  return { user, token, isAuthenticated, isAdmin, login, register, logout }
})

function loadFromStorage(): AuthResponse | null {
  const raw = localStorage.getItem(STORAGE_KEY)
  if (!raw) return null
  try {
    return JSON.parse(raw) as AuthResponse
  } catch {
    return null
  }
}
