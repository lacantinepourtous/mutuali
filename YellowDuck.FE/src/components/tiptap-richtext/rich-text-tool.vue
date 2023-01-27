<template>
  <b-button
    v-b-tooltip.hover
    :title="label"
    :aria-label="label"
    :pressed="isActive"
    type="button"
    variant="outline-primary"
    @click="onClick"
  >
    <b-icon v-if="icon" :icon="icon" aria-hidden="true"></b-icon>
    <span v-if="$slots.default"><slot /></span>
  </b-button>
</template>
<script>
export default {
  props: {
    isActive: Boolean,
    icon: String,
    label: String
  },
  methods: {
    onClick() {
      this.$emit("click");
    }
  }
};
</script>
