<script setup lang="ts">
const props = defineProps<{ modelValue: number; min?: number; max?: number }>()
const emit = defineEmits<{ 'update:modelValue': [value: number] }>()

function dec() {
  emit('update:modelValue', Math.max(props.min ?? 1, props.modelValue - 1))
}
function inc() {
  const max = props.max ?? Number.MAX_SAFE_INTEGER
  emit('update:modelValue', Math.min(max, props.modelValue + 1))
}
</script>

<template>
  <div class="qty-stepper">
    <button type="button" class="qty-btn" :disabled="modelValue <= (min ?? 1)" aria-label="Decrease" @click="dec">
      <i class="pi pi-minus" />
    </button>
    <span class="qty-value">{{ modelValue }}</span>
    <button type="button" class="qty-btn" :disabled="max !== undefined && modelValue >= max" aria-label="Increase" @click="inc">
      <i class="pi pi-plus" />
    </button>
  </div>
</template>
