<template>
  <div class="fab-container">
    <template v-if="!adEdited">
      <portal :to="$consts.enums.PORTAL_HEADER">
        <nav-close :to="{ name: $consts.urls.URL_AD_DETAIL, params: { id: this.adId } }"></nav-close>
      </portal>
      <div class="section section--md my-4">
        <h1 class="h2 my-4">{{ $t("page-title.edit-ad") }}</h1>
        <ad-form
          v-if="ad"
          :title="adTitle"
          :description="adDescription"
          :category="adCategory"
          :images="adImages"
          :address="adAddress"
          :show-address="adShowAddress"
          :price="adPrice"
          :priceToBeDetermined="adPriceToBeDetermined"
          :priceDescription="adPriceDescription"
          :conditions="adConditions"
          :organization="adOrganization"
          :surfaceSize="adSurfaceSize"
          :equipment="adEquipment"
          :surfaceDescription="adSurfaceDescription"
          :professionalKitchenEquipment="adProfessionalKitchenEquipment"
          :professionalKitchenEquipmentOther="adProfessionalKitchenEquipmentOther"
          :deliveryTruckType="adDeliveryTruckType"
          :deliveryTruckTypeOther="adDeliveryTruckTypeOther"
          :dayAvailability="adDayAvailability"
          :eveningAvailability="adEveningAvailability"
          :refrigerated="adRefrigerated"
          :canHaveDriver="adCanHaveDriver"
          :canSharedRoad="adCanSharedRoad"
          @submitForm="editAd"
          :btnLabel="$t('btn.edit-ad')"
          :disabledBtn="isSubmitted"
        />
      </div>
    </template>
    <form-complete
      v-else
      :title="$t('form-complete.create-ad.title')"
      :description="$t('form-complete.create-ad.description')"
      :image="require('@/assets/icons/checklist.png')"
      :ctas="formCompleteCtas"
    />
  </div>
</template>

<script>
import NavClose from "@/components/nav/close";
import FormComplete from "@/components/generic/form-complete";
import AdForm from "@/components/ad/form";

import { URL_ROOT, URL_AD_DETAIL, URL_AD_EDIT } from "@/consts/urls";
import { CONTENT_LANG_FR } from "@/consts/langs";

import NotificationService from "@/services/notification";
import { updateAd } from "@/services/ad";

export default {
  components: {
    NavClose,
    AdForm,
    FormComplete
  },
  data() {
    return {
      adEdited: false,
      isSubmitted: false,
      formCompleteCtas: [
        {
          action: () => this.$router.push({ name: URL_AD_DETAIL, params: { id: this.adId } }),
          text: this.$t("btn.display-detail-ad")
        },
        { action: () => this.$router.push({ name: URL_ROOT }), text: this.$t("btn.return-dashboard") }
      ]
    };
  },
  apollo: {
    ad: {
      fetchPolicy: "no-cache",
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
              name: URL_AD_EDIT,
              params: { id: slugiffyUrl }
            });
          }
        }
      }
    }
  },
  gqlErrors() {
    return {
      IMAGE_NOT_FOUND(error) {
        return this.$t("error.image-upload");
      }
    };
  },
  computed: {
    adId: function() {
      return this.$route.params.id.split("-").last();
    },
    adTitle: function() {
      return this.ad.translationOrDefault.title;
    },
    adDescription: function() {
      return this.ad.translationOrDefault.description;
    },
    adCategory: function() {
      return this.ad.category;
    },
    adAddress: function() {
      return this.ad.address;
    },
    adShowAddress: function() {
      return this.ad.showAddress;
    },
    adImages: function() {
      return this.ad.gallery;
    },
    adPrice: function() {
      return this.ad.price;
    },
    adPriceToBeDetermined: function() {
      return this.ad.priceToBeDetermined;
    },
    adPriceDescription: function() {
      return this.ad.translationOrDefault.priceDescription;
    },
    adConditions: function() {
      return this.ad.translationOrDefault.conditions;
    },
    adOrganization: function() {
      return this.ad.organization;
    },
    adSurfaceSize: function() {
      return this.ad.translationOrDefault.surfaceSize;
    },
    adEquipment: function() {
      return this.ad.translationOrDefault.equipment;
    },
    adSurfaceDescription: function() {
      return this.ad.translationOrDefault.surfaceDescription;
    },
    adProfessionalKitchenEquipmentOther: function() {
      return this.ad.translationOrDefault.professionalKitchenEquipmentOther;
    },
    adDeliveryTruckTypeOther: function() {
      return this.ad.translationOrDefault.deliveryTruckTypeOther;
    },
    adProfessionalKitchenEquipment: function() {
      return this.ad.professionalKitchenEquipment;
    },
    adDeliveryTruckType: function() {
      return this.ad.deliveryTruckType;
    },
    adDayAvailability: function() {
      return this.ad.dayAvailability;
    },
    adEveningAvailability: function() {
      return this.ad.eveningAvailability;
    },
    adRefrigerated: function() {
      return this.ad.refrigerated;
    },
    adCanHaveDriver: function() {
      return this.ad.canHaveDriver;
    },
    adCanSharedRoad: function() {
      return this.ad.canSharedRoad;
    }
  },
  methods: {
    editAd: async function(input) {
      if (input && Object.keys(input).length === 0 && input.constructor === Object) {
        NotificationService.showInfo(this.$t("notification.edit-ad-empty"));
      } else {
        this.isSubmitted = true;
        input.adId = this.adId;
        await updateAd(input);
        this.adEdited = true;
        window.scrollTo(0, 0);
        this.isSubmitted = false;
      }
    }
  }
};
</script>

<graphql>
query AdById($id: ID!, $language: ContentLanguage!) {
  ad(id: $id) {
    id
    translationOrDefault(language: $language) {
      id
      language
      title
      description
      priceDescription
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
    price
    priceToBeDetermined
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
</graphql>
