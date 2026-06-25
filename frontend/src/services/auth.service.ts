import http from './http'
import type { AuthResponse } from '@/types'

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  password: string
  fullName: string
  companyName: string
}

export const authService = {
  async login(request: LoginRequest): Promise<AuthResponse> {
    const { data } = await http.post<AuthResponse>('/auth/login', request)
    return data
  },

  async register(request: RegisterRequest): Promise<AuthResponse> {
    const { data } = await http.post<AuthResponse>('/auth/register', request)
    return data
  },
}
