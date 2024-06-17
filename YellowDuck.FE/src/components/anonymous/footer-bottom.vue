<template>
  <div class="footer-bottom">
    <div class="section section--xl">
      <div class="footer-bottom__items">
        <div class="footer-bottom__logos">
          <a class="footer-bottom__link footer-bottom__link--cibim" href="https://cibim.org/" target="_blank">
            <b-img
              width="258"
              alt="Logo Carrefour de l'Industrie Bioalimentaire de l'Île de Montréal (CIBÎM)"
              :src="require('@/assets/logos/logo-cibim.svg')"
              class="mr-3"
            ></b-img>
          </a>
          <a class="footer-bottom__link" href="https://www.rtcbq.com/" target="_blank">
            <b-img
              width="230"
              alt="Logo Regroupement des tables de concertation bioalimentaire du Québec (RTCBQ)"
              :src="require('@/assets/logos/logo-rtcbq.png')"
            ></b-img>
          </a>
        </div>

        <div class="footer-bottom__small">
          <span class="footer-bottom__item">
            &copy; {{ new Date().getFullYear() }}
            {{ $t("text.landing-page-all-right-reserved") }}
          </span>
          <span class="footer-bottom__item" v-html="$t('text.landing-page-terms-of-use')"></span>
          <span class="footer-bottom__item" v-html="$t('text.landing-page-privacy-policy')"></span>
        </div>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.footer-bottom {
  padding: $spacer * 4 0 $spacer * 2;

  @include media-breakpoint-up(md) {
    padding: $spacer * 6 0 $spacer;
  }

  &__items {
    @include media-breakpoint-up(lg) {
      display: flex;
      align-items: flex-end;
      justify-content: space-between;
    }
  }

  &__small {
    font-size: 13px;
    margin-top: $spacer;
    flex-grow: 1;
    text-align: center;

    @include media-breakpoint-up(lg) {
      margin-bottom: $spacer;
      text-align: right;
    }
  }

  &__item {
    white-space: nowrap;

    &:not(:last-child) {
      &:after {
        content: "–";
        display: inline-block;
        margin: 0 $spacer * 0.5;
      }
    }
  }

  &__logos {
    text-align: center;
  }

  &__link {
    display: inline-block;
  }
}
</style>
