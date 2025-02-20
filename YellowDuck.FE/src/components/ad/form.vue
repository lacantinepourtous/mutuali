<template>
  <s-form @submit="submitForm" class="ad-form">
    <div class="section section--md section--padding-x section--border-bottom my-4 pb-5 rm-child-margin">
      <h2 class="my-4">{{ $t("section-title.general-info") }}</h2>
      <s-form-input
        v-model="form.title"
        id="title"
        :label="$t('label.ad-title')"
        name="title"
        rules="required"
        :placeholder="$t('placeholder.ad-title')"
        required
      />
      <s-form-select
        v-model="form.category"
        id="category"
        :label="$t('label.ad-category')"
        name="category"
        rules="required"
        :placeholder="$t('placeholder.ad-category')"
        :options="categoryOptions"
        required
      />

      <s-form-hidden
        :value="hasAtLeastOneTransactionType ? 1 : 0"
        id="hasAtLeastOneTransactionType"
        name="hasAtLeastOneTransactionType"
      />

      <!-- Rent Section -->
      <s-form-checkbox
        v-model="form.isAvailableForRent"
        id="isAvailableForRent"
        :label="$t('label.isAvailableForRent')"
        name="isAvailableForRent"
        rules="atLeastOneChecked:@hasAtLeastOneTransactionType"
      />
      <div v-if="form.isAvailableForRent" class="sub-section">
        <s-form-input
          v-if="!form.rentPriceToBeDetermined"
          v-model="form.rentPrice"
          id="rentPrice"
          type="number"
          :label="$t('label.ad-rent-price')"
          name="rentPrice"
          rules="requiredNumeric"
          :placeholder="$t('placeholder.ad-rent-price')"
          required
          append="$"
        />
        <s-form-checkbox
          v-model="form.rentPriceToBeDetermined"
          id="rentPriceToBeDetermined"
          :label="$t('label.ad-rentPriceToBeDetermined')"
          name="rentPriceToBeDetermined"
        />
        <s-form-select
          v-if="form.rentPriceToBeDetermined"
          v-model="form.rentPriceRange"
          id="rentPriceRange"
          :label="$t('label.ad-rent-price-range')"
          name="rentPriceRange"
          rules="required"
          :placeholder="$t('placeholder.ad-rent-price-range')"
          :options="rentalPriceRangeOptions"
          required
        />
        <s-form-input
          v-else
          v-model="form.rentPriceDescription"
          id="rentPriceDescription"
          :label="$t('label.ad-priceDescription')"
          name="rentPriceDescription"
          rules="required"
          :placeholder="$t('placeholder.ad-priceDescription')"
          required
        />
      </div>

      <!-- Sale Section -->
      <s-form-checkbox
        v-model="form.isAvailableForSale"
        id="isAvailableForSale"
        :label="$t('label.isAvailableForSale')"
        name="isAvailableForSale"
        rules="atLeastOneChecked:@hasAtLeastOneTransactionType"
      />
      <div v-if="form.isAvailableForSale" class="sub-section">
        <s-form-input
          v-if="!form.salePriceToBeDetermined"
          v-model="form.salePrice"
          id="salePrice"
          type="number"
          :label="$t('label.ad-sale-price')"
          name="salePrice"
          rules="requiredNumeric"
          :placeholder="$t('placeholder.ad-sale-price')"
          required
          append="$"
        />
        <s-form-checkbox
          v-model="form.salePriceToBeDetermined"
          id="salePriceToBeDetermined"
          :label="$t('label.ad-salePriceToBeDetermined')"
          name="salePriceToBeDetermined"
        />
        <s-form-select
          v-if="form.salePriceToBeDetermined"
          v-model="form.salePriceRange"
          id="salePriceRange"
          :label="$t('label.ad-sale-price-range')"
          name="salePriceRange"
          rules="required"
          :placeholder="$t('placeholder.ad-sale-price-range')"
          :options="salePriceRangeOptions"
          required
        />
        <s-form-input
          v-else
          v-model="form.salePriceDescription"
          id="salePriceDescription"
          :label="$t('label.ad-priceDescription')"
          name="salePriceDescription"
          :placeholder="$t('placeholder.ad-priceDescription')"
        />
      </div>

      <!-- Trade Section -->
      <s-form-checkbox
        v-model="form.isAvailableForTrade"
        id="isAvailableForTrade"
        :label="$t('label.isAvailableForTrade')"
        name="isAvailableForTrade"
        rules="atLeastOneChecked:@hasAtLeastOneTransactionType"
      />
      <div v-if="form.isAvailableForTrade" class="sub-section">
        <s-form-input
          v-model="form.tradeDescription"
          id="tradeDescription"
          :label="$t('label.ad-tradeDescription')"
          name="tradeDescription"
          :placeholder="$t('placeholder.ad-tradeDescription')"
        />
      </div>

      <!-- Donation Section -->
      <s-form-checkbox
        v-model="form.isAvailableForDonation"
        id="isAvailableForDonation"
        :label="$t('label.isAvailableForDonation')"
        name="isAvailableForDonation"
        rules="atLeastOneChecked:@hasAtLeastOneTransactionType"
      />
      <div v-if="form.isAvailableForDonation" class="sub-section">
        <s-form-input
          v-model="form.donationDescription"
          id="donationDescription"
          :label="$t('label.ad-donationDescription')"
          name="donationDescription"
          :placeholder="$t('placeholder.ad-donationDescription')"
        />
      </div>
    </div>
    <div class="section section--md section--padding-x section--border-bottom my-4 pb-5 rm-child-margin">
      <h2 class="my-4">{{ $t("section-title.description") }}</h2>
      <s-form-image
        v-model="form.images"
        :currentImages="form.currentImages"
        id="image"
        :label="$t('label.ad-image')"
        name="image"
        :placeholder="$t('placeholder.ad-image')"
        :drop-placeholder="$t('placeholder.ad-drop-image')"
        rules="required|image"
        required
      />

      <form-partial-delivery-truck v-if="form.category === CATEGORY_DELIVERY_TRUCK" v-model="form" />
      <form-partial-professional-kitchen v-if="form.category === CATEGORY_PROFESSIONAL_KITCHEN" v-model="form" />
      <form-partial-storage-space v-if="form.category === CATEGORY_STORAGE_SPACE" v-model="form" />
      <form-partial-other v-if="isMiscCategory" v-model="form" />

      <s-form-checkbox-group
        v-if="isAllergenCategory"
        v-model="form.allergen"
        id="allergen"
        :label="$t('label.ad-allergen')"
        label-class="h2 mt-4 mb-2"
        name="allergen"
        :options="allergenOptions"
      />
    </div>

    <div class="section section--md section--padding-x section--border-bottom my-4 pb-5 rm-child-margin">
      <fieldset id="availabilitiesFieldset" aria-labelledby="availabilitiesFieldset__legend">
        <legend id="availabilitiesFieldset__legend" class="h2 mt-4 mb-3">{{ $t("label.availability") }}</legend>
        <b-row>
          <b-col>
            <s-form-availability
              v-model="form.dayAvailability"
              id="dayAvailability"
              :label="$t('label.ad-dayAvailability')"
              :specify-label="$t('label.ad-dayAvailability.specify')"
              :specify-label-sr-only="true"
              name="dayAvailability"
              :options="availabilityWeekdayOptions"
            />
          </b-col>
          <b-col>
            <s-form-availability
              v-model="form.eveningAvailability"
              id="eveningAvailability"
              :label="$t('label.ad-eveningAvailability')"
              :specify-label="$t('label.ad-eveningAvailability.specify')"
              :specify-label-sr-only="true"
              name="eveningAvailability"
              :options="availabilityWeekdayOptions"
            />
          </b-col>
        </b-row>
      </fieldset>
    </div>

    <div class="section section--md section--padding-x section--border-bottom my-4 pb-5 rm-child-margin">
      <h2 class="my-4">{{ $t("section-title.contact-details") }}</h2>
      <s-form-input
        v-model="form.organization"
        id="organization"
        :label="$t('label.ad-organization')"
        name="organization"
        rules="required"
        :placeholder="$t('placeholder.ad-organization')"
        required
      />
      <s-form-google-autocomplete
        v-model="form.address"
        id="address"
        :label="$t('label.ad-address')"
        name="address"
        rules="required|haveLatLong"
        :placeholder="$t('placeholder.ad-address')"
        required
      />
      <s-form-checkbox v-model="form.showAddress" id="showAddress" :label="$t('label.ad-showAddress')" name="showAddress" />
    </div>

    <div class="section section--md section--padding-x section--border-bottom my-4 pb-5 rm-child-margin">
      <s-form-checkbox-group
        v-model="form.certification"
        id="certification"
        name="certification"
        :label="$t('section-title.certifications')"
        label-class="h2 mb-4"
        :options="certificationOptions"
      />
    </div>

    <div class="section section--md mt-4 mb-5">
      <s-form-checkbox
        v-model="form.infoIsTrue"
        id="infoIsTrue"
        :label="$t('label.infoIsTrue')"
        name="infoIsTrue"
        :rules="{ required: { allowFalse: false } }"
        required
      />
    </div>
    <div class="fab-container__fab">
      <div class="section section--md">
        <b-button :disabled="disabledBtn" type="submit" :variant="btnVariant" size="lg" block :aria-label="$t('sr.edit')">
          <b-icon icon="pencil" class="mr-2" aria-hidden="true"></b-icon>{{ btnLabel }}
        </b-button>
        <b-button
          v-if="canTransfer"
          :disabled="disabledBtn"
          type="button"
          :to="{ name: $consts.urls.URL_AD_TRANSFER, params: { id: this.adId } }"
          variant="secondary"
          size="lg"
          block
          :aria-label="$t('sr.transfer')"
        >
          <b-icon icon="arrow-right" class="mr-2" aria-hidden="true"></b-icon>{{ transferBtnLabel }}
        </b-button>
      </div>
    </div>
  </s-form>
</template>

<script>
import SForm from "@/components/form/s-form";
import SFormInput from "@/components/form/s-form-input";
import SFormHidden from "@/components/form/s-form-hidden";
import SFormCheckbox from "@/components/form/s-form-checkbox";
import SFormCheckboxGroup from "@/components/form/s-form-checkbox-group";
import SFormSelect from "@/components/form/s-form-select";
import SFormImage from "@/components/form/s-form-image";
import SFormGoogleAutocomplete from "@/components/form/s-form-google-autocomplete";
import SFormAvailability from "@/components/form/s-form-availability";

import FormPartialDeliveryTruck from "@/components/ad/form-partial-delivery-truck";
import FormPartialProfessionalKitchen from "@/components/ad/form-partial-professional-kitchen";
import FormPartialStorageSpace from "@/components/ad/form-partial-storage-space";
import FormPartialOther from "@/components/ad/form-partial-other";

import { AdCategory } from "@/mixins/ad-category";
import { AdSalePriceRange } from "@/mixins/ad-sale-price-range";
import { AdRentalPriceRange } from "@/mixins/ad-rental-price-range";
import { AvailabilityWeekday } from "@/mixins/availability-weekday";
import { Certification } from "@/mixins/certification";
import { Allergen } from "@/mixins/allergen";

import {
  CATEGORY_PROFESSIONAL_KITCHEN,
  CATEGORY_DELIVERY_TRUCK,
  CATEGORY_STORAGE_SPACE,
  CATEGORY_PROFESSIONAL_COOKING_EQUIPMENT,
  CATEGORY_PREP_EQUIPMENT,
  CATEGORY_REFRIGERATION_EQUIPMENT,
  CATEGORY_HEAVY_EQUIPMENT,
  CATEGORY_SURPLUS,
  CATEGORY_OTHER
} from "@/consts/categories";

export default {
  mixins: [AdCategory, AdSalePriceRange, AdRentalPriceRange, AvailabilityWeekday, Certification, Allergen],
  props: {
    adId: {
      type: String,
      default: ""
    },
    title: {
      type: String,
      default: ""
    },
    category: {
      type: String,
      default: ""
    },
    description: {
      type: String,
      default: "<p></p>"
    },
    organization: {
      type: String,
      default: ""
    },
    images: {
      type: Array,
      default: null
    },
    address: {
      type: Object,
      default: null
    },
    showAddress: {
      type: Boolean,
      default: false
    },
    isAvailableForRent: {
      type: Boolean,
      default: false
    },
    isAvailableForSale: {
      type: Boolean,
      default: false
    },
    isAvailableForTrade: {
      type: Boolean,
      default: false
    },
    isAvailableForDonation: {
      type: Boolean,
      default: false
    },
    rentPrice: {
      type: Number
    },
    rentPriceToBeDetermined: {
      type: Boolean,
      default: false
    },
    rentPriceRange: {
      type: String,
      default: ""
    },
    rentPriceDescription: {
      type: String,
      default: ""
    },
    salePrice: {
      type: Number
    },
    salePriceToBeDetermined: {
      type: Boolean,
      default: false
    },
    salePriceRange: {
      type: String,
      default: ""
    },
    salePriceDescription: {
      type: String,
      default: ""
    },
    tradeDescription: {
      type: String,
      default: ""
    },
    donationDescription: {
      type: String,
      default: ""
    },
    surfaceSize: {
      type: String,
      default: ""
    },
    equipment: {
      type: String,
      default: "<p></p>"
    },
    surfaceDescription: {
      type: String,
      default: "<p></p>"
    },
    professionalKitchenEquipment: {
      type: Array,
      default: null
    },
    professionalKitchenEquipmentOther: {
      type: String,
      default: ""
    },
    deliveryTruckType: {
      type: String,
      default: ""
    },
    deliveryTruckTypeOther: {
      type: String,
      default: ""
    },
    allergen: {
      type: Array,
      default: null
    },
    dayAvailability: {
      type: Array,
      default: null
    },
    eveningAvailability: {
      type: Array,
      default: null
    },
    refrigerated: {
      type: Boolean,
      default: false
    },
    canSharedRoad: {
      type: Boolean,
      default: false
    },
    canHaveDriver: {
      type: Boolean,
      default: false
    },
    conditions: {
      type: String,
      default: ""
    },
    certification: {
      type: Array,
      default: null
    },
    infoIsTrue: {
      type: Boolean,
      default: false
    },
    btnLabel: {
      type: String,
      required: true
    },
    btnVariant: {
      type: String,
      default: "primary"
    },
    canTransfer: {
      type: Boolean,
      default: false
    },
    transferBtnLabel: {
      type: String,
      required: false
    },
    disabledBtn: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      form: {
        title: this.title,
        category: this.category,
        description: this.description,
        organization: this.organization,
        currentImages: this.images || [],
        images: this.images || [],
        address: this.address,
        showAddress: this.showAddress,
        isAvailableForRent: this.isAvailableForRent,
        isAvailableForSale: this.isAvailableForSale,
        isAvailableForTrade: this.isAvailableForTrade,
        isAvailableForDonation: this.isAvailableForDonation,
        rentPrice: this.rentPrice,
        rentPriceToBeDetermined: this.rentPriceToBeDetermined,
        rentPriceRange: this.rentPriceRange,
        rentPriceDescription: this.rentPriceDescription,
        salePrice: this.salePrice,
        salePriceToBeDetermined: this.salePriceToBeDetermined,
        salePriceRange: this.salePriceRange,
        salePriceDescription: this.salePriceDescription,
        tradeDescription: this.tradeDescription,
        donationDescription: this.donationDescription,
        conditions: this.conditions,
        surfaceSize: this.surfaceSize,
        equipment: this.equipment,
        surfaceDescription: this.surfaceDescription,
        professionalKitchenEquipment: this.professionalKitchenEquipment || [],
        professionalKitchenEquipmentOther: this.professionalKitchenEquipmentOther,
        deliveryTruckType: this.deliveryTruckType,
        deliveryTruckTypeOther: this.deliveryTruckTypeOther,
        allergen: this.allergen || [],
        dayAvailability: this.dayAvailability || [],
        eveningAvailability: this.eveningAvailability || [],
        refrigerated: this.refrigerated,
        canSharedRoad: this.canSharedRoad,
        canHaveDriver: this.canHaveDriver,
        certification: this.certification || [],
        infoIsTrue: this.infoIsTrue
      },
      CATEGORY_PROFESSIONAL_KITCHEN,
      CATEGORY_DELIVERY_TRUCK,
      CATEGORY_STORAGE_SPACE,
      CATEGORY_OTHER
    };
  },
  components: {
    SForm,
    SFormInput,
    SFormHidden,
    SFormCheckbox,
    SFormCheckboxGroup,
    SFormSelect,
    SFormImage,
    SFormGoogleAutocomplete,
    SFormAvailability,
    FormPartialDeliveryTruck,
    FormPartialProfessionalKitchen,
    FormPartialStorageSpace,
    FormPartialOther
  },
  watch: {
    "form.rentPriceToBeDetermined"() {
      if (this.form.rentPriceToBeDetermined) {
        this.form.rentPrice = null;
        this.form.rentPriceDescription = "";
      }
    },
    "form.salePriceToBeDetermined"() {
      if (this.form.salePriceToBeDetermined) {
        this.form.salePrice = null;
        this.form.salePriceDescription = "";
      }
    },
    "form.isAvailableForRent"(value) {
      if (!value) {
        this.form.rentPrice = null;
        this.form.rentPriceToBeDetermined = false;
        this.form.rentPriceDescription = "";
      }
    },
    "form.isAvailableForSale"(value) {
      if (!value) {
        this.form.salePrice = null;
        this.form.salePriceToBeDetermined = false;
        this.form.salePriceDescription = "";
      }
    },
    "form.isAvailableForTrade"(value) {
      if (!value) {
        this.form.tradeDescription = "";
      }
    },
    "form.isAvailableForDonation"(value) {
      if (!value) {
        this.form.donationDescription = "";
      }
    }
  },
  computed: {
    hasAtLeastOneTransactionType() {
      return (
        this.form.isAvailableForRent ||
        this.form.isAvailableForSale ||
        this.form.isAvailableForTrade ||
        this.form.isAvailableForDonation
      );
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
      return miscCategories.includes(this.form.category);
    },
    isAllergenCategory() {
      const allergenCategories = [
        CATEGORY_PROFESSIONAL_KITCHEN,
        CATEGORY_PREP_EQUIPMENT,
        CATEGORY_PROFESSIONAL_COOKING_EQUIPMENT,
        CATEGORY_SURPLUS,
        CATEGORY_DELIVERY_TRUCK
      ];
      return allergenCategories.includes(this.form.category);
    }
  },
  methods: {
    haveDifferentImages: function (images, formImages) {
      for (let i = 0; i < images.length; i++) {
        if (formImages[i].file.name) {
          if (images[i].src !== formImages[i].file.name) {
            return true;
          }
          if (images[i].alt !== formImages[i].alt) {
            return true;
          }
        }
        if (images[i].alt !== formImages[i].alt) {
          return true;
        }
      }
      return false;
    },
    haveDifferentItems: function (images, formImages) {
      for (let i = 0; i < images.length; i++) {
        if (images[i] !== formImages[i].name) {
          return true;
        }
      }
      return false;
    },
    submitForm: function () {
      let input = {};

      const maybeEditedFields = [
        "title",
        "category",
        "description",
        "address",
        "showAddress",
        "isAvailableForRent",
        "isAvailableForSale",
        "isAvailableForTrade",
        "isAvailableForDonation",
        "rentPrice",
        "rentPriceToBeDetermined",
        "rentPriceRange",
        "rentPriceDescription",
        "salePrice",
        "salePriceToBeDetermined",
        "salePriceRange",
        "salePriceDescription",
        "tradeDescription",
        "donationDescription",
        "conditions",
        "organization",
        "surfaceSize",
        "equipment",
        "surfaceDescription",
        "professionalKitchenEquipment",
        "professionalKitchenEquipmentOther",
        "deliveryTruckType",
        "deliveryTruckTypeOther",
        "allergen",
        "dayAvailability",
        "eveningAvailability",
        "refrigerated",
        "canSharedRoad",
        "canHaveDriver",
        "certification",
        "infoIsTrue"
      ];
      for (let maybeEditedField of maybeEditedFields) {
        if (
          Array.isArray(this[maybeEditedField]) &&
          this[maybeEditedField].sort().join(",") === this.form[maybeEditedField].sort().join(",")
        ) {
          continue;
        }
        if (this[maybeEditedField] === this.form[maybeEditedField]) continue;

        input[maybeEditedField] = this.form[maybeEditedField];
      }

      if (
        this.images === null ||
        this.images.length !== this.form.images.length ||
        this.haveDifferentImages(this.images, this.form.images)
      ) {
        input.galleryItems = this.form.images;
      }
      this.$emit("submitForm", input);
    }
  }
};
</script>

<style lang="scss">
.ad-form {
  // Override style legend with h2 class
  .h2 {
    padding-bottom: 0;
    padding-top: 0;
    font-size: 2rem;

    @include media-breakpoint-down(lg) {
      font-size: calc(1.325rem + 0.9vw);
    }
  }

  .sub-section {
    margin-top: $spacer / -2;
    margin-bottom: $spacer * 2;
    border-left: 3px solid $yellow;
    padding-left: $spacer * 1.25;
  }
}
</style>
