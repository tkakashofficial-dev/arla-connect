import http from './http'
import type { AdminClaim, ClaimStatus, PagedResult } from '@/types'

export const adminClaimsService = {
  async getAll(query: { page?: number; pageSize?: number } = {}): Promise<PagedResult<AdminClaim>> {
    const { data } = await http.get<PagedResult<AdminClaim>>('/admin/claims', { params: query })
    return data
  },

  async updateStatus(id: string, status: ClaimStatus): Promise<void> {
    await http.put(`/admin/claims/${id}/status`, { status })
  },
}
