import http from './http'
import type { Category, PagedResult, Product } from '@/types'

export interface ProductsQuery {
  search?: string
  categoryId?: string
  page?: number
  pageSize?: number
}

export const productsService = {
  async getProducts(query: ProductsQuery = {}): Promise<PagedResult<Product>> {
    const { data } = await http.get<PagedResult<Product>>('/products', { params: query })
    return data
  },

  async getProduct(id: string): Promise<Product> {
    const { data } = await http.get<Product>(`/products/${id}`)
    return data
  },

  async getCategories(): Promise<Category[]> {
    const { data } = await http.get<Category[]>('/products/categories')
    return data
  },
}
