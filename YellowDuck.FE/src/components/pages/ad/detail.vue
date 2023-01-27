<template>
  <div v-if="ad" class="equipment-detail fab-container" :class="{ 'equipment-detail--modal': displayMap }">
    <template v-if="displayMap">
      <div class="equipment-detail__modal-body">
        <portal :to="$consts.enums.PORTAL_HEADER">
          <nav-close @close="hideMap"></nav-close>
        </portal>
        <google-map
          :markers="adMarkers"
          :zoom-control="false"
          :geolocation-control="false"
          :latitude="adLatitude"
          :longitude="adLongitude"
        />
      </div>
      <div class="equipment-detail__model-footer">
        <p class="equipment-detail__map-location">
          <template v-if="adShowAddress">
            {{ adAddressReadable }}
          </template>
          <template v-else>
            {{ adCity }}
            <span class="small d-block">{{ $t("hint.approximate-location") }}</span>
          </template>
        </p>
      </div>
    </template>
    <div v-else class="mt-4 mb-2">
      <div class="section section--md my-4">
        <breadcrumb :items="breadcrumbs" />
      </div>

      <div class="my-4 equipment-detail__carousel">
        <carousel v-if="adGallery.length > 1" isFullWidth isLight>
          <b-carousel-slide v-for="(item, index) in adGallery" :key="index">
            <template #img>
              <ad-picture :src="item.src" :alt="item.alt ? item.alt : ''" />
            </template>
          </b-carousel-slide>
        </carousel>
        <div v-else class="equipment-detail__carousel-img-container">
          <ad-picture :src="adGallery[0].src" :alt="adGallery[0].alt ? adGallery[0].alt : ''" />
        </div>
      </div>

      <div class="section section--md">
        <div class="mb-n2">
          <div class="container-fluid">
            <div class="row align-items-center">
              <div class="col">
                <ad-category-badge :category="adCategory" />
              </div>
              <div class="col col-auto" v-if="!isAdOwnByCurrentUser">
                <a
                  :aria-label="$t('btn.report-ad')"
                  class="equipment-detail__report btn-link"
                  :href="
                    `mailto:${contactUsEmail}?subject=${$t('email.report-subject')}&body=${$t('email.report-body', {
                      url: adUrlForReport
                    })}`
                  "
                >
                  <b-icon-flag class="mr-sm-2" aria-hidden="true" />
                  <span class="d-none d-sm-inline">{{ $t("btn.report-ad") }}</span>
                </a>
              </div>
            </div>
          </div>

          <p class="mt-2 text-uppercase font-weight-bold letter-spacing-wide small">{{ adOrganization }}</p>
        </div>
        <h1 class="my-4">{{ adTitle }}</h1>
        <p class="mb-0 mt-n2 small text-decoration-none">
          <strong class="h4 font-weight-normal font-family-base mr-1">{{ adPrice }}</strong>
          {{ adPriceDescription }}
        </p>
      </div>
      <div class="section section--md section--border-top section--border-bottom mt-6">
        <div class="equipment-detail__location">
          <p v-if="adShowAddress">
            <span class="responsive-text">{{ adAddressReadable }}</span
            ><br />
            <b-button variant="link" class="p-0 mr-1 font-weight-bolder" @click="showMap">{{
              $t("btn.display-ad-on-map")
            }}</b-button>
          </p>
          <p v-else>
            <span class="responsive-text">{{ adCity }}</span
            ><br />
            <b-button variant="link" class="p-0 mr-1 font-weight-bolder" @click="showMap">{{
              $t("btn.display-ad-on-map")
            }}</b-button>
            <span class="d-inline-block small align-middle">{{ $t("hint.approximate-location") }}</span>
          </p>
        </div>
      </div>
      <user-profile-snippet
        :id="adOwnerProfileId"
        show-registration-date
        hide-organization
        titleTag="p"
        class="section--border-bottom py-4"
      />
      <div class="section section--md py-6">
        <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-description") }}</h2>
        <div class="rm-child-margin" v-html="adDescription"></div>
      </div>
      <div v-if="haveAdConditions" class="section section--md section--border-top py-6">
        <h2 class="font-family-base font-weight-bold mb-4">{{ $t("label.ad-conditions") }}</h2>
        <div class="rm-child-margin" v-html="adConditions"></div>
      </div>
      <div v-if="ad.averageRating > 0" class="section section--md section--border-top my-4">
        <ad-rating-carousel :id="adId" class="mt-4" />
      </div>
      <div class="section section--md section--border-top py-6">
        <h2 class="font-family-base h4 font-weight-bold mb-4">{{ $t("label.ad-disclaimers") }}</h2>
        <div class="small rm-child-margin" v-html="$t('text.ad-disclaimers')"></div>
      </div>
      <div v-if="adIsPublish" class="fab-container__fab section section--md">
        <b-button v-if="!isAdOwnByCurrentUser" variant="primary" size="lg" class="text-truncate" block @click="contactUser">
          <b-icon icon="envelope" class="mr-1" aria-hidden="true"></b-icon>
          {{ $t("btn.contact-owner", { owner: adOwnerName }) }}
        </b-button>
        <template v-else-if="isAdOwnByCurrentUser">
          <b-button variant="primary" size="lg" class="text-truncate" block @click="editAd">
            <b-icon icon="pencil" class="mr-1" aria-hidden="true"></b-icon>
            {{ $t("btn.edit-ad") }}
          </b-button>
          <b-button variant="danger" size="lg" class="text-truncate" block @click="unpublishAd">
            {{ $t("btn.unpublish-ad") }}
          </b-button>
        </template>
      </div>
      <div v-else class="fab-container__fab section section--md section--padding-x bg-light py-3">
        <p v-if="isAdOwnByCurrentUser && haveJustUnpublish">{{ $t("text.ad-notpublish-owner") }}</p>
        <p v-else>{{ $t("text.ad-notpublish") }}</p>
        <template v-if="isAdOwnByCurrentUser">
          <b-button variant="primary" size="sm" class="text-truncate mr-2" @click="publishAd">
            {{ $t("btn.publish-ad") }}
          </b-button>
          <b-button variant="outline-primary" size="sm" class="text-truncate" @click="editAd">
            {{ $t("btn.edit-ad") }}
          </b-button>
        </template>
      </div>
    </div>
  </div>
</template>

<script>
import { URL_LIST_AD, URL_AD_DETAIL, URL_CREATE_CONVERSATION, URL_AD_EDIT } from "@/consts/urls";
import { CONTENT_LANG_FR } from "@/consts/langs";
import { VUE_APP_MUTUALI_CONTACT_MAIL } from "@/helpers/env";

import { unpublishAd, publishAd } from "@/services/ad";

import AdCategoryBadge from "@/components/ad/category-badge";
import AdRatingCarousel from "@/components/ad/rating-carousel";
import AdPicture from "@/components/ad/picture";
import Breadcrumb from "@/components/generic/breadcrumb";
import Carousel from "@/components/generic/carousel";
import GoogleMap from "@/components/generic/google-map";
import NavClose from "@/components/nav/close";
import UserProfileSnippet from "@/components/user-profile/snippet";

export default {
  components: {
    AdCategoryBadge,
    AdRatingCarousel,
    AdPicture,
    Breadcrumb,
    Carousel,
    GoogleMap,
    NavClose,
    UserProfileSnippet
  },
  data() {
    return {
      displayMap: false,
      haveJustUnpublish: false
    };
  },
  computed: {
    contactUsEmail: function() {
      return VUE_APP_MUTUALI_CONTACT_MAIL;
    },
    isConnected: function() {
      return this.user && this.user.isConnected;
    },
    breadcrumbs: function() {
      return [{ to: { name: URL_LIST_AD }, text: this.$t("breadcrumb.list-ad") }, { text: this.adTitle }];
    },
    isAdOwnByCurrentUser: function() {
      return this.me ? this.adOwnerId === this.me.id : false;
    },
    adId: function() {
      return this.$route.params.id.split("-").last();
    },
    adTitle: function() {
      return this.ad.translationOrDefault.title;
    },
    adOrganization: function() {
      return this.ad.organization;
    },
    adDescription: function() {
      return this.ad.translationOrDefault.description;
    },
    adCategory: function() {
      return this.ad.category;
    },
    adGallery: function() {
      return this.ad.gallery;
    },
    adOwnerId: function() {
      return this.ad.user.id;
    },
    adOwnerName: function() {
      return this.ad.user.profile.publicName;
    },
    adOwnerProfileId: function() {
      return this.ad.user.profile.id;
    },
    adCity: function() {
      let neighborhood = this.ad.address.neighborhood ? this.ad.address.neighborhood : this.ad.address.sublocality;
      let locality = this.ad.address.locality;

      if (neighborhood && locality && neighborhood != locality) {
        return `${neighborhood}, ${locality}`;
      } else if (neighborhood) {
        return neighborhood;
      } else {
        return locality;
      }
    },
    adAddress: function() {
      return this.ad.address;
    },
    adAddressReadable: function() {
      const addressElements = [];
      if (this.adAddress.route) addressElements.push(`${this.adAddress.streetNumber} ${this.adAddress.route}`.trim());
      addressElements.push(this.adCity);
      if (this.adAddress.postalCode) addressElements.push(this.adAddress.postalCode);
      return addressElements.join(", ");
    },
    adLatitude: function() {
      return this.ad.address.latitude;
    },
    adLongitude: function() {
      return this.ad.address.longitude;
    },
    adShowAddress: function() {
      return this.ad.showAddress;
    },
    adMarkers: function() {
      const markerIcon = this.adShowAddress
        ? require("@/assets/icons/marker-green.svg")
        : require("@/assets/icons/marker-radius.svg");
      return [{ lat: this.adLatitude, lng: this.adLongitude, icon: markerIcon }];
    },
    adPrice: function() {
      return this.ad.priceToBeDetermined ? this.$t("price.toBeDetermined") : this.$format.formatMoney(this.ad.price);
    },
    adPriceDescription: function() {
      return this.ad.priceToBeDetermined ? "" : this.ad.translationOrDefault.priceDescription;
    },
    adConditions: function() {
      return this.ad.translationOrDefault.conditions;
    },
    adIsPublish: function() {
      return this.ad.isPublish;
    },
    adUrlForReport: function() {
      return encodeURI(window.location.href);
    },
    haveAdConditions: function() {
      let adConditionsEl = document.createElement("div");
      adConditionsEl.innerHTML = this.adConditions;
      return adConditionsEl.textContent.trim() !== "";
    }
  },
  methods: {
    contactUser: function() {
      let routeData = this.$router.resolve({ name: URL_CREATE_CONVERSATION, params: { adId: this.adId } });
      window.open(routeData.href, "_blank");
    },
    showMap: function() {
      this.displayMap = true;
    },
    hideMap: function() {
      this.displayMap = false;
    },
    editAd: function() {
      this.$router.push({ name: URL_AD_EDIT, params: { id: this.adId } });
    },
    unpublishAd: async function() {
      this.haveJustUnpublish = true;
      await unpublishAd(this.adId);
    },
    publishAd: async function() {
      await publishAd(this.adId);
    }
  },
  apollo: {
    ad: {
      query() {
        return this.$options.query.AdById;
      },
      variables() {
        return {
          id: this.adId,
          language: CONTENT_LANG_FR
        };
      },
      result({ data }) {
        if (data) {
          let slugiffyUrl = this.$slugiffyAd(data.ad);
          if (this.$route.params.id !== slugiffyUrl) {
            this.$router.replace({
              name: URL_AD_DETAIL,
              params: { id: slugiffyUrl }
            });
          }
        }
      }
    },
    me: {
      query() {
        return this.$options.query.Me;
      },
      skip() {
        return !this.isConnected;
      }
    },
    user: {
      query() {
        return this.$options.query.LocalUser;
      }
    }
  }
};
</script>

<graphql>
query AdById($id: ID!, $language: ContentLanguage!) {
  ad(id: $id) {
    id
    isPublish
    translationOrDefault(language: $language) {
      id
      language
      title
      description
      priceDescription
      conditions
    }
    address {
      id
      latitude
      longitude
      locality
      postalCode
      route
      streetNumber
      neighborhood
      sublocality
    }
    showAddress
    category
    gallery {
      id
      src
      alt
    }
    user {
      id
      profile {
        id
        ... on UserProfileGraphType {
          publicName
        }
      }
    }
    price
    priceToBeDetermined
    averageRating
    organization
  }
}

query Me {
  me {
    id
  }
}

query LocalUser {
  user @client {
    isConnected
  }
}
</graphql>

<style lang="scss">
.equipment-detail {
  & {
    &--modal {
      align-self: stretch;
      display: flex;
      flex-direction: column;
    }
  }

  &__carousel {
    position: relative;
    z-index: 2;

    &-img-container {
      width: 100%;
      height: 50vh;
      position: relative;
      background-color: $green-darker;
    }
  }

  &__modal {
    &-body {
      display: flex;
      align-self: stretch;
      flex: 1 1 auto;
      flex-direction: column;
    }

    &-footer {
      display: block;
    }
  }

  &__location {
    background: url("~@/assets/icons/marker-green.svg") no-repeat 0 0;
    background-size: 13px auto;
    margin: $spacer * 2 0;
    padding: 0 $spacer 0 calc(#{$spacer} + 18px);
  }

  &__map {
    &-location {
      background: $light url("~@/assets/icons/marker-green.svg") no-repeat $spacer 50%;
      background-size: 13px auto;
      margin: 0;
      padding: $spacer $spacer $spacer calc(#{$spacer * 2} + 13px);
    }
  }

  &__report {
    color: $red;
    position: relative;

    &:hover {
      color: $red-darker;
    }

    &:before {
      content: "";
      position: absolute;
      width: calc(100% + #{$spacer});
      height: calc(100% + #{$spacer} / 2);
      transform: translate(-50%, -50%);
      top: 50%;
      left: 50%;
    }
  }
}
</style>
