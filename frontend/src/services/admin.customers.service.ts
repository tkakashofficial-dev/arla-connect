import http from './http'
import type { AdminCustomerDetail, AdminCustomerListItem, PagedResult } from '@/types'

export const adminCustomersService = {
  async getAll(query: { search?: string; page?: number; pageSize?: number } = {}): Promise<PagedResult<AdminCustomerListItem>> {
    const { data } = await http.get<PagedResult<AdminCustomerListItem>>('/admin/customers', { params: query })
    return data
  },

  async getById(id: string): Promise<AdminCustomerDetail> {
    const { data } = await http.get<AdminCustomerDetail>(`/admin/customers/${id}`)
    return data
  },
}
