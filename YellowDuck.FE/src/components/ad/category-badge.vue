<template>
  <b-badge class="custom-badge text-white py-1" variant="info" pill>
    <span class="custom-badge__label">{{ getCategoryLabel(category) }}</span>
    <b-button v-if="closable" class="custom-badge__dismiss-btn h-extend-cursor-area" @click="$emit('close')">
      <span class="sr-only">{{ dismissLabel }}</span>
      <b-icon-x aria-hidden="true" size="sm"></b-icon-x>
    </b-button>
  </b-badge>
</template>

<script>
import { AdCategory } from "@/mixins/ad-category";

export default {
  mixins: [AdCategory],
  props: {
    category: String,
    closable: Boolean,
    dismissLabel: String
  }
};
</script>

<style lang="scss">
.custom-badge {
  display: inline-flex;
  align-items: center;
  justify-content: space-between;
  max-width: 100%;

  &__label {
    display: block;
    text-overflow: ellipsis;
    overflow: hidden;
    white-space: nowrap;
    max-width: 100%;
  }

  &__dismiss-btn {
    background-color: $white;
    border-radius: 50%;
    height: 13px;
    width: 13px;
    color: $gray-900;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 0;
    margin-left: 4px;
    font-size: 11px;
    position: relative;
    left: 4px;

    &:hover,
    &:focus {
      background-color: $green;
      color: $white;
      transform: translate(0, 0);
    }
  }
}
</style>
