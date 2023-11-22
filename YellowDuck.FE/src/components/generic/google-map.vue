<template>
  <GmapMap
    ref="mapRef"
    :options="options"
    :center="center"
    :zoom="computedZoom"
    class="s-google-map"
    @click="mapClick"
    @center_changed="centerChanged"
    @zoom_changed="zoomChanged"
  >
    <GmapMarker
      v-for="m in allMarkers"
      :key="m.key"
      :position="{ lat: m.lat, lng: m.lng }"
      :clickable="m.clickable || markerClickable"
      :draggable="m.draggable || markerDraggable"
      :icon="m.icon || require('@/assets/icons/marker-green.svg')"
      @click="markerClick(m)"
    ></GmapMarker>
  </GmapMap>
</template>

<script>
import { gmapApi } from "gmap-vue";
import NotificationService from "@/services/notification";

export default {
  props: {
    markers: {
      type: Array,
      required: true
    },
    markerClickable: {
      type: Boolean,
      default: false
    },
    markerDraggable: {
      type: Boolean,
      default: false
    },
    zoom: {
      type: Number
    },
    zoomControl: {
      type: Boolean,
      default: true
    },
    geolocationControl: {
      type: Boolean,
      default: true
    },
    mapTypeControl: {
      type: Boolean,
      default: false
    },
    scaleControl: {
      type: Boolean,
      default: false
    },
    streetViewControl: {
      type: Boolean,
      default: false
    },
    rotateControl: {
      type: Boolean,
      default: false
    },
    fullscreenControl: {
      type: Boolean,
      default: false
    },
    disableDefaultUi: {
      type: Boolean,
      default: false
    },
    latitude: {
      type: Number
    },
    longitude: {
      type: Number
    }
  },
  data() {
    return {
      geolocLatitude: null,
      geolocLongitude: null,
      preventEmitEvents: false,
      positionMarker: null
    };
  },
  mounted() {
    this.$refs.mapRef.$mapPromise.then((map) => {
      map.mapTypes.set(
        "styled_map",
        new this.google.maps.StyledMapType(require("@/assets/gmap/style.json"), { name: "Styled Map" })
      );

      this.google.maps.event.addListenerOnce(map, "idle", () => {
        this.fitBoundsIfNeeded();
      });
    });

    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(this.setCenterPosition);
    }

    if (this.geolocationControl) {
      this.addGeolocationControl();
    }
  },
  computed: {
    allMarkers: function() {
      if (this.positionMarker !== null) {
        return [...this.markers, this.positionMarker];
      }
      return this.markers;
    },
    options: function() {
      return {
        zoomControl: this.zoomControl,
        zoomControlOptions: {
          position: this.google ? this.google.maps.ControlPosition.TOP_RIGHT : null
        },
        mapTypeControl: this.mapTypeControl,
        scaleControl: this.scaleControl,
        streetViewControl: this.streetViewControl,
        rotateControl: this.rotateControl,
        fullscreenControl: this.fullscreenControl,
        disableDefaultUi: this.disableDefaultUi,
        mapTypeId: "styled_map"
      };
    },
    google: gmapApi,
    center: function() {
      if (this.latitude && this.longitude) {
        return { lat: this.latitude, lng: this.longitude };
      } else if (this.geolocLatitude && this.geolocLongitude) {
        return { lat: this.geolocLatitude, lng: this.geolocLongitude };
      } else if (this.markers.length === 0) {
        return { lat: 45.51949093839918, lng: -73.63942886979196 };
      }

      let latitude = this.markers.reduce((accumulator, x) => accumulator + parseFloat(x.lat), 0);
      let longitude = this.markers.reduce((accumulator, x) => accumulator + parseFloat(x.lng), 0);

      return { lat: latitude / this.markers.length, lng: longitude / this.markers.length };
    },
    computedZoom: function() {
      if (this.zoom) {
        return this.zoom;
      }
      // If no marker, display a zoom level of 10 since it's the default geoloc
      else if (this.markers.length === 0) {
        return 10;
      }
      // If only one marker, display a map with a low level of zoom
      if (this.markers.length === 1) {
        return 15;
      }
      // If we use the geoloc lat & lng, we display a mid level of zoom
      if (!this.latitude && !this.longitude && this.geolocLatitude && this.geolocLongitude) {
        return 10;
      }
      // Else we display a high level of zoom since it's not perfectly center on current user position
      return 7;
    }
  },
  methods: {
    centerChanged(latLng) {
      if (this.preventEmitEvents) return;
      this.$emit("mapMoved", { lat: latLng.lat(), lng: latLng.lng() });
    },
    zoomChanged(zoomLevel) {
      if (this.preventEmitEvents) return;
      this.$emit("zoomChanged", zoomLevel);
    },
    mapClick(GmapMap) {
      if (this.preventEmitEvents) return;
      this.$emit("mapClicked");
    },
    markerClick(marker) {
      if (this.preventEmitEvents) return;
      this.$emit("markerClicked", marker);
    },
    async setCenterPosition(position, forceCenter) {
      this.geolocLatitude = position.coords.latitude;
      this.geolocLongitude = position.coords.longitude;
      if (forceCenter) {
        var map = await this.$refs.mapRef.$mapPromise;
        map.setCenter(new this.google.maps.LatLng(position.coords.latitude, position.coords.longitude));
      } else if (!this.latitude && !this.longitude) {
        // Wait nextTick to let the new center propagate
        this.$nextTick(() => {
          this.fitBoundsIfNeeded();
        });
      }
    },
    async fitBoundsIfNeeded() {
      var map = await this.$refs.mapRef.$mapPromise;
      if (!map.getBounds() || this.markers.length === 0) return;
      const mapBounds = map.getBounds();

      // If no markers are visible in the current zoom level
      if (
        !this.markers.some((m) => {
          return mapBounds.contains({ lat: m.lat, lng: m.lng });
        })
      ) {
        var bounds = this.generateBoundsWithMarkers();
        bounds.extend(map.getCenter());
        this.preventEmitEvents = true;
        map.fitBounds(bounds);
        this.preventEmitEvents = false;
      }
    },
    generateBoundsWithMarkers: function() {
      if (!this.google) return;
      var bounds = new this.google.maps.LatLngBounds();
      for (var marker of this.markers) {
        bounds.extend({ lat: marker.lat, lng: marker.lng });
      }

      return bounds;
    },
    async addGeolocationControl() {
      if (!navigator.geolocation) return;
      var map = await this.$refs.mapRef.$mapPromise;
      const geolocationControlButton = document.createElement("button");
      const geolocationIcon = document.createElement("img");
      geolocationIcon.src = require("@/assets/gmap/current-position-icon.svg");
      geolocationIcon.alt = this.$t("geolocation.btn");
      geolocationControlButton.appendChild(geolocationIcon);
      geolocationControlButton.classList.add("gmap-custom-control");
      map.controls[this.google.maps.ControlPosition.RIGHT_TOP].push(geolocationControlButton);
      geolocationControlButton.addEventListener("click", this.setCenterOnUserGeolocation);
    },
    setCenterOnUserGeolocation() {
      if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
          (position) => {
            this.setPositionMarker(position);
          },
          () => NotificationService.showError(this.$t("error.geolocation"))
        );
      }
    },
    setPositionMarker(position) {
      if (position === null) {
        this.positionMarker = null;
        return;
      }
      this.setCenterPosition(position, true);
      this.positionMarker = {
        id: "positionMarker",
        lat: position.coords.latitude,
        lng: position.coords.longitude,
        icon: {
          path: this.google.maps.SymbolPath.CIRCLE,
          scale: 8,
          fillOpacity: 1,
          strokeWeight: 2,
          fillColor: "#5384ED",
          strokeColor: "#ffffff"
        }
      };
    }
  }
};
</script>

<style lang="scss">
.s-google-map {
  width: 100%;
  height: 100%;
  height: -webkit-calc(100% - 70px);
  height: -moz-calc(100% - 70px);
  height: calc(100% - 70px);
}

.gmap-custom-control {
  margin: 10px;
  padding: 5px;
  border: none;
  width: 40px;
  height: 40px;
  background: white;
  box-shadow: rgba(0, 0, 0, 0.3) 0px 1px 4px -1px;
  border-radius: 2px;
}

.gmap-custom-control:hover img {
  filter: brightness(0.77);
}
</style>
