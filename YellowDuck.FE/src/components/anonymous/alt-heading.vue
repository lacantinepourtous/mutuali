<template>
  <div class="anonymous-alt-heading">
    <div class="anonymous-alt-heading__img-container">
      <img class="anonymous-alt-heading__img d-md-none" :alt="imgAlt" :src="imgSrcMobile" />
      <img class="anonymous-alt-heading__img d-none d-md-block" :alt="imgAlt" :src="imgSrc" />
    </div>

    <div class="anonymous-alt-heading__content">
      <span v-if="subtitle" class="d-inline-block mb-2 text-uppercase font-weight-bold letter-spacing-wide">{{ subtitle }}</span>
      <h1 v-if="title" class="mb-4">{{ title }}</h1>
      <slot>
        <p v-if="description">
          {{ description }}
        </p>
      </slot>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    imgSrc: {
      type: String,
      default: ""
    },
    imgSrcMobile: {
      type: String,
      default: ""
    },
    imgAlt: {
      type: String,
      default: ""
    },
    title: {
      type: String,
      default: ""
    },
    subtitle: {
      type: String,
      default: ""
    },
    description: {
      type: String,
      default: ""
    }
  }
};
</script>


<style lang="scss">
$img-width-mobile: 200px;
$img-height-mobile: 240px;
$img-width: 400px;
$img-height: 470px;

.anonymous-alt-heading {
  & {
    display: flex;
    flex-direction: column;
    margin-bottom: $spacer * 2;

    @include media-breakpoint-up(sm) {
      flex-direction: row;
      justify-content: space-between;
      height: 100%;
    }

    @include media-breakpoint-up(md) {
      margin-bottom: $spacer * -4;
    }
  }

  &__img-container {
    display: flex;
    flex-grow: 1;
    width: $img-width-mobile;
    height: $img-height-mobile;
    position: relative;
    z-index: 2;
    border-radius: 20px;
    overflow: hidden;
    flex-shrink: 0;

    @include media-breakpoint-up(md) {
      width: $img-width;
      height: auto;
      min-height: $img-height;
    }
  }

  &__img {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    object-fit: cover;
    width: 100%;
    height: 100%;
  }

  &__content {
    padding: $spacer * 2 0 0;

    & > :first-child {
      margin-top: 0;
    }

    & > :last-child {
      margin-bottom: 0;
    }

    @include media-breakpoint-up(sm) {
      padding: $spacer * 2 0 0 $spacer * 2;
    }

    @include media-breakpoint-up(md) {
      padding: $spacer * 4 0 $spacer * 8 $spacer * 4;
    }
  }
}
</style>
