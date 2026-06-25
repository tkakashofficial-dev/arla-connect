// Visual tile per product category (derived from the SKU prefix).
// Clean, brand-consistent, and works offline — no external image dependency.

interface Visual {
  emoji: string
  background: string
}

const byPrefix: Record<string, Visual> = {
  MILK: { emoji: '🥛', background: 'linear-gradient(135deg, #e8f1fb, #cfe2f7)' },
  CHE: { emoji: '🧀', background: 'linear-gradient(135deg, #fdf3e0, #f6e1bd)' },
  BUT: { emoji: '🧈', background: 'linear-gradient(135deg, #fffbe6, #f3ead0)' },
  YOG: { emoji: '🍶', background: 'linear-gradient(135deg, #fdeef2, #f6d6e0)' },
}

const fallback: Visual = { emoji: '📦', background: 'linear-gradient(135deg, #eef2f7, #dde5ef)' }

export function productVisual(sku: string): Visual {
  const prefix = sku.split('-')[0]
  return byPrefix[prefix] ?? fallback
}
