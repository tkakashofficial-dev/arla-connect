import type { ClaimStatus, OrderStatus } from '@/types'

export function formatCurrency(value: number, currency = 'DKK'): string {
  return new Intl.NumberFormat('da-DK', { style: 'currency', currency }).format(value)
}

export function formatDate(iso: string): string {
  return new Date(iso).toLocaleString('da-DK', { dateStyle: 'medium', timeStyle: 'short' })
}

type Severity = 'success' | 'info' | 'warn' | 'danger' | 'secondary'

export function orderStatusSeverity(status: OrderStatus): Severity {
  switch (status) {
    case 'Delivered':
    case 'Shipped':
      return 'success'
    case 'Confirmed':
      return 'info'
    case 'Cancelled':
      return 'danger'
    default:
      return 'secondary'
  }
}

export function claimStatusSeverity(status: ClaimStatus): Severity {
  switch (status) {
    case 'Approved':
    case 'Resolved':
      return 'success'
    case 'UnderReview':
      return 'info'
    case 'Rejected':
      return 'danger'
    default:
      return 'warn'
  }
}
