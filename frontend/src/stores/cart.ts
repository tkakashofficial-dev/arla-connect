import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import type { Product } from '@/types'

export interface CartItem {
  product: Product
  quantity: number
}

export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItem[]>([])

  const count = computed(() => items.value.reduce((sum, i) => sum + i.quantity, 0))
  const total = computed(() => items.value.reduce((sum, i) => sum + i.product.unitPrice * i.quantity, 0))
  const isEmpty = computed(() => items.value.length === 0)

  function add(product: Product, quantity = 1) {
    const existing = items.value.find((i) => i.product.id === product.id)
    if (existing) {
      existing.quantity = Math.min(existing.quantity + quantity, product.stockQuantity)
    } else {
      items.value.push({ product, quantity: Math.min(quantity, product.stockQuantity) })
    }
  }

  function updateQuantity(productId: string, quantity: number) {
    const item = items.value.find((i) => i.product.id === productId)
    if (!item) return
    if (quantity <= 0) {
      remove(productId)
    } else {
      item.quantity = quantity
    }
  }

  function remove(productId: string) {
    items.value = items.value.filter((i) => i.product.id !== productId)
  }

  function clear() {
    items.value = []
  }

  return { items, count, total, isEmpty, add, updateQuantity, remove, clear }
})
