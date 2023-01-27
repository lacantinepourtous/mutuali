<template>
  <div class="equipment-list" v-if="ads !== undefined">
    <div class="section">
      <b-container fluid>
        <b-row class="my-2">
          <b-col>
            <s-form-google-autocomplete
              v-if="false"
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
          </b-col>
          <b-col class="equipment-list__dropdown">
            <dropdown
              v-model="category"
              id="adCategory"
              :label="categoryLabel(category)"
              :options="categoryOptions"
              right
              variant="outline-secondary"
            />
          </b-col>
        </b-row>
      </b-container>
    </div>
    <hr class="my-0" />
    <google-map
      ref="map"
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
    <div class="equipment-list__bottom section section--md">
      <ad-card v-if="displayAdSnippet" :id="snippetAdId" />
      <b-button
        v-else-if="isConnected && !isAdmin"
        variant="primary"
        size="lg"
        block
        :to="{ name: $consts.urls.URL_CREATE_AD }"
        >{{ $t("nav.create-ad") }}</b-button
      >
    </div>
  </div>
</template>

<graphql>
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

query Ads($category: AdCategory = null) {
  ads(category: $category) {
    id
    isPublish
    address {
      id
      latitude
      longitude
    }
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

import AdCard from "@/components/ad/card";
import Dropdown from "@/components/generic/dropdown";
import SFormGoogleAutocomplete from "@/components/form/s-form-google-autocomplete";
import GoogleMap from "@/components/generic/google-map";

import { AdCategory } from "@/mixins/ad-category";

export default {
  mixins: [AdCategory],
  components: { AdCard, Dropdown, SFormGoogleAutocomplete, GoogleMap },
  computed: {
    isConnected: function() {
      return this.user && this.user.isConnected;
    },
    isAdmin: function() {
      return !this.me || this.me.type === this.$consts.enums.USER_TYPE_ADMIN;
    },
    displayAdSnippet: function() {
      return this.snippetAdId !== "";
    }
  },
  methods: {
    mapMoved(latLng) {
      localStorage.setItem(LOCAL_STORAGE_MAP_LATLONG, JSON.stringify(latLng));
    },
    zoomChanged(zoomLevel) {
      localStorage.setItem(LOCAL_STORAGE_MAP_ZOOMLEVEL, zoomLevel);
    },
    resetMarkerIcon() {
      let marker = this.adMarkers.find((x) => x.id === this.snippetAdId);
      if (marker) {
        marker.icon = require("@/assets/icons/marker-green.svg");
      }
    },
    mapClicked() {
      this.resetMarkerIcon();
      this.snippetAdId = "";
    },
    markerClicked(marker) {
      this.resetMarkerIcon();
      marker.icon = require("@/assets/icons/marker-yellow.svg");
      this.snippetAdId = marker.id;
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
    addPositionMarker(address) {
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
    }
  },
  data() {
    let latLng = JSON.parse(localStorage.getItem(LOCAL_STORAGE_MAP_LATLONG));
    let zoomLevel = Number(localStorage.getItem(LOCAL_STORAGE_MAP_ZOOMLEVEL));

    if (latLng === null) {
      latLng = { lat: null, lng: null };
    }

    let category = this.$router.currentRoute.query.category;
    if (category === undefined) {
      category = "";
    } else {
      category = this.validateCategory(category);
    }

    return {
      snippetAdId: "",
      adMarkers: [],
      initialLatitude: latLng.lat,
      initialLongitude: latLng.lng,
      initialZoomLevel: zoomLevel,
      address: null,
      category: category,
      categoryOptions: [
        { value: "", text: this.$t("select.all-equipment") },
        { value: CATEGORY_PROFESSIONAL_KITCHEN, text: this.$t("select.category-professional-kitchen") },
        { value: CATEGORY_DELIVERY_TRUCK, text: this.$t("select.category-delivery-truck") },
        { value: CATEGORY_STORAGE_SPACE, text: this.$t("select.category-storage-space") },
        { value: CATEGORY_OTHER, text: this.$t("select.category-other") }
      ]
    };
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
        if (this.category === "") {
          return {};
        } else {
          return {
            category: this.category
          };
        }
      },
      result({ data }) {
        if (data) {
          this.adMarkers = data.ads
            .filter((x) => x.isPublish)
            .map((x) => {
              let pos = randomPosition(x.address.latitude, x.address.longitude);
              return {
                id: x.id,
                key: x.address.id,
                lat: pos.lat,
                lng: pos.lng,
                icon: require("@/assets/icons/marker-green.svg")
              };
            });

          let query = {};

          if (this.category !== "") {
            query.category = this.category;
          }

          this.$router
            .replace({
              name: URL_LIST_AD,
              query
            })
            .catch(() => {});
        }
      }
    }
  }
};
</script>

<style lang="scss">
.equipment-list {
  position: relative;
  width: 100%;
  z-index: 0;

  &__dropdown {
    display: flex;
    justify-content: flex-end;
  }

  &__bottom {
    position: absolute;
    bottom: $spacer;
    z-index: 1;
  }
}
</style>
