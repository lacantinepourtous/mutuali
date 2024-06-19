<template>
  <div class="equipment-list" :class="{ 'equipment-list--view-list': view === LIST_VIEW }">
    <div ref="adViewStatus" role="status" class="sr-only" aria-live="polite"></div>
    <div v-if="displayAdvancedFilters" class="equipment-list__modal-body">
      <portal :to="$consts.enums.PORTAL_HEADER">
        <nav-close ref="filterNavbar" @close="hideAdvancedFilters"></nav-close>
      </portal>
      <ad-filters v-model="filters" @input="onFiltersUpdated()" />
    </div>
    <template v-else-if="ads !== undefined">
      <div class="equipment-list__header">
        <div class="section py-3">
          <div class="equipment-list__header-top">
            <s-form-google-autocomplete
              class="equipment-list__google-address"
              :show-search-button="true"
              v-model="address"
              @input="addPositionMarker"
              :placeholder="$t('label.adsList.searchAddress')"
              id="address"
              margin="none"
              name="address"
              :label="$t('label.address')"
              label-sr-only
            />
            <dropdown
              class="equipment-list__filters-dropdown"
              v-model="filters.category"
              @input="onFiltersUpdated()"
              id="adCategory"
              :label="categoryLabel(filters.category)"
              :options="categoryOptions"
              right
              variant="outline-secondary"
            />
            <b-button class="equipment-list__filters-btn" variant="outline-secondary" @click="showAdvancedFilters">
              <span class="sr-only">{{ $t("sr.open-advanced-search") }}</span>
              <b-icon-sliders aria-hidden="true"></b-icon-sliders>
              <span v-if="filtersCount != 0" class="equipment-list__filters-btn-badge">
                {{ filtersCount }}
                <span class="sr-only">{{ $t("sr.active-filters") }}</span>
              </span>
            </b-button>
          </div>
        </div>

        <div class="equipment-list__header-bottom">
          <div class="section">
            <div class="equipment-list__header-bottom-btns">
              <b-button
                :aria-label="$t('btn.show-ad-card')"
                class="equipment-list__toggle-view"
                :class="{ 'equipment-list__toggle-view--active': view === CARD_VIEW }"
                variant="primary"
                @click="setView(CARD_VIEW)"
                >{{ $t("label.card") }}</b-button
              >
              <b-button
                :aria-label="$t('btn.show-ad-list')"
                class="equipment-list__toggle-view"
                :class="{ 'equipment-list__toggle-view--active': view === LIST_VIEW }"
                variant="primary"
                @click="setView(LIST_VIEW)"
                >{{ $t("label.list") }}</b-button
              >
              <b-button
                v-if="filtersCount != 0"
                @click="reinitFilters()"
                class="equipment-list__reset-filters-btn"
                variant="link"
                >{{ $t("label-reinit-filters") }}</b-button
              >
            </div>
          </div>
        </div>
      </div>

      <google-map
        ref="map"
        v-show="view === CARD_VIEW"
        :markers="adMarkers"
        marker-clickable
        @mapClicked="mapClicked"
        @markerClicked="markerClicked"
        @mapMoved="mapMoved"
        @zoomChanged="zoomChanged"
        :latitude="initialLatitude"
        :longitude="initialLongitude"
        :zoom="initialZoomLevel"
      />

      <div v-show="view === LIST_VIEW" class="equipment-list__scroll">
        <div class="section mt-3 mb-6">
          <div class="equipment-list__list-header mb-4">
            <div class="equipment-list__list-header-section mr-2">
              <ad-category-badge v-if="filters.category" :category="filters.category" closable @close="filters.category = null" />
            </div>

            <div class="equipment-list__list-header-section ml-2">
              <dropdown
                v-model="sort"
                @input="applySorting()"
                id="sort"
                class="w-100"
                :label="sortLabel(sort)"
                :options="sortOptionsAvailable"
                right
                variant="outline-secondary"
              />
            </div>
          </div>

          <div class="equipment-list__list-view-row">
            <div class="equipment-list__list-view-col" v-for="adMarker in adMarkers" :key="adMarker.id">
              <ad-card class="equipment-list__list-view-card" :ad="adMarker.ad" :distance="adMarker.distance" />
            </div>
          </div>

          <div v-if="adMarkers.length === 0" class="no-content my-5 py-sm-4">
            <img class="no-content__img mb-4" alt="" :src="require('@/assets/ambiance/nothing.svg')" />
            <p>{{ $t("text.no-result") }}</p>
            <div class="mb-2">
              <b-button variant="primary" v-if="filtersCount != 0" @click="reinitFilters()">{{
                $t("label-reinit-filters")
              }}</b-button>
            </div>
            <div class="mb-2">
              <b-button variant="outline-primary" :to="{ name: $consts.urls.URL_LIST_AD_ALERT }">{{
                $t("btn.create-alert")
              }}</b-button>
            </div>
          </div>
        </div>
      </div>
      <alert-modal v-if="adMarkers.length === 0 && view === CARD_VIEW" :icon="require('@/assets/ambiance/nothing.svg')">
        <p>{{ $t("text.no-result") }}</p>
        <template v-slot:footer>
          <b-button variant="primary" v-if="filtersCount != 0" @click="reinitFilters()" class="mr-2">{{
            $t("label-reinit-filters")
          }}</b-button>
          <b-button variant="outline-primary" :to="{ name: $consts.urls.URL_AD_ALERT_ADD }">{{
            $t("btn.create-alert")
          }}</b-button>
        </template>
      </alert-modal>
      <alert-modal v-else-if="isConnected && !createAlertModalHidden" :icon="require('@/assets/ambiance/bell.svg')">
        <p>
          <strong>{{ $t("title.modal-first-login") }}</strong>
          {{ $t("text.modal-first-login") }}
        </p>
        <template v-slot:footer>
          <b-button variant="primary" :to="{ name: $consts.urls.URL_AD_ALERT_ADD }" class="mr-2">{{
            $t("btn.create-alert")
          }}</b-button>
          <b-button @click="hideCreateAlertModal()" variant="outline-primary" class="mr-2">{{ $t("btn.later") }}</b-button>
        </template>
      </alert-modal>
      <alert-modal
        v-else-if="!isConnected && !createAlertModalHidden"
        closable
        @close="hideCreateAlertModal()"
        :icon="require('@/assets/ambiance/bell.svg')"
      >
        <p>
          <strong>{{ $t("title.modal-first-visit") }}</strong>
          {{ $t("text.modal-first-visit") }}
        </p>
        <template v-slot:footer>
          <b-button variant="primary" :to="{ name: $consts.urls.URL_AD_ALERT_ADD }" class="mr-2">{{
            $t("btn.subscribe-alert")
          }}</b-button>
          <b-button variant="outline-primary" :to="{ name: $consts.urls.URL_LOGIN }">{{ $t("btn.login-alt") }}</b-button>
        </template>
      </alert-modal>
      <div class="equipment-list__bottom section section--md">
        <ad-card v-if="displayAdSnippet && view === CARD_VIEW" :ad="snippetAd" />
        <template v-else-if="isConnected && createAlertModalHidden && adMarkers.length != 0">
          <div v-if="isAdmin" class="mx-4">
            <b-button variant="admin" size="lg" block :to="{ name: $consts.urls.URL_PREPARE_AD }">{{
              $t("nav.prepare-ad")
            }}</b-button>
          </div>
          <div v-else class="mx-4">
            <b-button variant="admin" size="lg" block :to="{ name: $consts.urls.URL_CREATE_AD }">{{
              $t("nav.create-ad")
            }}</b-button>
          </div>
        </template>
      </div>
    </template>
  </div>
</template>

<graphql>
query Me {
  me {
    id
    type
    firstLoginModalClosed
  }
}

query LocalUser {
  user @client {
    isConnected
  }
}

query Ads(
  $category: AdCategory = null
  $dayAvailability: [DayOfWeek!] = null
  $eveningAvailability: [DayOfWeek!] = null
  $professionalKitchenEquipment: [ProfessionalKitchenEquipment!] = null
  $deliveryTruckType: DeliveryTruckType = null
  $refrigerated: Boolean = null
  $canSharedRoad: Boolean = null
  $canHaveDriver: Boolean = null
  $language: ContentLanguage!
) {
  ads(
    category: $category
    dayAvailability: $dayAvailability
    eveningAvailability: $eveningAvailability
    professionalKitchenEquipment: $professionalKitchenEquipment
    deliveryTruckType: $deliveryTruckType
    refrigerated: $refrigerated
    canSharedRoad: $canSharedRoad
    canHaveDriver: $canHaveDriver
  ) {
    id
    isPublish
    isAdminOnly
    createdAtUTC
    address {
      id
      latitude
      longitude
    }
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
import { randomPosition } from "@/helpers/latLngRandomize";

import { URL_LIST_AD } from "@/consts/urls";
import { LOCAL_STORAGE_MAP_LATLONG, LOCAL_STORAGE_MAP_ZOOMLEVEL } from "@/consts/local-storage";
import {
  CATEGORY_PROFESSIONAL_KITCHEN,
  CATEGORY_DELIVERY_TRUCK,
  CATEGORY_STORAGE_SPACE,
  CATEGORY_OTHER
} from "@/consts/categories";
import { CONTENT_LANG_FR } from "@/consts/langs";

import AdCard from "@/components/ad/card";
import AdFilters from "@/components/ad/filters";
import Dropdown from "@/components/generic/dropdown";
import SFormGoogleAutocomplete from "@/components/form/s-form-google-autocomplete";
import GoogleMap from "@/components/generic/google-map";
import NavClose from "@/components/nav/close";
import AdCategoryBadge from "@/components/ad/category-badge";
import AlertModal from "@/components/generic/alert-modal";

import { gmapApi } from "gmap-vue";
import { AdCategory } from "@/mixins/ad-category";
import UserService from "@/services/user";

const CARD_VIEW = "CARD";
const LIST_VIEW = "LIST";
const SORT_DISTANCE_ASC = "DISTANCE-ASC";
const SORT_DISTANCE_DESC = "DISTANCE-DESC";
const SORT_DATE_ASC = "DATE-ASC";
const SORT_DATE_DESC = "DATE-DESC";

const defaultFilters = {
  category: null,
  professionalKitchenEquipment: [],
  deliveryTruckType: null,
  dayAvailability: [],
  eveningAvailability: [],
  refrigerated: null,
  canHaveDriver: null,
  canSharedRoad: null
};

export default {
  mixins: [AdCategory],
  components: {
    AdCard,
    AdFilters,
    Dropdown,
    SFormGoogleAutocomplete,
    GoogleMap,
    NavClose,
    AdCategoryBadge,
    AlertModal
  },
  computed: {
    isConnected() {
      return this.user && this.user.isConnected;
    },
    isAdmin() {
      return !this.me || this.me.type === this.$consts.enums.USER_TYPE_ADMIN;
    },
    createAlertModalHidden() {
      return (
        (this.isConnected && (!this.me || this.me.firstLoginModalClosed)) ||
        (!this.isConnected && this.localHideCreateAlertModal) ||
        (this.displayAdSnippet && this.view === CARD_VIEW) ||
        this.isAdmin
      );
    },
    displayAdSnippet() {
      return this.snippetAd !== null;
    },
    sortOptionsAvailable() {
      return this.sortOptions.filter((x) => {
        if (this.userPosition === null && [SORT_DISTANCE_ASC, SORT_DISTANCE_DESC].includes(x.value)) {
          return false;
        }
        return true;
      });
    },
    researchPosition() {
      return this.address || this.userPosition;
    },
    researchPositionLatLng() {
      if (!this.researchPosition) return;
      return new this.google.maps.LatLng(this.researchPosition.latitude, this.researchPosition.longitude);
    },
    google: gmapApi
  },
  methods: {
    mapMoved(latLng) {
      localStorage.setItem(LOCAL_STORAGE_MAP_LATLONG, JSON.stringify(latLng));
    },
    zoomChanged(zoomLevel) {
      localStorage.setItem(LOCAL_STORAGE_MAP_ZOOMLEVEL, zoomLevel);
    },
    resetMarkerIcon() {
      let marker = this.adMarkers.find((x) => this.snippetAd !== null && x.id === this.snippetAd.id);
      if (marker) {
        marker.icon = require("@/assets/icons/marker-red.svg");
      }
    },
    mapClicked() {
      this.resetMarkerIcon();
      this.snippetAd = null;
    },
    markerClicked(marker) {
      this.resetMarkerIcon();
      marker.icon = require("@/assets/icons/marker-yellow.svg");
      this.snippetAd = marker.ad;
    },
    validateCategory(category) {
      if (
        category === CATEGORY_PROFESSIONAL_KITCHEN ||
        category === CATEGORY_DELIVERY_TRUCK ||
        category === CATEGORY_STORAGE_SPACE ||
        category === CATEGORY_OTHER
      ) {
        return category;
      } else {
        return "";
      }
    },
    categoryLabel(category) {
      const options = this.categoryOptions.slice(1);
      const selectedOption = options.find((cat) => cat.value === category);
      if (!selectedOption) return this.$t("select.filter");
      return selectedOption.text;
    },
    sortLabel(sort) {
      const selectedOption = this.sortOptions.find((s) => s.value === sort);
      if (!selectedOption) return this.$t("select.sort");
      return selectedOption.text;
    },
    addPositionMarker(address) {
      this.calculateMarkerDistance();
      this.setQuery();
      if (address === null) {
        this.$refs.map.setPositionMarker(null);
        return;
      }
      this.$refs.map.setPositionMarker({
        coords: {
          latitude: address.latitude,
          longitude: address.longitude
        }
      });
    },
    updateAdViewStatus(toggleSearch) {
      // For accessibility purposes
      if (toggleSearch && this.displayAdvancedFilters) {
        this.$refs.adViewStatus.innerHTML = this.$t("sr.advanced-search-visible");
      } else if (this.view === LIST_VIEW) {
        if (toggleSearch) {
          this.$refs.adViewStatus.innerHTML = this.$t("sr.advanced-search-hidden") + this.$t("sr.ad-list-view");
        } else {
          this.$refs.adViewStatus.innerHTML = this.$t("sr.ad-list-view");
        }
      } else if (this.view === CARD_VIEW) {
        if (toggleSearch) {
          this.$refs.adViewStatus.innerHTML = this.$t("sr.advanced-search-hidden") + this.$t("sr.ad-card-view");
        } else {
          this.$refs.adViewStatus.innerHTML = this.$t("sr.ad-card-view");
        }
      }
    },
    showAdvancedFilters() {
      this.displayAdvancedFilters = true;

      this.$nextTick(() => {
        let navbarCloseBtn = this.$refs.filterNavbar.$el;
        navbarCloseBtn.querySelector(".nav-close").focus();
        this.updateAdViewStatus(true);
      });
    },
    hideAdvancedFilters() {
      this.displayAdvancedFilters = false;
      this.$refs.adViewStatus.innerHTML = this.$t("sr.advanced-search-visible");
      this.updateAdViewStatus(true);
    },
    reinitFilters() {
      this.filters = {
        ...defaultFilters
      };
    },
    async onFiltersUpdated() {
      this.hideAdvancedFilters();
    },
    applySorting() {
      let direction = 1;
      let sortingProp = null;
      switch (this.sort) {
        case SORT_DISTANCE_DESC:
          direction = -1;
          sortingProp = "distance";
          break;
        case SORT_DISTANCE_ASC:
          sortingProp = "distance";
          break;
        case SORT_DATE_DESC:
          direction = -1;
          sortingProp = "createdTimestamp";
          break;
        case SORT_DATE_ASC:
          sortingProp = "createdTimestamp";
          break;
      }
      if (sortingProp === null) return;
      this.adMarkers.sort((a, b) => {
        if (a[sortingProp] > b[sortingProp]) {
          return 1 * direction;
        } else if (a[sortingProp] < b[sortingProp]) {
          return -1 * direction;
        }
        return 0;
      });
      this.setQuery();
    },
    getUserPosition() {
      navigator.geolocation.getCurrentPosition((position) => {
        this.userPosition = position.coords;
        if (!this.address) this.calculateMarkerDistance();
      });
    },
    calculateMarkerDistance() {
      if (!this.researchPosition) return;
      for (const marker of this.adMarkers) {
        const markerLatLng = new this.google.maps.LatLng(marker.lat, marker.lng);
        marker.distance = this.google.maps.geometry.spherical.computeDistanceBetween(markerLatLng, this.researchPositionLatLng);
      }
      this.applySorting();
    },
    setView(view) {
      this.view = view;
      this.setQuery();
      this.updateAdViewStatus();
    },
    setQuery() {
      let query = {};
      this.filtersCount = 0;
      if (this.filters.category !== null) {
        query.category = this.filters.category;
        this.filtersCount++;
      }

      if (this.filters.professionalKitchenEquipment.length > 0) {
        query.professionalKitchenEquipment = this.filters.professionalKitchenEquipment;
        this.filtersCount++;
      }

      if (this.filters.deliveryTruckType !== null) {
        query.deliveryTruckType = this.filters.deliveryTruckType;
        this.filtersCount++;
      }

      if (this.filters.dayAvailability.length > 0) {
        query.dayAvailability = this.filters.dayAvailability;
        this.filtersCount++;
      }

      if (this.filters.eveningAvailability.length > 0) {
        query.eveningAvailability = this.filters.eveningAvailability;
        this.filtersCount++;
      }

      if (this.filters.refrigerated === true) {
        query.refrigerated = true;
        this.filtersCount++;
      }

      if (this.filters.canHaveDriver === true) {
        query.canHaveDriver = true;
        this.filtersCount++;
      }

      if (this.filters.canSharedRoad === true) {
        query.canSharedRoad = true;
        this.filtersCount++;
      }

      if (this.sort) {
        query.sort = this.sort;
      }

      if (this.view !== CARD_VIEW) {
        query.view = this.view;
      }

      if (this.address) {
        query.address = this.address.formatedAddress;
        query.lat = this.address.latitude;
        query.lon = this.address.longitude;
      }

      this.$router
        .replace({
          name: URL_LIST_AD,
          query
        })
        .catch(() => {});
    },
    hideCreateAlertModal() {
      if (this.isConnected) {
        UserService.updateFirstLoginModalClosed(true);
      } else {
        this.$cookies.set("hideCreateAlertModal", true);
        this.localHideCreateAlertModal = true;
      }
    }
  },
  data() {
    let latLng = JSON.parse(localStorage.getItem(LOCAL_STORAGE_MAP_LATLONG));
    let zoomLevel = Number(localStorage.getItem(LOCAL_STORAGE_MAP_ZOOMLEVEL));

    if (latLng === null) {
      latLng = { lat: null, lng: null };
    }

    let filters = {
      ...defaultFilters
    };

    for (var filter of Object.keys(filters)) {
      if (this.$router.currentRoute.query.hasOwnProperty(filter)) {
        filters[filter] = this.$router.currentRoute.query[filter];
      }
    }

    return {
      displayAdvancedFilters: false,
      localHideCreateAlertModal: this.$cookies.isKey("hideCreateAlertModal"),
      snippetAd: null,
      adMarkers: [],
      initialLatitude: latLng.lat,
      initialLongitude: latLng.lng,
      initialZoomLevel: zoomLevel,
      address: null,
      userPosition: null,
      filters,
      filtersCount: 0,
      sort: this.$router.currentRoute.query.sort || null,
      sortOptions: [
        { value: SORT_DISTANCE_ASC, text: this.$t("select.distance-asc") },
        { value: SORT_DISTANCE_DESC, text: this.$t("select.distance-desc") },
        { value: SORT_DATE_ASC, text: this.$t("select.date-asc") },
        { value: SORT_DATE_DESC, text: this.$t("select.date-desc") }
      ],
      view: this.$router.currentRoute.query.view == LIST_VIEW ? LIST_VIEW : CARD_VIEW,
      categoryOptions: [
        { value: null, text: this.$t("select.all-equipment") },
        {
          value: CATEGORY_PROFESSIONAL_KITCHEN,
          text: this.$t("select.category-professional-kitchen")
        },
        {
          value: CATEGORY_DELIVERY_TRUCK,
          text: this.$t("select.category-delivery-truck")
        },
        { value: CATEGORY_STORAGE_SPACE, text: this.$t("select.category-storage-space") },
        { value: CATEGORY_OTHER, text: this.$t("select.category-other") }
      ],
      CARD_VIEW,
      LIST_VIEW
    };
  },
  mounted() {
    this.getUserPosition();
    if (
      this.$router.currentRoute.query.address &&
      this.$router.currentRoute.query.lat > 0 &&
      this.$router.currentRoute.query.lon < 0
    ) {
      this.address = {
        formatedAddress: this.$router.currentRoute.query.address,
        latitude: this.$router.currentRoute.query.lat,
        longitude: this.$router.currentRoute.query.lon
      };
    }
  },
  watch: {
    "filters.category"(value) {
      if (value !== CATEGORY_PROFESSIONAL_KITCHEN) {
        this.filters.professionalKitchenEquipment = [];
      }
      if (value !== CATEGORY_DELIVERY_TRUCK) {
        this.filters.deliveryTruckType = null;
        this.filters.refrigerated = false;
        this.filters.canHaveDriver = false;
        this.filters.canSharedRoad = false;
      }
    }
  },
  apollo: {
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
    },
    ads: {
      query() {
        return this.$options.query.Ads;
      },
      variables() {
        return {
          category: this.filters.category,
          professionalKitchenEquipment: this.filters.professionalKitchenEquipment,
          deliveryTruckType: this.filters.deliveryTruckType,
          dayAvailability: this.filters.dayAvailability,
          eveningAvailability: this.filters.eveningAvailability,
          refrigerated: this.filters.refrigerated,
          canHaveDriver: this.filters.canHaveDriver,
          canSharedRoad: this.filters.canSharedRoad,
          language: CONTENT_LANG_FR
        };
      },
      result({ data }) {
        this.filtersCount = 0;
        this.snippetAd = null;
        if (data) {
          this.adMarkers = data.ads
            .filter((x) => x.isPublish)
            .map((x) => {
              let pos = randomPosition(x.address.latitude, x.address.longitude);
              return {
                id: x.id,
                ad: x,
                key: x.address.id,
                lat: pos.lat,
                lng: pos.lng,
                distance: null,
                createdTimestamp: new Date(x.createdAtUTC).getTime(),
                icon: x.isAdminOnly ? require("@/assets/icons/marker-green.svg") : require("@/assets/icons/marker-red.svg")
              };
            });
          this.$gmapApiPromiseLazy().then(() => {
            this.calculateMarkerDistance();
          });

          this.setQuery();
        }
      }
    }
  }
};
</script>

<style lang="scss">
.equipment-list {
  background-color: $green-lighter;
  position: relative;
  width: 100%;
  z-index: 0;

  &--view-list {
    &:after {
      content: "";
      position: absolute;
      bottom: 0;
      left: 0;
      right: 0;
      width: 100%;
      background: rgb(255, 255, 255);
      background: linear-gradient(0deg, rgba(255, 255, 255, 1) 0%, rgba(255, 255, 255, 0) 100%);
      height: 100px;
    }
  }

  &__header {
    background-color: $white;

    @include media-breakpoint-up(lg) {
      display: flex;
      flex-direction: row-reverse;
      justify-content: space-between;
    }
  }

  &__header-top {
    display: flex;
    justify-content: space-between;
    align-items: center;

    @include media-breakpoint-up(md) {
      justify-content: flex-end;
    }
  }

  &__header-bottom {
    display: flex;
    align-items: center;
    padding: $spacer / 4 0;
  }

  &__header-bottom-btns {
    display: flex;
    align-items: center;
  }

  &__toggle-view {
    background-color: $green-lighter;
    border-color: $green-lighter;
    color: $gray-900;
    margin-right: 4px;

    @include media-breakpoint-down(lg) {
      &:hover {
        transform: translate(0, 0);
        border-color: white;
        box-shadow: none;
        background-color: $green;
      }
    }

    &--active {
      background-color: $red-darker;
      border-color: $red-darker;
      color: $white;

      &:hover {
        transform: translate(0, 0);
        box-shadow: none;
        border: 1px solid $red-darker;
        background-color: $red-darker;
        cursor: default !important;
      }

      @include media-breakpoint-up(lg) {
        background-color: $red-darker;
        border-color: $red-darker;

        &:hover {
          background-color: $red-darker;
          border-color: $red-darker;
        }
      }
    }
  }

  &__reset-filters-btn {
    color: $white;
    font-size: $small-font-size;
    margin-left: auto;
    position: relative;
    left: 12px;

    @include media-breakpoint-up(lg) {
      color: $gray-900;
      white-space: nowrap;
      font-size: $font-size-base;
      left: 0;
    }
  }

  &__google-address {
    @include media-breakpoint-up(sm) {
      min-width: 300px;
    }

    @include media-breakpoint-up(md) {
      margin-right: auto;
    }

    @include media-breakpoint-up(lg) {
      margin-right: 0;
    }
  }

  &__filters-dropdown {
    display: none;

    @include media-breakpoint-up(md) {
      display: block;
      padding-left: 12px;

      & > .dropdown-toggle {
        min-height: 38px;
      }
    }
  }

  &__filters-btn {
    display: flex;
    align-items: center;
    min-height: 38px;
    margin-left: 12px;
  }

  &__filters-btn-badge {
    background-color: $green;
    border-radius: 50%;
    width: 18px;
    height: 18px;
    color: white;
    display: flex;
    justify-content: center;
    align-items: center;
    font-size: $small-font-size;
    margin-left: 8px;
  }

  &__list-header {
    display: flex;
    align-items: center;

    @include media-breakpoint-up(sm) {
      justify-content: flex-end;
    }
  }

  &__list-header-section {
    width: calc(50% - 8px);

    @include media-breakpoint-up(sm) {
      width: auto;
    }
  }

  &__list-view-row {
    margin-bottom: 100px;

    @include media-breakpoint-up(md) {
      display: flex;
      flex-wrap: wrap;
      margin: 0 -12px;
    }
  }

  &__list-view-col {
    margin: 0 0 24px;

    @include media-breakpoint-up(md) {
      width: calc(50% - 24px);
      margin: 0 12px 24px;
    }

    @include media-breakpoint-up(xl) {
      width: calc(33.33% - 24px);
    }
  }

  &__list-view-card {
    height: 100%;
  }

  &__scroll {
    height: calc(100vh - #{$nav-height} - #{$footer-height} - #{$header-top-height} - #{$header-bottom-height});
    overflow-y: scroll;

    @include media-breakpoint-up(lg) {
      height: calc(100vh - #{$nav-height} - #{$footer-height} - #{$header-top-height});
    }
  }

  &__bottom {
    position: absolute;
    bottom: $spacer;
    z-index: 1;
  }

  &__modal {
    &-body {
      display: flex;
      align-self: stretch;
      flex: 1 1 auto;
      flex-direction: column;
      background-color: $white;
      height: calc(100vh - #{$nav-height});
    }
  }
}
</style>
