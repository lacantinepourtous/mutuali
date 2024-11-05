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
$logo-width: 240px;
$logo-height: 110px;
.logos {
  &__items {
    display: grid;
    grid-template-columns: 1fr 1fr;
    grid-template-rows: auto;
    column-gap: $spacer * 2;
    row-gap: $spacer * 0.5;
    padding-left: 0;

    @include media-breakpoint-up(md) {
      grid-template-columns: 1fr 1fr 1fr 1fr;
    }
  }

  &__item {
    height: $logo-height;
  }

  &__img-container {
    position: relative;
    max-width: $logo-width;
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

  &__desc {
    max-width: 540px;
    margin: $spacer * 2.5 auto 0;
    text-align: center;
    font-size: 14px;
  }
}
</style>
