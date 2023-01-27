<template>
  <div class="footer-bottom gray-lighter bg-light text-center">
    <div class="section section--xl">
      <p class="h3 mb-3">{{ $t("text.landing-page-project-by") }}</p>
      <a class="footer-bottom__link link-scale" href="https://lacantinepourtous.org/" target="_blank">
        <b-img width="192" alt="Logo Cantine pour tous" :src="require('@/assets/logos/logo-cantine.png')" class="mb-3"></b-img>
      </a>
      <span class="footer-bottom__items">
        <small class="footer-bottom__item">{{ $t("text.landing-page-all-right-reserved") }}</small>
        <small class="footer-bottom__item" v-html="$t('text.landing-page-terms-of-use')"></small>
        <small class="footer-bottom__item" v-html="$t('text.landing-page-privacy-policy')"></small>
      </span>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.footer-bottom {
  padding: $spacer * 6 0 $spacer * 4;

  @include media-breakpoint-up(md) {
    padding: $spacer * 12 0 $spacer * 8;
  }

  &__items {
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;

    @include media-breakpoint-up(sm) {
      flex-direction: row;
    }
  }

  &__item:not(:last-child) {
    margin-bottom: 8px;

    @include media-breakpoint-up(sm) {
      margin-bottom: 0;

      &:after {
        content: "–";
        display: inline-block;
        margin: 0 $spacer * 0.5;
      }
    }
  }

  &__link {
    display: inline-block;
  }
}
</style>
