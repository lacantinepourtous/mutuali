<template>
  <b-badge class="custom-badge text-white py-1" variant="info" pill>
    <span class="custom-badge__label">{{ getCategoryLabel(category) }}</span>
    <b-button v-if="closable" class="custom-badge__dismiss-btn h-extend-cursor-area" @click="$emit('close')">
      <span class="sr-only">{{ dismissLabel }}</span>
      <svg
        class="custom-badge__dismiss-btn-x"
        aria-hidden="true"
        xmlns="http://www.w3.org/2000/svg"
        width="32"
        height="32"
        fill="currentColor"
        viewBox="0 0 256 256"
      >
        <path
          d="M205.66,194.34a8,8,0,0,1-11.32,11.32L128,139.31,61.66,205.66a8,8,0,0,1-11.32-11.32L116.69,128,50.34,61.66A8,8,0,0,1,61.66,50.34L128,116.69l66.34-66.35a8,8,0,0,1,11.32,11.32L139.31,128Z"
        ></path>
      </svg>
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
    border: solid 0px transparent;
    height: 14px;
    width: 14px;
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
      box-shadow: none;
      transform: translate(0, 0);
    }

    &-x {
      height: 12px;
      width: 12px;
    }
  }
}
</style>
