import http from './http'
import type { AdminSummary } from '@/types'

export const adminDashboardService = {
  async getSummary(): Promise<AdminSummary> {
    const { data } = await http.get<AdminSummary>('/admin/dashboard')
    return data
  },
}
