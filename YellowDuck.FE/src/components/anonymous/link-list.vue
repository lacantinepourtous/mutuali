<template>
  <div class="link-list">
    <slot name="title">
      <h3 class="link-list__title" v-if="title">{{ title }}</h3>
    </slot>

    <div v-for="(link, index) in links" :key="index">
      <LinkBox :link="link" />
    </div>
  </div>
</template>

<script>
import LinkBox from "./link-box";

export default {
  components: {
    LinkBox
  },
  props: {
    title: {
      type: String,
      default: ""
    },
    links: {
      type: Array,
      default: null,
      required: true
    }
  }
};
</script>

<style lang="scss" scoped>
.link-list {
  &__title {
  }
}
</style>
