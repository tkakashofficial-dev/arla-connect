import http from './http'
import type { Claim, ClaimReason, PagedResult } from '@/types'

export interface CreateClaimRequest {
  orderId: string
  reason: ClaimReason
  description: string
}

export interface ClaimsQuery {
  page?: number
  pageSize?: number
}

export const claimsService = {
  async createClaim(request: CreateClaimRequest): Promise<Claim> {
    const { data } = await http.post<Claim>('/claims', request)
    return data
  },

  async getMyClaims(query: ClaimsQuery = {}): Promise<PagedResult<Claim>> {
    const { data } = await http.get<PagedResult<Claim>>('/claims', { params: query })
    return data
  },

  async getClaim(id: string): Promise<Claim> {
    const { data } = await http.get<Claim>(`/claims/${id}`)
    return data
  },
}
