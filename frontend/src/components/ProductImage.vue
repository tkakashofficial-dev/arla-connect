<script setup lang="ts">
import { ref, watch } from 'vue'
import { productVisual } from '@/utils/productImage'

const props = defineProps<{ sku: string; imageUrl?: string | null }>()

// If the image fails to load, fall back to the branded category tile.
const failed = ref(false)
watch(
  () => props.imageUrl,
  () => {
    failed.value = false
  },
)
</script>

<template>
  <img
    v-if="imageUrl && !failed"
    :src="imageUrl"
    :alt="sku"
    class="product-img"
    loading="lazy"
    @error="failed = true"
  />
  <span v-else class="product-img-fallback" :style="{ background: productVisual(sku).background }">
    {{ productVisual(sku).emoji }}
  </span>
</template>
