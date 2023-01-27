<template>
  <s-form @submit="submitForm">
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
    <s-form-input
      v-if="!form.priceToBeDetermined"
      v-model="form.price"
      id="price"
      type="number"
      :label="$t('label.ad-price')"
      name="price"
      rules="requiredNumeric"
      :placeholder="$t('placeholder.ad-price')"
      required
      append="$"
    />
    <s-form-checkbox
      v-model="form.priceToBeDetermined"
      id="priceToBeDetermined"
      :label="$t('label.ad-priceToBeDetermined')"
      name="priceToBeDetermined"
    />
    <s-form-input
      v-if="!form.priceToBeDetermined"
      v-model="form.priceDescription"
      id="priceDescription"
      :label="$t('label.ad-priceDescription')"
      name="priceDescription"
      rules="required"
      :placeholder="$t('placeholder.ad-priceDescription')"
      required
    />
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
    <form-partial-delivery-truck v-if="false" v-model="form" />
    <form-partial-professional-kitchen v-if="false" v-model="form" />
    <form-partial-storage-space v-if="false" v-model="form" />
    <form-partial-other v-if="true" v-model="form" />
    <s-form-availability
      v-if="false"
      v-model="form.dayAvailability"
      id="dayAvailability"
      :label="$t('label.ad-dayAvailability')"
      :specify-label="$t('label.ad-dayAvailability.specify')"
      name="dayAvailability"
      :options="availabilityWeekdayOptions"
    />
    <s-form-availability
      v-if="false"
      v-model="form.eveningAvailability"
      id="eveningAvailability"
      :label="$t('label.ad-eveningAvailability')"
      :specify-label="$t('label.ad-eveningAvailability.specify')"
      name="eveningAvailability"
      :options="availabilityWeekdayOptions"
    />
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
    <s-form-checkbox
      v-if="false"
      v-model="form.showAddress"
      id="showAddress"
      :label="$t('label.ad-showAddress')"
      name="showAddress"
    />
    <div class="fab-container__fab">
      <b-button :disabled="disabledBtn" type="submit" variant="primary" size="lg" block :aria-label="$t('sr.edit')">
        <b-icon icon="pencil" class="mr-2" aria-hidden="true"></b-icon>{{ btnLabel }}
      </b-button>
    </div>
  </s-form>
</template>

<script>
import SForm from "@/components/form/s-form";
import SFormInput from "@/components/form/s-form-input";
import SFormCheckbox from "@/components/form/s-form-checkbox";
import SFormSelect from "@/components/form/s-form-select";
import SFormImage from "@/components/form/s-form-image";
import SFormGoogleAutocomplete from "@/components/form/s-form-google-autocomplete";
import SFormAvailability from "@/components/form/s-form-availability";

import FormPartialDeliveryTruck from "@/components/ad/form-partial-delivery-truck";
import FormPartialProfessionalKitchen from "@/components/ad/form-partial-professional-kitchen";
import FormPartialStorageSpace from "@/components/ad/form-partial-storage-space";
import FormPartialOther from "@/components/ad/form-partial-other";

import { AdCategory } from "@/mixins/ad-category";
import { AvailabilityWeekday } from "@/mixins/availability-weekday";

import {
  CATEGORY_PROFESSIONAL_KITCHEN,
  CATEGORY_DELIVERY_TRUCK,
  CATEGORY_STORAGE_SPACE,
  CATEGORY_OTHER
} from "@/consts/categories";

export default {
  mixins: [AdCategory, AvailabilityWeekday],
  props: {
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
    price: {
      type: Number
    },
    priceToBeDetermined: {
      type: Boolean,
      default: false
    },
    priceDescription: {
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
    btnLabel: {
      type: String,
      required: true
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
        price: this.price,
        priceToBeDetermined: this.priceToBeDetermined,
        priceDescription: this.priceDescription,
        conditions: this.conditions,
        surfaceSize: this.surfaceSize,
        equipment: this.equipment,
        surfaceDescription: this.surfaceDescription,
        professionalKitchenEquipment: this.professionalKitchenEquipment || [],
        professionalKitchenEquipmentOther: this.professionalKitchenEquipmentOther,
        deliveryTruckType: this.deliveryTruckType,
        deliveryTruckTypeOther: this.deliveryTruckTypeOther,
        dayAvailability: this.dayAvailability || [],
        eveningAvailability: this.eveningAvailability || [],
        refrigerated: this.refrigerated,
        canSharedRoad: this.canSharedRoad,
        canHaveDriver: this.canHaveDriver
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
    SFormCheckbox,
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
    "form.priceToBeDetermined"() {
      if (this.form.priceToBeDetermined) {
        this.form.price = null;
        this.form.priceDescription = "";
      }
    }
  },
  methods: {
    haveDifferentImages: function(images, formImages) {
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
    haveDifferentItems: function(images, formImages) {
      for (let i = 0; i < images.length; i++) {
        if (images[i] !== formImages[i].name) {
          return true;
        }
      }
      return false;
    },
    submitForm: function() {
      let input = {};

      const maybeEditedFields = [
        "title",
        "category",
        "description",
        "address",
        "showAddress",
        "price",
        "priceToBeDetermined",
        "priceDescription",
        "conditions",
        "organization",
        "surfaceSize",
        "equipment",
        "surfaceDescription",
        "professionalKitchenEquipment",
        "professionalKitchenEquipmentOther",
        "deliveryTruckType",
        "deliveryTruckTypeOther",
        "dayAvailability",
        "eveningAvailability",
        "refrigerated",
        "canSharedRoad",
        "canHaveDriver"
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
