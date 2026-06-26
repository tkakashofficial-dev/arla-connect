import http from './http'
import type { PagedResult, Product } from '@/types'
import type { ProductsQuery } from './products.service'

export interface CreateProductPayload {
  sku: string
  name: string
  description?: string | null
  unitPrice: number
  currency: string
  stockQuantity: number
  imageUrl?: string | null
  categoryId: string
}

export interface UpdateProductPayload {
  name: string
  description?: string | null
  unitPrice: number
  currency: string
  stockQuantity: number
  imageUrl?: string | null
  categoryId: string
  isActive: boolean
}

export const adminProductsService = {
  async getAll(query: ProductsQuery = {}): Promise<PagedResult<Product>> {
    const { data } = await http.get<PagedResult<Product>>('/admin/products', { params: query })
    return data
  },

  async create(payload: CreateProductPayload): Promise<Product> {
    const { data } = await http.post<Product>('/admin/products', payload)
    return data
  },

  async update(id: string, payload: UpdateProductPayload): Promise<Product> {
    const { data } = await http.put<Product>(`/admin/products/${id}`, payload)
    return data
  },

  async remove(id: string): Promise<void> {
    await http.delete(`/admin/products/${id}`)
  },

  async uploadImage(file: File): Promise<string> {
    const form = new FormData()
    form.append('file', file)
    const { data } = await http.post<{ url: string }>('/admin/products/image', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    return data.url
  },
}
