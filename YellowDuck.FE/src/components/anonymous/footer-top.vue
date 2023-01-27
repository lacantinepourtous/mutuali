<template>
  <b-container fluid class="footer-top text-center text-light">
    <b-row>
      <b-col cols="12" sm="6" class="green footer-top__block footer-top__block--green d-flex justify-content-center py-5 px-3">
        <div class="section section--xl py-sm-5">
          <h2 class="mb-3 h3 text-light">{{ $t("text.landing-page-understand-mutuali") }}</h2>
          <p class="lead mb-0 font-weight-bold responsive-text">
            <a href="https://mutuali.notion.site/mutuali/Foire-aux-questions-0b482ef447694f73926b495675f1685e" target="_blank">
              {{ $t("btn.landing-page-faq") }}
            </a>
          </p>
        </div>
      </b-col>
      <b-col
        cols="12"
        sm="6"
        class="green-darker footer-top__block footer-top__block--green-medium d-flex justify-content-center py-5 px-3"
      >
        <div class="section section--xl py-sm-5">
          <h2 class="mb-3 h3 text-light">{{ $t("btn.landing-page-contact-us") }}</h2>
          <p class="lead mb-0 font-weight-bold responsive-text">
            <span v-html="$t('text.landing-page-contact-us')"></span>
          </p>
        </div>
      </b-col>
    </b-row>
  </b-container>
</template>

<style lang="scss" scoped>
.footer-top {
  &__block {
    &--green {
      background-color: $green;
    }

    &--green-medium {
      background-color: $green-medium;
    }
  }
}
</style>
