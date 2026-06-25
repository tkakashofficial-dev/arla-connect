import http from './http'
import type { Order, PagedResult } from '@/types'

export interface CreateOrderLine {
  productId: string
  quantity: number
}

export interface CreateOrderRequest {
  lines: CreateOrderLine[]
}

export interface OrdersQuery {
  page?: number
  pageSize?: number
}

export const ordersService = {
  async createOrder(request: CreateOrderRequest): Promise<Order> {
    const { data } = await http.post<Order>('/orders', request)
    return data
  },

  async getMyOrders(query: OrdersQuery = {}): Promise<PagedResult<Order>> {
    const { data } = await http.get<PagedResult<Order>>('/orders', { params: query })
    return data
  },

  async getOrder(id: string): Promise<Order> {
    const { data } = await http.get<Order>(`/orders/${id}`)
    return data
  },
}
