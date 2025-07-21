<template>
  <div class="search-filters">
    <div class="section section--lg my-4 my-md-5">
      <h2 class="mb-3">{{ $t("label.advancedSearch") }}</h2>
      <p class="label mb-1">{{ $t('label-whichCategory') }}</p>
      <ad-filters-category class="search-filters__select" id="adCategoryFilterAdvancedSearch" :category="filters.category" @update:category="(value) => updateFilters('category', value)" />
    </div>
    <hr />

    <div class="section section--lg my-4 my-md-5">
      <div class="search-filters__row">
        <div v-if="hasTwoCols" class="search-filters__col rm-child-margin">
          <s-form-checkbox-group
            v-if="filters.category === CATEGORY_PROFESSIONAL_KITCHEN"
            :value="filters.professionalKitchenEquipment"
            @input="(value) => updateFilters('professionalKitchenEquipment', value)"
            id="professionalKitchenEquipment"
            :label="$t('label.ad-professionalKitchenEquipment')"
            label-class="label"
            name="professionalKitchenEquipment"
            :options="professionalKitchenEquipmentOptions"
          />
          <template v-if="filters.category === CATEGORY_DELIVERY_TRUCK">
            <s-form-select
              :value="filters.deliveryTruckType"
              @input="(value) => updateFilters('deliveryTruckType', value)"
              id="deliveryTruckType"
              :label="$t('label.ad-deliveryTruckType')"
              name="deliveryTruckType"
              :options="deliveryTruckTypesOptionsWithAll"
            />
            <s-form-checkbox
              :value="filters.refrigerated"
              @input="(value) => updateFilters('refrigerated', value)"
              id="refrigerated"
              :label="$t('label.ad-refrigerated')"
              name="refrigerated"
            />
            <s-form-checkbox
              :value="filters.canSharedRoad"
              @input="(value) => updateFilters('canSharedRoad', value)"
              id="canSharedRoad"
              :label="$t('label.ad-canSharedRoad')"
              name="canSharedRoad"
            />
            <s-form-checkbox
              :value="filters.canHaveDriver"
              @input="(value) => updateFilters('canHaveDriver', value)"
              id="canHaveDriver"
              :label="$t('label.ad-canHaveDriver')"
              name="canHaveDriver"
            />
          </template>
        </div>

        <div class="search-filters__col">
          <s-form-availability
            v-if="filters.category !== CATEGORY_SUBCONTRACTING"
            id="availability"
            :legend="$t('label.availability')"
            legend-class="label mb-0"
            :day-availability="filters.dayAvailability"
            :evening-availability="filters.eveningAvailability"
            @update:dayAvailability="(v) => updateFilters('dayAvailability', v)"
            @update:eveningAvailability="(v) => updateFilters('eveningAvailability', v)"
          />
        </div>
      </div>
    </div>
    <hr />

    <div class="section section--lg my-6">
      <div class="fab-container__fab">
        <b-button @click="confirm()" type="submit" variant="admin" size="lg" block :aria-label="$t('sr.edit')">
          {{ $t("label.search") }}
        </b-button>
        <b-button @click="reinit()" type="submit" variant="outline-primary" size="lg" block :aria-label="$t('sr.edit')">
          {{ $t("label.reinit") }}
        </b-button>
      </div>
    </div>
  </div>
</template>

<script>
import {
  CATEGORY_PROFESSIONAL_KITCHEN,
  CATEGORY_DELIVERY_TRUCK,
  CATEGORY_SUBCONTRACTING
} from "@/consts/categories";

import SFormCheckboxGroup from "@/components/form/s-form-checkbox-group";
import SFormAvailability from "@/components/form/s-form-availability";
import SFormCheckbox from "@/components/form/s-form-checkbox";
import SFormSelect from "@/components/form/s-form-select";
import AdFiltersCategory from "@/components/ad/filters-category";

import { AdCategory } from "@/mixins/ad-category";
import { ProfessionalKitchenEquipment } from "@/mixins/professional-kitchen-equipment";
import { AdDeliveryTruckType } from "@/mixins/ad-delivery-truck-type";
import { AvailabilityWeekday } from "@/mixins/availability-weekday";

const defaultFilterValue = {
  category: null,
  professionalKitchenEquipment: [],
  deliveryTruckType: null,
  dayAvailability: [],
  eveningAvailability: [],
  refrigerated: false,
  canHaveDriver: false,
  canSharedRoad: false
};

export default {
  mixins: [AdCategory, ProfessionalKitchenEquipment, AvailabilityWeekday, AdDeliveryTruckType],
  components: { SFormCheckboxGroup, SFormAvailability, SFormCheckbox, SFormSelect, AdFiltersCategory },
  props: {
    value: {
      type: Object,
      default() {
        return { ...defaultFilterValue };
      }
    }
  },
  methods: {
    updateFilters(filter, value) {
      this.filters[filter] = value;
    },
    confirm() {
      this.$emit("input", this.filters);
    },
    reinit() {
      this.filters = { ...defaultFilterValue };
      this.$emit("input", this.filters);
    }
  },
  data() {
    return {
      filters: { ...this.value },
      professionalKitchenEquipment: [],
      CATEGORY_PROFESSIONAL_KITCHEN,
      CATEGORY_DELIVERY_TRUCK,
      CATEGORY_SUBCONTRACTING
    };
  },
  computed: {
    deliveryTruckTypesOptionsWithAll() {
      let options = [{ value: null, text: this.$t("select.all-equipment") }];
      if (Array.isArray(this.deliveryTruckTypesOptions)) {
        options = [...options, ...this.deliveryTruckTypesOptions];
      }
      return options;
    },
    hasTwoCols() {
      return this.filters.category === CATEGORY_PROFESSIONAL_KITCHEN || this.filters.category === CATEGORY_DELIVERY_TRUCK;
    }
  }
};
</script>
<style lang="scss">
.search-filters {
  &__row {
    @include media-breakpoint-up(lg) {
      display: flex;
      margin: 0 -32px -24px;
    }
  }

  &__col {
    margin-bottom: 24px;

    @include media-breakpoint-up(lg) {
      width: calc(50% - 64px);
      margin: 0 32px 24px;
    }
  }

  &__select {
    width: 100%;
  }

  .label {
    font-size: 20px;
    font-weight: 500;
    display: block;
    padding-top: 0;
    padding-bottom: calc(0.375rem + 1px);
  }
}
</style>
