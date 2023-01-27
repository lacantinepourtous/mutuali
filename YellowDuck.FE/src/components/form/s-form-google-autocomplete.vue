<template>
  <s-field
    class="position-relative"
    :id="id"
    :inputId="inputId"
    :label-sr-only="labelSrOnly"
    :margin="margin"
    :name="name"
    :label="label"
    :description="description"
    :rules="rules"
    v-slot="{ sState }"
  >
    <s-validation-provider-value v-model="computedValue"></s-validation-provider-value>
    <gmap-autocomplete
      ref="address"
      v-show="edit"
      :id="inputId"
      :class="`form-control ${sState !== '' && sState === false ? 'is-invalid' : ''}`"
      :placeholder="computedPlaceholder"
      @place_changed="placeChanged"
      @change="inputChange"
      @focus="inputFocus"
      @blur="inputBlur"
      :options="autocompleteOptions"
    >
    </gmap-autocomplete>
    <b-button
      v-if="showSearchButton && edit"
      class="search-address-btn form-control"
      variant="outline-secondary"
      :aria-label="$t('btn.search-address')"
      @click="editAddress()"
    >
      <b-icon icon="search" aria-hidden="true"></b-icon>
    </b-button>
    <b-button
      v-if="!edit"
      class="form-control d-flex align-items-center justify-content-between"
      variant="outline-secondary"
      :aria-label="$t('btn.edit-ad-address')"
      @click="editAddress"
    >
      <span class="text-truncate">{{ address }}</span>
      <b-icon icon="pencil-fill" aria-hidden="true"></b-icon>
    </b-button>
  </s-field>
</template>

<script>
import SField from "@/components/form/s-field";
import SValidationProviderValue from "@/components/form/s-validation-provider-value";

export default {
  props: {
    id: String,
    label: String,
    labelSrOnly: Boolean,
    name: String,
    rules: {
      type: [String, Object]
    },
    value: {
      type: [Object],
      default() {
        return null;
      }
    },
    placeholder: String,
    description: String,
    margin: String,
    showSearchButton: Boolean
  },
  components: {
    SField,
    SValidationProviderValue
  },
  data() {
    return {
      autocompleteOptions: {
        componentRestrictions: {
          country: "ca"
        },
        fields: [
          "address_components",
          "adr_address",
          "alt_id",
          "formatted_address",
          "geometry",
          "icon",
          "id",
          "name",
          "photo",
          "place_id",
          "scope",
          "type",
          "url",
          "vicinity"
        ]
      },
      displayPlaceholder: true
    };
  },
  methods: {
    inputChange: function (e) {
      // "inputChange" is always triggered before "placeChanged"
      this.computedValue = null;
      e.target.value = "";
    },
    placeChanged: function (place) {
      if (!place || !place.geometry) {
        this.computedValue = null;
        return;
      }

      if (place.address_components === undefined) {
        return;
      }

      this.computedValue = this.formatResult(place);
    },
    inputBlur: function (e) {
      this.displayPlaceholder = true;
    },
    inputFocus: function (e) {
      this.biasAutocompleteLocation();
    },
    editAddress: function () {
      this.computedValue = null;
      this.displayPlaceholder = false;
      setTimeout(() => this.$refs.address.$refs.input.focus(), 16);
    },
    // Bias autocomplete location from VueGoogleAutocomplete
    biasAutocompleteLocation: function () {
      const _this = this;
      this.$gmapApiPromiseLazy().then(function () {
        navigator.geolocation.getCurrentPosition((position) => {
          // eslint-disable-next-line
          let circle = new google.maps.Circle({
            center: {
              lat: position.coords.latitude,
              lng: position.coords.longitude
            },
            radius: position.coords.accuracy
          });
          _this.$refs.address.$autocomplete.setBounds(circle.getBounds());
        });
      });
    },
    // Format result from VueGoogleAutocomplete
    formatResult: function (place) {
      const ADDRESS_COMPONENTS = {
        subpremise: "short_name",
        street_number: "short_name",
        route: "long_name",
        locality: "long_name",
        administrative_area_level_1: "short_name",
        administrative_area_level_2: "long_name",
        country: "long_name",
        postal_code: "short_name"
      };

      let returnData = {};
      for (let i = 0; i < place.address_components.length; i++) {
        let addressType = place.address_components[i].types[0];

        if (ADDRESS_COMPONENTS[addressType]) {
          let val = place.address_components[i][ADDRESS_COMPONENTS[addressType]];
          returnData[addressType] = val;
        }
      }

      returnData["latitude"] = place.geometry.location.lat();
      returnData["longitude"] = place.geometry.location.lng();

      return returnData;
    }
  },
  computed: {
    computedValue: {
      get() {
        return this.value;
      },
      set(val) {
        this.$emit("input", val);
      }
    },
    computedPlaceholder: function () {
      return this.displayPlaceholder ? this.placeholder : "";
    },
    address: function () {
      var firstSection = [];
      var lastSection = [];
      if (this.value.streetNumber || this.value.street_number)
        firstSection.push(this.value.streetNumber || this.value.street_number);
      if (this.value.route) firstSection.push(this.value.route);
      if (this.value.locality || this.value.administrative_area_level_2)
        lastSection.push(this.value.locality || this.value.administrative_area_level_2);
      if (lastSection.length === 0 && this.value.administrative_area_level_2)
        lastSection.push(this.value.administrative_area_level_2);
      if (lastSection.length === 0 && this.value.administrative_area_level_1)
        lastSection.push(this.value.administrative_area_level_1);

      var address = firstSection.join(" ");
      if (address.trim() !== "" && lastSection.length > 0) address += ", ";
      address += lastSection.join(" ");

      return address.trim();
    },
    edit: function () {
      return this.value === null;
    },
    inputId() {
      return `input-${this.name}`;
    }
  }
};
</script>

<style lang="scss">
.pac-container {
  background-color: $list-group-bg;
  box-sizing: content-box;
  border: $list-group-border-width solid $list-group-border-color;
  border-radius: $list-group-border-radius;
  box-shadow: none;
  font-family: $font-family-base;
  font-size: $input-font-size;
  line-height: $input-line-height;
  padding: $spacer * 0.5 0;

  .pac-item {
    border: none;
    color: $input-placeholder-color;
    font-size: $small-font-size;
    line-height: $input-line-height * 1.25;
    padding: $spacer * 0.25 $input-padding-x;

    &:hover,
    &:focus,
    &:active {
      background-color: $list-group-hover-bg;
    }
  }

  .pac-icon {
    background: none;
    margin: 2px 0 0 0;
    width: calc(#{$input-padding-x} + 4px);

    &-marker {
      background: url("~@/assets/icons/marker-green.svg") no-repeat 0% 50%;
      background-size: contain;
    }
  }

  .pac-item-query {
    color: $input-plaintext-color;
    font-size: $input-font-size;
    line-height: $input-line-height;
  }

  .pac-matched {
    color: $dark;
  }

  &::after {
    background-position: calc(100% - #{$input-padding-x}) 50%;
    margin-top: $spacer * 0.5;
  }
}

.search-address-btn {
  position: absolute;
  top: 0px;
  bottom: 0px;
  right: 0px;
  background-color: white;
  width: 45px;
  border-top-left-radius: 0;
  border-bottom-left-radius: 0;
}
</style>
