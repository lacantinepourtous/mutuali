<template>
  <router-link
    v-if="ad"
    class="mutuali-ad-card card"
    :class="{ 'mutuali-ad-card--admin': adIsAdminOnly }"
    :to="{ name: $consts.urls.URL_AD_DETAIL, params: { id: ad.id } }"
  >
    <div class="mutuali-ad-card__badge">
      <ad-category-badge :category="adCategory" is-short />
    </div>

    <div class="mutuali-ad-card__pic">
      <img
        v-if="adGallery && adGallery[0]"
        class="mutuali-ad-card__img"
        :src="`${adGallery[0].src}?mode=crop&width=150&height=150`"
        :alt="adGallery[0].alt ? adGallery[0].alt : ''"
      />
      <div v-if="adIsAdminOnly" class="mutuali-ad-card__overlay-admin"></div>
      <b-img
        v-if="adIsAdminOnly"
        class="mutuali-ad-card__icon-invisible"
        :src="require('@/assets/icons/invisible.svg')"
        alt=""
        height="30"
        block
      ></b-img>
    </div>
    <div class="mutuali-ad-card__text">
      <p class="mutuali-ad-card__organization mb-2 letter-spacing-wide small">
        <span class="font-weight-bolder text-uppercase">{{ adOrganization }}</span>
        <template v-if="typeof distance === 'number'">
          <svg
            class="mutuali-ad-card__circle green mx-2"
            viewBox="0 0 4 4"
            height="4px"
            width="4px"
            xmlns="http://www.w3.org/2000/svg"
          >
            <circle cx="2" cy="2" r="2" fill="currentColor" />
          </svg>
          <span>{{ adDistance }}km</span>
        </template>
      </p>
      <p class="mutuali-ad-card__title font-weight-bold">{{ adTitle }}</p>

      <ul class="mutuali-ad-card__types">
        <li v-if="adPriceDetails.isAvailableForRent" class="mutuali-ad-card__types-item">
          <b-img :src="require('@/assets/icons/rent.svg')" alt="" height="16" block></b-img>
          <div class="mutuali-ad-card__types-text">
            <div class="mutuali-ad-card__types-title">{{ $t("label.forRent") }}</div>
            <div
              class="mutuali-ad-card__types-price"
              :class="{ 'mutuali-ad-card__types-price--sm': adPriceDetails.rentPriceToBeDetermined }"
            >
              {{ adPriceDetails.rentPrice }}
            </div>
          </div>
        </li>
        <li v-if="adPriceDetails.isAvailableForSale" class="mutuali-ad-card__types-item">
          <b-img :src="require('@/assets/icons/sale.svg')" alt="" height="16" block></b-img>
          <div class="mutuali-ad-card__types-text">
            <div class="mutuali-ad-card__types-title">{{ $t("label.forSale") }}</div>
            <div
              class="mutuali-ad-card__types-price"
              :class="{ 'mutuali-ad-card__types-price--sm': adPriceDetails.salePriceToBeDetermined }"
            >
              {{ adPriceDetails.salePrice }}
            </div>
          </div>
        </li>
        <li v-if="adPriceDetails.isAvailableForTrade" class="mutuali-ad-card__types-item">
          <b-img :src="require('@/assets/icons/trade.svg')" alt="" height="16" block></b-img>
          <div class="mutuali-ad-card__types-text">
            <div class="mutuali-ad-card__types-title">{{ $t("label.forTrade") }}</div>
          </div>
        </li>
        <li v-if="adPriceDetails.isAvailableForDonation" class="mutuali-ad-card__types-item">
          <b-img :src="require('@/assets/icons/donation.svg')" alt="" height="16" block></b-img>
          <div class="mutuali-ad-card__types-text">
            <div class="mutuali-ad-card__types-title">{{ $t("label.forDonation") }}</div>
          </div>
        </li>
      </ul>
    </div>
  </router-link>
</template>

<script>
import AdCategoryBadge from "@/components/ad/category-badge";
import { PriceDetails } from "@/mixins/price-details";

export default {
  components: {
    AdCategoryBadge
  },
  mixins: [PriceDetails],
  props: {
    ad: {
      type: Object,
      required: true
    },
    distance: {
      type: Number,
      required: false
    },
    showPrice: {
      type: Boolean,
      required: false,
      default: true
    }
  },
  computed: {
    adTitle: function () {
      return this.ad.translationOrDefault.title;
    },
    adCategory: function () {
      return this.ad.category;
    },
    adGallery: function () {
      return this.ad.gallery;
    },
    adOrganization: function () {
      return this.ad.organization;
    },
    adDistance: function () {
      return Math.round(this.distance / 100) / 10; // m to km, rounded to first decimal.
    },
    adIsAdminOnly: function () {
      return this.ad.isAdminOnly;
    },
    adPriceDetails: function () {
      return this.getPriceDetailsFromAd(this.ad);
    }
  }
};
</script>

<style lang="scss">
.mutuali-ad-card {
  align-items: flex-start;
  flex-direction: row;
  position: relative;
  border: 0;

  &--admin {
    .mutuali-ad-card__pic {
      border: 1px solid $yellow;
      border-right: 0;
    }
    .mutuali-ad-card__text {
      border: 1px solid $yellow;
      border-left: 0;
    }
  }

  &:hover {
    text-decoration: none;
    color: inherit;

    .mutuali-ad-card__title {
      text-decoration-color: currentColor;
    }
  }

  &__badge {
    position: absolute;
    top: -14px;
    right: $spacer;
  }

  &__title {
    transition: color 0.1s ease-in-out, text-decoration 0.2s ease-in-out;
    text-decoration: underline;
    text-underline-offset: 2px;
    text-decoration-thickness: 2px;
    text-decoration-color: transparent;
    font-size: 20px;
    line-height: 1.2;
    margin-bottom: auto;
  }

  &__pic {
    align-items: stretch;
    align-self: stretch;
    background-color: $light;
    display: flex;
    flex: 0 0 auto;
    justify-content: center;
    min-height: 100px;
    overflow: hidden;
    width: #{"min(100px, 30%)"};
    border-top-left-radius: 6px;
    border-bottom-left-radius: 6px;
    position: relative;
  }

  &__img {
    display: block;
    height: 100%;
    width: 100%;
    object-fit: cover;
    object-position: center;
  }

  &__overlay-admin {
    background-color: $yellow;
    opacity: 0.8;
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
  }

  &__icon-invisible {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
  }

  &__text {
    display: flex;
    flex-direction: column;
    height: 100%;
    flex: 1 1 auto;
    padding: $spacer $spacer * 0.75 $spacer * 0.75;
    border-top-right-radius: 6px;
    border-bottom-right-radius: 6px;
  }

  &__organization {
    display: flex;
    align-items: center;
  }

  &__footer {
    margin-top: auto;
    padding-top: 24px;
  }

  &__circle {
    position: relative;
    top: 1px;
  }

  &__types {
    display: flex;
    flex-wrap: wrap;
    row-gap: $spacer / 1.5;
    column-gap: $spacer;
    list-style-type: none;
    padding-left: 0;
    margin: $spacer 0 0;

    &-item {
      display: flex;
      flex: 1 1 0;
      column-gap: $spacer / 2;
      text-transform: uppercase;
      font-weight: 700;
      color: $gray-700;
      line-height: 1.4;
    }

    &-title {
      font-size: 10px;
      @include media-breakpoint-up(lg) {
        font-size: 12px;
      }
    }

    &-price {
      font-size: 13px;
      text-wrap: nowrap;
      @include media-breakpoint-up(lg) {
        font-size: 14px;
      }

      &--sm {
        font-size: 12px;
        text-transform: none;
        font-weight: 400;
      }
    }
  }
}
</style>
