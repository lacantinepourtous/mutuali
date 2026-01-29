<template>
  <div v-if="ad" class="equipment-detail fab-container" :class="[getCategoryGroupByCategory(ad.category).color, { 'equipment-detail--modal': displayMap }]">
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
          <svg class="equipment-detail__map-location-icon" width="18" height="41" viewBox="0 0 18 41" fill="currentColor" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
            <path fill-rule="evenodd" clip-rule="evenodd" d="M8.60552 40.7989C2.88217 32.8965 0 22.2477 0 8.69393C0 3.89609 3.85199 0.0137671 8.60552 0C13.359 0.0137671 17.211 3.89609 17.211 8.69393C17.211 22.2477 14.3357 32.8965 8.60552 40.7989ZM8.605 14.21C11.7006 14.21 14.21 11.7006 14.21 8.605C14.21 5.50944 11.7006 3 8.605 3C5.50944 3 3 5.50944 3 8.605C3 11.7006 5.50944 14.21 8.605 14.21Z" />
          </svg>
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
      <div class="section section--full-width my-4">
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

      <div class="layout" :class="isAdOwnByCurrentUser ? 'section section--md' : 'layout--sticky px-4'">
        <div v-if="!isAdOwnByCurrentUser" class="layout__sticky d-none d-md-block">
          <div class="layout__sticky-inside grey-box">
            <user-profile-snippet
              :id="ad.user.profile.id"
              show-registration-date
              hide-organization
              titleTag="p"
            />
            
            <div class="text-center border-top border-white pt-3">
              <h2 class="h6 text-uppercase font-weight-bolder mb-1">
                {{ $t("title.contact-owner") }}
              </h2>
              <p class="small mb-2 font-weight-bold mx-3">
                {{ $t("description.contact-owner") }}
              </p>
              <b-button v-if="!isAdOwnByCurrentUser" variant="admin" class="text-truncate d-flex align-items-center justify-content-center" block @click="contactUser">
                <svg class="mr-2" xmlns="http://www.w3.org/2000/svg" width="27" height="27" viewBox="0 0 27 27" fill="none" aria-hidden="true">
                  <path fill-rule="evenodd" clip-rule="evenodd" d="M5.22192 5.78931C3.4327 5.78931 1.96973 7.26373 1.96973 9.05429V19.6097C1.96973 21.4002 3.4327 22.8747 5.22192 22.8747H6.46061V25.886L6.46188 25.8873C6.46443 26.2556 6.67852 26.5907 7.01241 26.7475C7.34628 26.9042 7.74008 26.8558 8.02681 26.6238L12.5814 22.876H21.7402C23.5294 22.876 25 21.4016 25 19.6111V9.05565C25 7.26517 23.5294 5.79067 21.7402 5.79067L5.22192 5.78931ZM5.22192 7.70849H21.739C22.4921 7.70849 23.0796 8.28959 23.0796 9.05422V19.6096C23.0796 20.3743 22.4934 20.9553 21.739 20.9553H12.2334C12.0117 20.9566 11.7963 21.0344 11.6243 21.1758L8.37973 23.8469V21.9124L8.381 21.9111C8.37845 21.3822 7.94646 20.9541 7.41759 20.9553H5.22313C4.46998 20.9553 3.89016 20.3755 3.89016 19.6096V9.05421C3.89016 8.28831 4.4687 7.70848 5.22313 7.70848L5.22192 7.70849ZM8.38743 11.1239C8.13383 11.1226 7.88916 11.2233 7.7082 11.403C7.52724 11.5814 7.42529 11.8248 7.42402 12.0796C7.42402 12.3358 7.52469 12.5817 7.70565 12.7627C7.88661 12.9424 8.13257 13.0443 8.38742 13.0431H18.5823C18.8359 13.0418 19.0806 12.9398 19.259 12.7589C19.4387 12.5792 19.5381 12.3345 19.5381 12.0797C19.5355 11.5533 19.1086 11.1264 18.5823 11.1239L8.38743 11.1239ZM8.38743 15.6223C8.13383 15.6211 7.88916 15.7217 7.7082 15.9002C7.52724 16.0798 7.42529 16.3233 7.42402 16.5781C7.42402 16.8343 7.52469 17.0789 7.70565 17.2599C7.88661 17.4408 8.13257 17.5428 8.38742 17.5415H14.3794C14.6356 17.5428 14.8815 17.4408 15.0625 17.2599C15.2435 17.0789 15.3441 16.8342 15.3428 16.5781C15.3416 16.3232 15.2396 16.0798 15.0599 15.9002C14.879 15.7217 14.6343 15.6211 14.3794 15.6223H8.38743Z" fill="#414042"/>
                </svg>
                {{ $t("btn.contact-owner") }}
              </b-button>
            </div>
          </div>
        </div>

        <div class="layout__content" :class="{ 'layout__content--sticky': !isAdOwnByCurrentUser }">
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

            <h1 class="my-4">{{ ad.translationOrDefault.title }}</h1>
            <detail-transaction-types :ad="ad" />
          </div>

          <div class="border-top border-grey mt-6">
            <div class="equipment-detail__location">
              <svg class="equipment-detail__location-icon" width="18" height="41" viewBox="0 0 18 41" fill="currentColor" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
                <path fill-rule="evenodd" clip-rule="evenodd" d="M8.60552 40.7989C2.88217 32.8965 0 22.2477 0 8.69393C0 3.89609 3.85199 0.0137671 8.60552 0C13.359 0.0137671 17.211 3.89609 17.211 8.69393C17.211 22.2477 14.3357 32.8965 8.60552 40.7989ZM8.605 14.21C11.7006 14.21 14.21 11.7006 14.21 8.605C14.21 5.50944 11.7006 3 8.605 3C5.50944 3 3 5.50944 3 8.605C3 11.7006 5.50944 14.21 8.605 14.21Z" />
              </svg>
              <p v-if="ad.showAddress" class="mb-0">
                <span class="responsive-text">{{ adAddressReadable }}</span
                ><br />
                <b-button variant="link" class="p-0 mr-1 font-weight-bolder" @click="showMap">{{
                  $t("btn.display-ad-on-map")
                }}</b-button>
              </p>
              <p v-else class="mb-0">
                <span class="responsive-text">{{ adCity }}</span
                ><br />
                <b-button variant="link" class="p-0 mr-1 font-weight-bolder" @click="showMap">{{
                  $t("btn.display-ad-on-map")
                }}</b-button>
                <span class="d-inline-block small align-middle">{{ $t("hint.approximate-location") }}</span>
              </p>
            </div>
          </div>

          <detail-partial-delivery-truck v-if="ad.category === CATEGORY_DELIVERY_TRUCK" :ad="ad" />
          <detail-partial-professional-kitchen v-if="ad.category === CATEGORY_PROFESSIONAL_KITCHEN" :ad="ad" />
          <detail-partial-storage-space v-if="ad.category === CATEGORY_STORAGE_SPACE" :ad="ad" />
          <detail-partial-subcontracting v-if="ad.category === CATEGORY_SUBCONTRACTING" :ad="ad" />
          <detail-partial-human-resources v-if="ad.category === CATEGORY_HUMAN_RESOURCES" :ad="ad" />
          <detail-partial-other v-if="isMiscCategory" :ad="ad" />

          <div v-if="adAvailability.length && (ad.isAvailableForRent || ad.category === CATEGORY_HUMAN_RESOURCES)" class="border-top border-grey py-6">
            <h2 class="font-family-base font-weight-bold mb-4">
              {{ $t("label.availability") }}
            </h2>
            <detail-calendar
              :availability="adAvailability"
              :restrictions="ad.availabilityRestriction"
              :view-only="isAdOwnByCurrentUser"
              @update-conversation-message="(v) => (conversationMessage = v)"
            />
          </div>

          <div v-if="ad.averageRating > 0 || ad.adRatings.some(r => r.comment)" class="border-top border-grey my-4">
            <ad-rating-carousel :id="adId" class="mt-4" @rating-deleted="onRatingDeleted" />
          </div>

          <div class="border-top border-grey py-6">
            <h2 class="font-family-base h4 font-weight-bold mb-4">
              {{ $t("label.ad-disclaimers") }}
            </h2>
            <div class="small rm-child-margin" v-html="$t('text.ad-disclaimers')"></div>
          </div>

          <div v-if="!isAdOwnByCurrentUser" class="user-mobile grey-box d-md-none mx-n2">
            <user-profile-snippet
              :id="ad.user.profile.id"
              show-registration-date
              hide-organization
              titleTag="p"
            />
          </div>
        </div>
      </div>
     
      <div v-if="ad.isPublish" class="fab-container__fab" :class="(isAdOwnByCurrentUser || isAdmin) ? 'text-center bg-light p-1 sm:p-3' : 'px-3'">
        <template v-if="isAdOwnByCurrentUser || isAdmin">
          <b-button variant="primary" size="lg" class="text-truncate mx-1" @click="editAd">
            <b-icon icon="pencil" class="mr-md-1" aria-hidden="true"></b-icon>
            <span class="sr-only-sm">{{ $t("btn.edit-ad") }}</span>
          </b-button>
          <b-button v-if="ad.isAdminOnly" variant="admin" size="lg" class="text-truncate mx-1" @click="transferAd">
            <b-icon icon="arrow-right" class="mr-md-1" aria-hidden="true"></b-icon>
            <span class="sr-only-sm">{{ $t("btn.transfer-ad") }}</span>
          </b-button>
          <b-button v-if="!ad.isAdminOnly" variant="danger" size="lg" class="text-truncate mx-1" @click="unpublishAd">
            <b-icon icon="eye-slash" class="mr-md-1" aria-hidden="true"></b-icon>
            <span class="sr-only-sm">{{ $t("btn.unpublish-ad") }}</span>
          </b-button>
          <b-button v-if="isAdmin && !ad.locked" variant="warning" size="lg" class="text-truncate mx-1" @click="lockAd">
            <b-icon icon="x-circle" class="mr-md-1" aria-hidden="true"></b-icon>
            <span class="sr-only-sm">{{ $t("btn.lock-ad") }}</span>
          </b-button>
          <b-button v-if="isAdmin && ad.locked" variant="success" size="lg" class="text-truncate mx-1" @click="unlockAd">
            <b-icon icon="check-circle" class="mr-md-1" aria-hidden="true"></b-icon>
            <span class="sr-only-sm">{{ $t("btn.unlock-ad") }}</span>
          </b-button>
        </template>
        <div v-else class="grey-box border-top border-white text-center d-md-none">
          <h2 class="h6 text-uppercase font-weight-bolder mb-1">
            {{ $t("title.contact-owner") }}
          </h2>
          <p class="small mb-2 font-weight-bold">
            {{ $t("description.contact-owner") }}
          </p>
          <b-button variant="admin" class="text-truncate d-flex align-items-center justify-content-center" block @click="contactUser">
            <svg class="mr-2" xmlns="http://www.w3.org/2000/svg" width="27" height="27" viewBox="0 0 27 27" fill="none" aria-hidden="true">
              <path fill-rule="evenodd" clip-rule="evenodd" d="M5.22192 5.78931C3.4327 5.78931 1.96973 7.26373 1.96973 9.05429V19.6097C1.96973 21.4002 3.4327 22.8747 5.22192 22.8747H6.46061V25.886L6.46188 25.8873C6.46443 26.2556 6.67852 26.5907 7.01241 26.7475C7.34628 26.9042 7.74008 26.8558 8.02681 26.6238L12.5814 22.876H21.7402C23.5294 22.876 25 21.4016 25 19.6111V9.05565C25 7.26517 23.5294 5.79067 21.7402 5.79067L5.22192 5.78931ZM5.22192 7.70849H21.739C22.4921 7.70849 23.0796 8.28959 23.0796 9.05422V19.6096C23.0796 20.3743 22.4934 20.9553 21.739 20.9553H12.2334C12.0117 20.9566 11.7963 21.0344 11.6243 21.1758L8.37973 23.8469V21.9124L8.381 21.9111C8.37845 21.3822 7.94646 20.9541 7.41759 20.9553H5.22313C4.46998 20.9553 3.89016 20.3755 3.89016 19.6096V9.05421C3.89016 8.28831 4.4687 7.70848 5.22313 7.70848L5.22192 7.70849ZM8.38743 11.1239C8.13383 11.1226 7.88916 11.2233 7.7082 11.403C7.52724 11.5814 7.42529 11.8248 7.42402 12.0796C7.42402 12.3358 7.52469 12.5817 7.70565 12.7627C7.88661 12.9424 8.13257 13.0443 8.38742 13.0431H18.5823C18.8359 13.0418 19.0806 12.9398 19.259 12.7589C19.4387 12.5792 19.5381 12.3345 19.5381 12.0797C19.5355 11.5533 19.1086 11.1264 18.5823 11.1239L8.38743 11.1239ZM8.38743 15.6223C8.13383 15.6211 7.88916 15.7217 7.7082 15.9002C7.52724 16.0798 7.42529 16.3233 7.42402 16.5781C7.42402 16.8343 7.52469 17.0789 7.70565 17.2599C7.88661 17.4408 8.13257 17.5428 8.38742 17.5415H14.3794C14.6356 17.5428 14.8815 17.4408 15.0625 17.2599C15.2435 17.0789 15.3441 16.8342 15.3428 16.5781C15.3416 16.3232 15.2396 16.0798 15.0599 15.9002C14.879 15.7217 14.6343 15.6211 14.3794 15.6223H8.38743Z" fill="#414042"/>
            </svg>
            {{ $t("btn.contact-owner") }}
          </b-button>
        </div>
      </div>
      <div v-else class="fab-container__fab mt-4" :class="(isAdOwnByCurrentUser || isAdmin) ? 'text-center bg-light p-3' : 'px-3'">
        <p v-if="ad.locked">{{ $t("text.ad-locked") }}</p>
        <p v-else>{{ $t("text.ad-notpublish") }}</p>
        <template v-if="isAdOwnByCurrentUser || isAdmin">
          <b-button v-if="!ad.locked" variant="primary" size="sm" class="text-truncate mr-2" @click="publishAd">
            {{ $t("btn.publish-ad") }}
          </b-button>
          <b-button v-if="ad.isAdminOnly" variant="secondary" size="sm" class="text-truncate mr-2" @click="transferAd">
            {{ $t("btn.transfer-ad") }}
          </b-button>
          <b-button v-if="isAdmin && !ad.locked" variant="warning" size="sm" class="text-truncate mr-2" @click="lockAd">
            {{ $t("btn.lock-ad") }}
          </b-button>
          <b-button v-if="isAdmin && ad.locked" variant="success" size="sm" class="text-truncate mr-2" @click="unlockAd">
            {{ $t("btn.unlock-ad") }}
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
  CATEGORY_PROFESSIONAL_COOKING_EQUIPMENT,
  CATEGORY_PREP_EQUIPMENT,
  CATEGORY_REFRIGERATION_EQUIPMENT,
  CATEGORY_HEAVY_EQUIPMENT,
  CATEGORY_SURPLUS,
  CATEGORY_SUBCONTRACTING,
  CATEGORY_HUMAN_RESOURCES,
  CATEGORY_OTHER
} from "@/consts/categories";
import { VUE_APP_MUTUALI_CONTACT_MAIL } from "@/helpers/env";


import { unpublishAd, publishAd, lockAd, unlockAd } from "@/services/ad";
import { AvailabilityWeekday } from "@/mixins/availability-weekday";
import { AdCategory } from "@/mixins/ad-category";

import NotificationService from "@/services/notification";

import AdCategoryBadge from "@/components/ad/category-badge";
import AdRatingCarousel from "@/components/ad/rating-carousel";
import AdPicture from "@/components/ad/picture";
import Breadcrumb from "@/components/generic/breadcrumb";
import Carousel from "@/components/generic/carousel";
import GoogleMap from "@/components/generic/google-map";
import NavClose from "@/components/nav/close";
import UserProfileSnippet from "@/components/user-profile/snippet";
import DetailPartialDeliveryTruck from "@/components/ad/detail-partial-delivery-truck";
import DetailPartialProfessionalKitchen from "@/components/ad/detail-partial-professional-kitchen";
import DetailPartialStorageSpace from "@/components/ad/detail-partial-storage-space";
import DetailPartialSubcontracting from "@/components/ad/detail-partial-subcontracting";
import DetailPartialHumanResources from "@/components/ad/detail-partial-human-resources";
import DetailPartialOther from "@/components/ad/detail-partial-other";
import DetailCalendar from "@/components/ad/detail-calendar";
import DetailTransactionTypes from "@/components/ad/detail-transaction-types";

export default {
  mixins: [AvailabilityWeekday, AdCategory],
  components: {
    AdCategoryBadge,
    AdRatingCarousel,
    AdPicture,
    Breadcrumb,
    Carousel,
    GoogleMap,
    NavClose,
    UserProfileSnippet,
    DetailPartialDeliveryTruck,
    DetailPartialProfessionalKitchen,
    DetailPartialStorageSpace,
    DetailPartialSubcontracting,
    DetailPartialHumanResources,
    DetailPartialOther,
    DetailCalendar,
    DetailTransactionTypes
  },
  data() {
    return {
      displayMap: false,
      haveJustUnpublish: false,
      conversationMessage: "",
      CATEGORY_PROFESSIONAL_KITCHEN,
      CATEGORY_DELIVERY_TRUCK,
      CATEGORY_STORAGE_SPACE,
      CATEGORY_PROFESSIONAL_COOKING_EQUIPMENT,
      CATEGORY_PREP_EQUIPMENT,
      CATEGORY_REFRIGERATION_EQUIPMENT,
      CATEGORY_HEAVY_EQUIPMENT,
      CATEGORY_SURPLUS,
      CATEGORY_SUBCONTRACTING,
      CATEGORY_HUMAN_RESOURCES,
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
      return this.me && this.me.type === this.$consts.enums.USER_TYPE_ADMIN;
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
      if (this.ad.address.locality) addressElements.push(this.ad.address.locality);
      if (this.ad.address.postalCode) addressElements.push(this.ad.address.postalCode);
      return addressElements.join(", ");
    },
    adMarkers() {
      const markerIcon = this.ad.showAddress
        ? require(`@/assets/icons/marker-${this.getCategoryGroupByCategory(this.ad.category).color}.svg`)
        : require("@/assets/icons/marker-radius.svg");
      return [
        {
          lat: this.ad.address.latitude,
          lng: this.ad.address.longitude,
          icon: markerIcon
        }
      ];
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
    },
    isMiscCategory() {
      const miscCategories = [
        CATEGORY_OTHER,
        CATEGORY_PROFESSIONAL_COOKING_EQUIPMENT,
        CATEGORY_PREP_EQUIPMENT,
        CATEGORY_REFRIGERATION_EQUIPMENT,
        CATEGORY_HEAVY_EQUIPMENT,
        CATEGORY_SURPLUS
      ];
      return miscCategories.includes(this.ad.category);
    }
  },
  methods: {
    contactUser() {
      let routeData = this.$router.resolve({
        name: URL_CREATE_CONVERSATION,
        params: { adId: this.adId },
        query: {
          message: this.conversationMessage
        }
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
      NotificationService.showSuccess(this.$t("text.ad-notpublish-owner"));
    },
    async publishAd() {
      await publishAd(this.adId);
    },
    async lockAd() {
      await lockAd(this.adId);
    },
    async unlockAd() {
      await unlockAd(this.adId);
    },
    onRatingDeleted(ratingId) {
      if (this.$apollo.queries.ad) {
        this.$apollo.queries.ad.refetch();
      }
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
    locked
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
      humanResourceFieldOther
      tasks
      qualifications
      geographicCoverage
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
    adRatings {
      id
      comment
    }
    organization
    refrigerated
    canSharedRoad
    canHaveDriver
    professionalKitchenEquipment
    deliveryTruckType
    humanResourceField
    dayAvailability
    eveningAvailability
    availabilityRestriction {
      id
      startDate
      day
      evening
    }
    certification
    allergen
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

<style lang="scss" scoped>
.layout {
  &--sticky {
    max-width: 1200px;
    margin: 0 auto;
  }

  &__sticky {
    position: sticky;
    top: 80px;
    width: 100%;

    &-inside {
      position: absolute;
      top: 0;
      right: 0;
      width: 280px;
    }
  }

  &__content {
    &--sticky {
      @include media-breakpoint-up(md) {
        width: calc(100% - 340px);
      }
    }
  }
}

.grey-box {
  background-color: $green-lighter;
  border-radius: 8px;
  padding: 16px 14px;
}

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
    margin: $spacer * 2 0;
    padding: 0 $spacer;
    display: flex;
    align-items: start;
    column-gap: $spacer;

    &-icon {
      color: var(--accent-color);
      width: 13px;
    }
  }

  &__map {
    &-location {
      margin: 0;
      padding: $spacer;

      &-icon {
        color: var(--accent-color);
        width: 13px;
      }
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
