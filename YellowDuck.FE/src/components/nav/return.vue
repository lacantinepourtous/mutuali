<template>
  <b-navbar class="nav-return">
    <b-navbar-brand class="text-dark nav-return__back-link" :to="to">
      <b-icon-chevron-left aria-hidden="true" class="nav-return__back-link-arrow"></b-icon-chevron-left>
      <span class="small font-weight-bolder align-text-bottom">{{ label || $t("btn.cancel") }}</span>
    </b-navbar-brand>
    <div v-if="$slots.default" class="ml-auto">
      <slot></slot>
    </div>
  </b-navbar>
</template>

<script>
export default {
  props: {
    to: {
      default: null,
      required: false
    },
    label: {
      default: "",
      required: false
    }
  }
};
</script>

<style lang="scss">
.nav-return {
  background-color: $white;
  box-shadow: 0 -1px 0 0 $gray-200 inset;
  height: $nav-return-height;

  &__back-link {
    &:hover {
      .nav-return__back-link-arrow {
        animation: bounce-left 1s ease-in-out;
        animation-iteration-count: 3;

        @media (prefers-reduced-motion: reduce) {
          animation: none;
        }
      }
    }
  }
}
</style>
