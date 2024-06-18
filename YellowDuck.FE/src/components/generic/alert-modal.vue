<template>
  <div class="modal-bottom section section--sm bg-warning p-4">
    <b-button variant="link" v-if="closable" class="modal-bottom__dismiss-btn h-extend-cursor-area" @click="$emit('close')">
      <span class="sr-only">{{ dismissLabel }}</span>
      <b-icon-x aria-hidden="true" size="sm"></b-icon-x>
    </b-button>
    <img v-if="icon" class="modal-bottom__image mb-2" alt="" :src="icon" />
    <slot></slot>
    <div v-if="$slots.footer" class="mb-2">
      <slot name="footer"></slot>
    </div>
  </div>
</template>
<script>
export default {
  props: {
    icon: String,
    closable: Boolean,
    dismissLabel: String
  }
};
</script>
<style lang="scss">
.modal-bottom {
  position: absolute;
  bottom: 0;
  z-index: 1;

  &__image {
    width: 30px;
  }

  &__dismiss-btn {
    height: 20px;
    width: 20px;
    color: $dark;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 0;
    font-size: 18px;
    position: absolute;
    right: 1rem;
    top: 1rem;
  }
}
</style>
