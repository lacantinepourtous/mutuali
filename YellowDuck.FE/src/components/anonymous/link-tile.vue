<template>
  <div class="link-tile" :class="{ 'link-tile--reverse': reverse }">
    <img class="link-tile__img" :class="reverse ? 'link-tile__img--bottom' : 'link-tile__img--top'" :src="imgSrc" alt="" />
    <div class="link-tile__content">
      <h2 class="mb-3">
        <RouterLink class="link-title" :to="link">
          {{ title }}
        </RouterLink>
      </h2>
      <p class="mb-0">{{ text }}</p>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    title: {
      type: String,
      default: ""
    },
    link: {
      type: [String, Object],
      default: "#"
    },
    text: {
      type: String,
      default: ""
    },
    imgSrc: {
      type: String,
      default: ""
    },
    reverse: Boolean
  }
};
</script>

<style lang="scss" scoped>
$tile-width: 400px;
.link-tile {
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;

  @include media-breakpoint-up(sm) {
    width: $tile-width;
  }

  &--reverse {
    @include media-breakpoint-up(md) {
      flex-direction: column-reverse;
      margin-top: 60px;
    }
  }

  &__img {
    margin-bottom: $spacer;
    max-width: 50%;

    @include media-breakpoint-up(sm) {
      max-width: 100%;
    }

    @include media-breakpoint-up(md) {
      margin-bottom: $spacer * 2;
    }

    &--bottom {
      @include media-breakpoint-up(md) {
        margin-top: $spacer * 2;
        margin-bottom: 0;
      }
    }
  }
}
</style>
