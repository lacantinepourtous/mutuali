<template>
  <div class="anonymous-ambiance-img section section--md">
    <div class="anonymous-ambiance-img__container">
      <img
        class="anonymous-ambiance-img__img anonymous-ambiance-img__img--mobile"
        :alt="$t('text.landingpage.video.alt')"
        :src="require('@/assets/ambiance/woman-in-kitchen-810x456.jpg')"
      />
      <img
        class="anonymous-ambiance-img__img anonymous-ambiance-img__img--desktop"
        :alt="$t('text.landingpage.video.alt')"
        :src="require('@/assets/ambiance/woman-in-kitchen.jpg')"
      />
    </div>
  </div>
</template>

<style lang="scss" scoped>
$video-overlap: 100px;
.anonymous-ambiance-img {
  & {
    margin-top: $video-overlap;
  }

  &__container {
    position: relative;
    height: 0;
    padding-bottom: 56.25%;
    border-radius: 20px;
    overflow: hidden;
    top: $video-overlap * -1;
    margin-bottom: $video-overlap * -1;
  }

  &__img {
    position: absolute;
    top: 0;
    left: 0;
    bottom: 0;
    right: 0;
    width: 100%;
    height: 100%;
    object-fit: cover;

    &--mobile {
      @include media-breakpoint-up(sm) {
        display: none;
      }
    }

    &--desktop {
      display: none;
      @include media-breakpoint-up(sm) {
        display: block;
      }
    }
  }
}
</style>
