import axios, { AxiosError } from 'axios'
import { useAuthStore } from '@/stores/auth'
import router from '@/router'

const http = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
})

// Attach the JWT to every request when logged in.
http.interceptors.request.use((config) => {
  const auth = useAuthStore()
  if (auth.token) {
    config.headers.Authorization = `Bearer ${auth.token}`
  }
  return config
})

// On 401 the token is missing/expired -> sign out and send to login.
http.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      const auth = useAuthStore()
      if (auth.isAuthenticated) {
        auth.logout()
        router.push({ name: 'login' })
      }
    }
    return Promise.reject(error)
  },
)

/** Pull a human-readable message out of an API ProblemDetails error. */
export function getApiErrorMessage(error: unknown, fallback = 'Something went wrong.'): string {
  if (error instanceof AxiosError && error.response?.data) {
    const data = error.response.data as {
      title?: string
      detail?: string
      errors?: Record<string, string[]>
    }
    if (data.errors) {
      const first = Object.values(data.errors)[0]
      if (first?.length) return first[0]
    }
    return data.detail ?? data.title ?? fallback
  }
  return fallback
}

export default http
