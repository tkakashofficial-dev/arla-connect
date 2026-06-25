import http from './http'
import type { AdminOrder, OrderStatus, PagedResult } from '@/types'

export const adminOrdersService = {
  async getAll(query: { page?: number; pageSize?: number } = {}): Promise<PagedResult<AdminOrder>> {
    const { data } = await http.get<PagedResult<AdminOrder>>('/admin/orders', { params: query })
    return data
  },

  async updateStatus(id: string, status: OrderStatus): Promise<void> {
    await http.put(`/admin/orders/${id}/status`, { status })
  },
}
