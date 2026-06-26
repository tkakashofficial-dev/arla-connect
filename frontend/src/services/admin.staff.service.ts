import http from './http'
import type { AdminStaff } from '@/types'

export interface CreateStaffPayload {
  fullName: string
  email: string
  password: string
}

export const adminStaffService = {
  async getAll(): Promise<AdminStaff[]> {
    const { data } = await http.get<AdminStaff[]>('/admin/staff')
    return data
  },

  async create(payload: CreateStaffPayload): Promise<AdminStaff> {
    const { data } = await http.post<AdminStaff>('/admin/staff', payload)
    return data
  },
}
