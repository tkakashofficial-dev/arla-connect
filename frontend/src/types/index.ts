// Mirrors the API DTOs. Enums arrive as strings (JsonStringEnumConverter).

export type OrderStatus = 'Pending' | 'Confirmed' | 'Shipped' | 'Delivered' | 'Cancelled'
export type ClaimStatus = 'Open' | 'UnderReview' | 'Approved' | 'Rejected' | 'Resolved'
export type ClaimReason = 'DamagedGoods' | 'WrongItem' | 'MissingItem' | 'QualityIssue' | 'Other'

export interface AuthResponse {
  token: string
  expiresAtUtc: string
  email: string
  fullName: string
  role: string
  companyName: string
}

export interface Category {
  id: string
  name: string
  slug: string
}

export interface Product {
  id: string
  sku: string
  name: string
  description?: string
  unitPrice: number
  currency: string
  stockQuantity: number
  imageUrl?: string
  categoryId: string
  categoryName: string
  isActive: boolean
}

export interface OrderLine {
  productId: string
  sku: string
  productName: string
  quantity: number
  unitPrice: number
  lineTotal: number
}

export interface Order {
  id: string
  orderNumber: string
  status: OrderStatus
  currency: string
  totalAmount: number
  createdAtUtc: string
  lines: OrderLine[]
}

export interface Claim {
  id: string
  claimNumber: string
  orderId: string
  orderNumber: string
  reason: ClaimReason
  status: ClaimStatus
  description: string
  createdAtUtc: string
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

// --- Admin (back-office) ---
export interface AdminOrder {
  id: string
  orderNumber: string
  customerName: string
  status: OrderStatus
  currency: string
  totalAmount: number
  createdAtUtc: string
}

export interface AdminClaim {
  id: string
  claimNumber: string
  customerName: string
  orderNumber: string
  reason: ClaimReason
  status: ClaimStatus
  description: string
  createdAtUtc: string
}

export interface StatusCount {
  status: string
  count: number
}

export interface AdminSummary {
  totalOrders: number
  totalRevenue: number
  openClaims: number
  activeProducts: number
  lowStockProducts: number
  totalCustomers: number
  ordersByStatus: StatusCount[]
}
