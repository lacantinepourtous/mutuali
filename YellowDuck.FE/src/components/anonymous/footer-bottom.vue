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
            ></b-img>
          </a>
          <a class="footer-bottom__link" href="https://www.rtcbq.com/" target="_blank">
            <b-img
              width="230"
              alt="Logo Regroupement des tables de concertation bioalimentaire du Québec (RTCBQ)"
              :src="require('@/assets/logos/logo-rtcbq.png')"
            ></b-img>
          </a>
          <a class="footer-bottom__link" href="https://alimentssante.ca/" target="_blank">
            <b-img
              width="258"
              alt="Logo Créneau d'excellence Aliments Santé"
              :src="require('@/assets/logos/logo-aliments-sante-horizontal.svg')"
              class="footer-bottom__logo-aliments-sante-mobile d-md-none"
            ></b-img>
            <b-img
              width="110"
              alt="Logo Créneau d'excellence Aliments Santé"
              :src="require('@/assets/logos/logo-aliments-sante.svg')"
              class="d-none d-md-inline-block"
            ></b-img>
          </a>
        </div>
      </div>

      

      <div class="footer-bottom__small">
          <span class="footer-bottom__item mb-2 mb-md-0 mr-md-2">
            &copy; {{ new Date().getFullYear() }}
            {{ $t("text.landing-page-all-right-reserved") }}
          </span>
          <span class="footer-bottom__item" v-html="$t('text.landing-page-terms-of-use')"></span>
          <span class="footer-bottom__item" v-html="$t('text.landing-page-privacy-policy')"></span>
          <span class="footer-bottom__item">
            <a href="#" @click.prevent="$termsFeed.openPreferencesCenter()">{{ $t("text.landing-page-terms-feed") }}</a>
          </span>
        </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.footer-bottom {
  padding: $spacer * 4 0 $spacer * 2;

  @include media-breakpoint-up(md) {
    padding: $spacer * 4 0 $spacer;
  }

  &__items {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: $spacer * 2;

    @include media-breakpoint-up(md) {
      flex-direction: row;
      align-items: flex-end;
      justify-content: center;
      gap: $spacer * 3;
    }
  }

  &__small {
    font-size: 13px;
    text-align: center;
    margin: $spacer * 2 0 $spacer / 2;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    row-gap: $spacer / 2;

    @include media-breakpoint-up(md) {
      flex-direction: row;
      align-items: center;
      column-gap: $spacer * 2;
    }
  }

  &__item {
    @include media-breakpoint-up(md) {
      white-space: nowrap;
    }
  }

  &__logos {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: $spacer;
    flex-wrap: wrap;

    @include media-breakpoint-up(md) {
      flex-direction: row;
      align-items: center;
      gap: $spacer * 4;
    }
  }

  &__link {
    display: inline-block;
  }

  &__logo-aliments-sante-mobile {
    width: 100%;
    max-width: 258px;
  }

}
</style>
