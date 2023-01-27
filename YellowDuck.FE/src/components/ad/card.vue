<template>
  <router-link v-if="ad" class="mutuali-ad-card card" :to="{ name: $consts.urls.URL_AD_DETAIL, params: { id: this.id } }">
    <div class="mutuali-ad-card__pic">
      <img
        v-if="adGallery && adGallery[0]"
        class="mutuali-ad-card__img"
        :src="`${adGallery[0].src}?mode=crop&width=150&height=150`"
        :alt="adGallery[0].alt ? adGallery[0].alt : ''"
      />
    </div>
    <div class="mutuali-ad-card__text">
      <ad-category-badge :category="adCategory" />
      <p class="mt-2 mb-0 text-uppercase font-weight-bold letter-spacing-wide smaller">{{ adOrganization }}</p>
      <p class="mutuali-ad-card__title m-0 font-weight-bolder">{{ adTitle }}</p>
      <p v-if="showPrice" class="m-0 small text-decoration-none">
        <strong>{{ adPrice }}</strong>
        {{ adPriceDescription }}
      </p>
    </div>
  </router-link>
</template>

<graphql>
query AdById($id: ID!, $language: ContentLanguage!) {
  ad(id: $id) {
    id
    translationOrDefault(language: $language) {
      id
      language
      title
      priceDescription
    }
    category
    gallery {
      id
      src
      alt
    }
    price
    priceToBeDetermined
    organization
  }
}
</graphql>

<script>
import AdCategoryBadge from "@/components/ad/category-badge";

import { CONTENT_LANG_FR } from "@/consts/langs";

export default {
  components: {
    AdCategoryBadge
  },
  props: {
    id: {
      type: String,
      required: true
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
    adPrice: function () {
      return this.ad.priceToBeDetermined ? this.$t("price.toBeDetermined") : this.$format.formatMoney(this.ad.price);
    },
    adPriceDescription: function () {
      return this.ad.priceToBeDetermined ? "" : this.ad.translationOrDefault.priceDescription;
    },
    adOrganization: function () {
      return this.ad.organization;
    }
  },
  apollo: {
    ad: {
      query() {
        return this.$options.query.AdById;
      },
      variables() {
        return {
          id: this.id,
          language: CONTENT_LANG_FR
        };
      }
    }
  }
};
</script>

<style lang="scss">
.mutuali-ad-card {
  align-items: flex-start;
  flex-direction: row;

  &:hover {
    text-decoration: none;
    color: inherit;

    .mutuali-ad-card__title {
      text-decoration-color: currentColor;
    }
  }

  &__title {
    transition: color 0.1s ease-in-out, text-decoration 0.2s ease-in-out;
    text-decoration: underline;
    text-underline-offset: 2px;
    text-decoration-thickness: 2px;
    text-decoration-color: transparent;
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
  }

  &__img {
    display: block;
    height: 100%;
    width: 100%;
    object-fit: cover;
    object-position: center;
  }

  &__text {
    display: block;
    flex: 1 1 auto;
    padding: $spacer * 0.75;
  }
}
</style>
