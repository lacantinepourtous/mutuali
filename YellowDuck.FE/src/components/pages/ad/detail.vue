<template>
  <div v-if="ad" class="equipment-detail fab-container" :class="{ 'equipment-detail--modal': displayMap }">
    <div v-if="ad.isAdminOnly" class="px-3 py-2 yellow equipment-detail__notif">
      <b-img class="equipment-detail__notif-icon" :src="require('@/assets/icons/invisible.svg')" alt="" height="20" block></b-img>
      {{ $t("banner.ad-is-admin-only") }}
      <b-button size="sm" variant="link" :to="{ name: $consts.urls.URL_AD_TRANSFER, params: { id: this.adId } }">{{
        $t("btn.transfer-ad")
      }}</b-button>
    </div>
    <template v-if="displayMap">
      <div class="equipment-detail__modal-body">
        <portal :to="$consts.enums.PORTAL_HEADER">
          <nav-close @close="hideMap"></nav-close>
        </portal>
        <google-map
          :markers="adMarkers"
          :zoom-control="false"
          :geolocation-control="false"
          :latitude="ad.address.Latitude"
          :longitude="ad.address.Longitude"
        />
      </div>
      <div class="equipment-detail__model-footer">
        <p class="equipment-detail__map-location">
          <template v-if="ad.showAddress">
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
        <carousel v-if="ad.gallery.length > 1" isFullWidth isLight>
          <b-carousel-slide v-for="(item, index) in ad.gallery" :key="index">
            <template #img>
              <ad-picture :src="item.src" :alt="item.alt ? item.alt : ''" />
            </template>
          </b-carousel-slide>
        </carousel>
        <div v-else class="equipment-detail__carousel-img-container">
          <ad-picture :src="ad.gallery[0].src" :alt="ad.gallery[0].alt ? ad.gallery[0].alt : ''" />
        </div>
      </div>

      <div class="section section--md">
        <div class="mb-n2">
          <div class="container-fluid">
            <div class="row align-items-center">
              <div class="col">
                <ad-category-badge :category="ad.category" />
              </div>
              <div class="col col-auto" v-if="!isAdOwnByCurrentUser">
                <a
                  :aria-label="$t('btn.report-ad')"
                  class="equipment-detail__report btn-link"
                  :href="`mailto:${contactUsEmail}?subject=${$t('email.report-subject')}&body=${$t('email.report-body', {
                    url: adUrlForReport
                  })}`"
                >
                  <b-icon-flag class="mr-sm-2" aria-hidden="true" />
                  <span class="d-none d-sm-inline">{{ $t("btn.report-ad") }}</span>
                </a>
              </div>
            </div>
          </div>

          <p class="mt-2 text-uppercase font-weight-bold letter-spacing-wide small">
            {{ ad.organization }}
          </p>
        </div>
        <h1 class="my-4">{{ ad.translationOrDefault.title }}</h1>
        <ul class="equipment-detail__types">
          <li v-if="adPriceDetails.isAvailableForRent">
            <AdTypeCard
              :title="$t('label.forRent')"
              :price="adPriceDetails.rentPrice"
              :modality="adPriceDetails.rentPriceDescription"
              :footnote="adPriceDetails.rentPriceRange"
            >
              <b-img :src="require('@/assets/icons/rent.svg')" alt="" height="30" block></b-img>
            </AdTypeCard>
          </li>
          <li v-if="adPriceDetails.isAvailableForSale">
            <AdTypeCard
              :title="$t('label.forSale')"
              :price="adPriceDetails.salePrice"
              :modality="adPriceDetails.salePriceDescription"
              :footnote="adPriceDetails.salePriceRange"
            >
              <b-img :src="require('@/assets/icons/sale.svg')" alt="" height="30" block></b-img>
            </AdTypeCard>
          </li>
          <li v-if="adPriceDetails.isAvailableForTrade">
            <AdTypeCard :title="$t('label.forTrade')" :description="adPriceDetails.tradeDescription">
              <b-img :src="require('@/assets/icons/trade.svg')" alt="" height="30" block></b-img>
            </AdTypeCard>
          </li>
          <li v-if="adPriceDetails.isAvailableForDonation">
            <AdTypeCard :title="$t('label.forDonation')" :description="adPriceDetails.donationDescription">
              <b-img :src="require('@/assets/icons/donation.svg')" alt="" height="30" block></b-img>
            </AdTypeCard>
          </li>
        </ul>
      </div>
      <div class="section section--md section--border-top section--border-bottom mt-6">
        <div class="equipment-detail__location">
          <p v-if="ad.showAddress">
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
        :id="ad.user.profile.id"
        show-registration-date
        hide-organization
        titleTag="p"
        class="section--border-bottom py-4"
      />
      <detail-partial-delivery-truck v-if="ad.category === CATEGORY_DELIVERY_TRUCK" :ad="ad" />
      <detail-partial-professional-kitchen v-if="ad.category === CATEGORY_PROFESSIONAL_KITCHEN" :ad="ad" />
      <detail-partial-storage-space v-if="ad.category === CATEGORY_STORAGE_SPACE" :ad="ad" />
      <detail-partial-other v-if="ad.category === CATEGORY_OTHER" :ad="ad" />

      <div v-if="adAvailability.length" class="section section--md section--border-top py-6">
        <h2 class="font-family-base font-weight-bold mb-4">
          {{ $t("label.ad-availability") }}
        </h2>
        <ul>
          <li v-for="weekdayAvailability in adAvailability" :key="weekdayAvailability.key" class="mb-3">
            <strong> {{ weekdayAvailability.weekday }} </strong><br />
            <template v-if="weekdayAvailability.availability.day">{{ $t("label.ad-dayAvailability") }}</template>
            <template v-if="weekdayAvailability.availability.day && weekdayAvailability.availability.evening"> • </template>
            <template v-if="weekdayAvailability.availability.evening">{{ $t("label.ad-eveningAvailability") }}</template>
          </li>
        </ul>
      </div>

      <div v-if="ad.averageRating > 0" class="section section--md section--border-top my-4">
        <ad-rating-carousel :id="adId" class="mt-4" />
      </div>
      <div class="section section--md section--border-top py-6">
        <h2 class="font-family-base h4 font-weight-bold mb-4">
          {{ $t("label.ad-disclaimers") }}
        </h2>
        <div class="small rm-child-margin" v-html="$t('text.ad-disclaimers')"></div>
      </div>
      <div v-if="ad.isPublish" class="fab-container__fab section section--md">
        <b-button v-if="!isAdOwnByCurrentUser" variant="admin" size="lg" class="text-truncate" block @click="contactUser">
          <b-icon icon="envelope" class="mr-1" aria-hidden="true"></b-icon>
          {{ $t("btn.contact-owner", { owner: ad.user.profile.publicName }) }}
        </b-button>
        <template v-else-if="isAdOwnByCurrentUser">
          <b-button variant="primary" size="lg" class="text-truncate" block @click="editAd">
            <b-icon icon="pencil" class="mr-1" aria-hidden="true"></b-icon>
            {{ $t("btn.edit-ad") }}
          </b-button>
          <b-button v-if="ad.isAdminOnly" variant="admin" size="lg" class="text-truncate" block @click="transferAd">
            <b-icon icon="arrow-right" class="mr-1" aria-hidden="true"></b-icon>
            {{ $t("btn.transfer-ad") }}
          </b-button>
          <b-button v-if="!ad.isAdminOnly" variant="danger" size="lg" class="text-truncate" block @click="unpublishAd">
            {{ $t("btn.unpublish-ad") }}
          </b-button>
        </template>
      </div>
      <div v-else class="fab-container__fab section section--md section--padding-x bg-light py-3">
        <p v-if="isAdOwnByCurrentUser && haveJustUnpublish">
          {{ $t("text.ad-notpublish-owner") }}
        </p>
        <p v-else>{{ $t("text.ad-notpublish") }}</p>
        <template v-if="isAdOwnByCurrentUser">
          <b-button variant="primary" size="sm" class="text-truncate mr-2" @click="publishAd">
            {{ $t("btn.publish-ad") }}
          </b-button>
          <b-button v-if="ad.isAdminOnly" variant="secondary" size="sm" class="text-truncate mr-2" @click="transferAd">
            {{ $t("btn.transfer-ad") }}
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
import { URL_LIST_AD, URL_AD_DETAIL, URL_CREATE_CONVERSATION, URL_AD_EDIT, URL_AD_TRANSFER } from "@/consts/urls";
import { CONTENT_LANG_FR } from "@/consts/langs";
import {
  CATEGORY_PROFESSIONAL_KITCHEN,
  CATEGORY_DELIVERY_TRUCK,
  CATEGORY_STORAGE_SPACE,
  CATEGORY_OTHER
} from "@/consts/categories";
import { VUE_APP_MUTUALI_CONTACT_MAIL } from "@/helpers/env";

import { unpublishAd, publishAd } from "@/services/ad";
import { AvailabilityWeekday } from "@/mixins/availability-weekday";
import { PriceDetails } from "@/mixins/price-details";

import AdCategoryBadge from "@/components/ad/category-badge";
import AdRatingCarousel from "@/components/ad/rating-carousel";
import AdPicture from "@/components/ad/picture";
import AdTypeCard from "@/components/ad/type-card";
import Breadcrumb from "@/components/generic/breadcrumb";
import Carousel from "@/components/generic/carousel";
import GoogleMap from "@/components/generic/google-map";
import NavClose from "@/components/nav/close";
import UserProfileSnippet from "@/components/user-profile/snippet";
import DetailPartialDeliveryTruck from "@/components/ad/detail-partial-delivery-truck";
import DetailPartialProfessionalKitchen from "@/components/ad/detail-partial-professional-kitchen";
import DetailPartialStorageSpace from "@/components/ad/detail-partial-storage-space";
import DetailPartialOther from "@/components/ad/detail-partial-other";

export default {
  mixins: [AvailabilityWeekday, PriceDetails],
  components: {
    AdCategoryBadge,
    AdRatingCarousel,
    AdPicture,
    AdTypeCard,
    Breadcrumb,
    Carousel,
    GoogleMap,
    NavClose,
    UserProfileSnippet,
    DetailPartialDeliveryTruck,
    DetailPartialProfessionalKitchen,
    DetailPartialStorageSpace,
    DetailPartialOther
  },
  data() {
    return {
      displayMap: false,
      haveJustUnpublish: false,
      CATEGORY_PROFESSIONAL_KITCHEN,
      CATEGORY_DELIVERY_TRUCK,
      CATEGORY_STORAGE_SPACE,
      CATEGORY_OTHER
    };
  },
  computed: {
    contactUsEmail: function () {
      return VUE_APP_MUTUALI_CONTACT_MAIL;
    },
    isConnected() {
      return this.user && this.user.isConnected;
    },
    isAdmin() {
      return !this.me || this.me.type === this.$consts.enums.USER_TYPE_ADMIN;
    },
    breadcrumbs() {
      return [{ to: { name: URL_LIST_AD }, text: this.$t("breadcrumb.list-ad") }, { text: this.adTitle }];
    },
    isAdOwnByCurrentUser() {
      if (this.isAdmin) {
        return this.ad.isAdminOnly;
      } else {
        return this.me ? this.ad.user.id === this.me.id : false;
      }
    },
    adId() {
      return this.$route.params.id.split("-").last();
    },
    adCity() {
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
    adAddressReadable() {
      const addressElements = [];
      if (this.ad.address.route) addressElements.push(`${this.ad.address.streetNumber} ${this.ad.address.route}`.trim());
      addressElements.push(this.ad.city);
      if (this.ad.address.postalCode) addressElements.push(this.ad.address.postalCode);
      return addressElements.join(", ");
    },
    adMarkers() {
      const markerIcon = this.ad.showAddress
        ? require("@/assets/icons/marker-red.svg")
        : require("@/assets/icons/marker-radius.svg");
      return [
        {
          lat: this.ad.address.latitude,
          lng: this.ad.address.longitude,
          icon: markerIcon
        }
      ];
    },
    adPriceDetails() {
      return this.getPriceDetailsFromAd(this.ad);
    },
    adUrlForReport() {
      return encodeURI(window.location.href);
    },
    adAvailability() {
      var adAvailability = [];
      for (var availabilityWeekdayOption of this.availabilityWeekdayOptions) {
        var availability = { day: false, evening: false };
        if (this.ad.dayAvailability.includes(availabilityWeekdayOption.value)) {
          availability.day = true;
        }
        if (this.ad.eveningAvailability.includes(availabilityWeekdayOption.value)) {
          availability.evening = true;
        }
        if (availability.day || availability.evening) {
          adAvailability.push({
            key: availabilityWeekdayOption.value,
            weekday: availabilityWeekdayOption.text,
            availability
          });
        }
      }
      return adAvailability;
    }
  },
  methods: {
    contactUser() {
      let routeData = this.$router.resolve({
        name: URL_CREATE_CONVERSATION,
        params: { adId: this.adId }
      });
      window.open(routeData.href, "_blank");
    },
    showMap() {
      this.displayMap = true;
    },
    hideMap() {
      this.displayMap = false;
    },
    editAd() {
      this.$router.push({ name: URL_AD_EDIT, params: { id: this.adId } });
    },
    transferAd() {
      this.$router.push({ name: URL_AD_TRANSFER, params: { id: this.adId } });
    },
    async unpublishAd() {
      this.haveJustUnpublish = true;
      await unpublishAd(this.adId);
    },
    async publishAd() {
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
    isAdminOnly
    isAvailableForRent
    isAvailableForSale
    isAvailableForTrade
    isAvailableForDonation
    translationOrDefault(language: $language) {
      id
      language
      title
      description
      rentPriceDescription
      salePriceDescription
      tradeDescription
      donationDescription
      conditions
      equipment
      surfaceSize
      surfaceDescription
      professionalKitchenEquipmentOther
      deliveryTruckTypeOther
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
    rentPrice
    salePrice
    rentPriceToBeDetermined
    salePriceToBeDetermined
    rentPriceRange
    salePriceRange
    averageRating
    organization
    refrigerated
    canSharedRoad
    canHaveDriver
    professionalKitchenEquipment
    deliveryTruckType
    dayAvailability
    eveningAvailability
  }
}

query Me {
  me {
    id
    type
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

  &__notif {
    background-color: $yellow;
    display: flex;
    align-items: center;
    column-gap: 12px;
  }

  &__carousel {
    position: relative;
    z-index: 2;

    &-img-container {
      width: 100%;
      height: 50vh;
      position: relative;
      background-color: $gray-900;
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
    background: url("~@/assets/icons/marker-red.svg") no-repeat 0 0;
    background-size: 13px auto;
    margin: $spacer * 2 0;
    padding: 0 $spacer 0 calc(#{$spacer} + 18px);
  }

  &__map {
    &-location {
      background: $light url("~@/assets/icons/marker-red.svg") no-repeat $spacer 50%;
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

  &__types {
    list-style-type: none;
    padding-left: 0;

    @include media-breakpoint-up(lg) {
      display: flex;
      column-gap: $spacer;
    }

    & > li {
      margin-bottom: $spacer / 2;

      @include media-breakpoint-up(lg) {
        flex: 1 1 0;
        margin-bottom: 0;
      }
    }
  }
}
</style>
