<template>
  <div class="logos">
    <slot name="title">
      <h2 v-if="title" class="h3">{{ title }}</h2>
    </slot>
    <div class="logos__items">
      <div class="logos__item" v-for="(logo, index) in logos" :key="index">
        <div class="logos__img-container">
          <img class="logos__img" :src="logo.src" :alt="logo.alt" />
        </div>
      </div>
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
    logos: {
      type: Array,
      default() {
        return [];
      }
    }
  }
};
</script>

<style lang="scss" scoped>
$logo-width: 240px;
$logo-height: 110px;
.logos {
  &__items {
    display: flex;
    flex-wrap: wrap;
    align-items: center;
    flex-direction: col;

    @include media-breakpoint-up(md) {
      flex-direction: row;
      justify-content: space-between;
      margin: 0 $spacer * -1 $spacer * -1;
    }
  }

  &__item {
    margin: 0 0 $spacer * 2;
    width: 100%;
    height: $logo-height;

    @include media-breakpoint-up(md) {
      margin: 0 $spacer $spacer;
      width: calc(33.33% - #{$spacer} * 2);
    }
  }

  &__img-container {
    position: relative;
    max-width: 100%;
    width: $logo-width;
    height: $logo-height;
    margin: 0 auto;
  }

  &__img {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    height: 100%;
    width: 100%;
    object-fit: contain;
  }
}
</style>
