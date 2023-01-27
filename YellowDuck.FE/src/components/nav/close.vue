<template>
  <nav-base>
    <b-button class="nav-close h3" size="lg" :aria-label="label || $t('sr.close-panel')" :to="to" @click="$emit('close')">
      <b-icon-x aria-hidden="true"></b-icon-x>
    </b-button>
  </nav-base>
</template>

<script>
import NavBase from "@/components/nav/base";

export default {
  components: { NavBase },
  props: {
    label: {
      type: String,
      default: "",
      required: false
    },
    to: {
      type: [Object, null],
      default: null,
      required: false
    }
  }
};
</script>

<style lang="scss">
.nav-close {
  $navbar-height: $navbar-padding-y * 2 + $navbar-brand-padding-y * 2 + $navbar-brand-height;

  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0;
  height: $navbar-height;
  width: $navbar-height;
  position: absolute;
  top: 0;
  right: 0;
  padding: 0;
  transition: background-color 0.2s ease-in-out, color 0.2s ease-in-out;
  background-color: $green-darker;
  color: $gray-100;
  border: 0;

  &:hover {
    background-color: $gray-400;
    color: $green-darker;
    box-shadow: none;
    transform: translate(0, 0);
  }
}
</style>
