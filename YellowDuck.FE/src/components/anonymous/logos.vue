<template>
  <div class="logos">
    <slot name="title">
      <h2 v-if="title" class="h3">{{ title }}</h2>
    </slot>
    <ul class="logos__items">
      <li class="logos__item" v-for="(logo, index) in logos" :key="index">
        <div class="logos__img-container">
          <img class="logos__img" :src="logo.src" :alt="logo.alt" />
        </div>
      </li>
    </ul>
    <p v-if="description" class="logos__desc text-light">{{ description }}</p>
  </div>
</template>

<script>
export default {
  props: {
    title: {
      type: String,
      default: ""
    },
    description: {
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
$logo-height-mobile: 80px;
$logo-height: 110px;

.logos {
  &__items {
    display: grid;
    grid-template-columns: 1fr 1fr;
    column-gap: $spacer * 3;
    row-gap: $spacer * 0.5;
    padding-left: 0;
    margin: 0 $spacer;

    @include media-breakpoint-up(md) {
      display: flex;
      column-gap: $spacer * 4;
    }

    @include media-breakpoint-up(lg) {
      column-gap: $spacer * 6;
    }
  }

  &__item {
    height: $logo-height;
  }

  &__img-container {
    position: relative;
    height: $logo-height-mobile;
    margin: 0 auto;

    @include media-breakpoint-up(md) {
      height: $logo-height;
    }
  }

  &__img {
    max-width: 100%;
    height: auto;
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    width: 100%;
    height: 100%;
    object-fit: contain;
    object-position: center;

    @include media-breakpoint-up(md) {
      position: static;
    }
  }

  &__desc {
    max-width: 540px;
    margin: $spacer * 2.5 auto 0;
    text-align: center;
    font-size: 14px;
  }
}
</style>
