<template>
  <div class="fab-container">
    <template v-if="!adEdited">
      <portal :to="$consts.enums.PORTAL_HEADER">
        <nav-close :to="{ name: $consts.urls.URL_AD_DETAIL, params: { id: adId } }"></nav-close>
      </portal>
      <div class="section section--md section--padding-x section--border-bottom my-4">
        <h1 class="my-4">{{ $t("page-title.edit-ad") }}</h1>
      </div>
      <ad-form
        v-if="ad"
        :adId="adId"
        :title="ad.translationOrDefault.title"
        :description="ad.translationOrDefault.description"
        :category="ad.category"
        :is-available-for-sale="ad.isAvailableForSale"
        :is-available-for-rent="ad.isAvailableForRent"
        :is-available-for-trade="ad.isAvailableForTrade"
        :is-available-for-donation="ad.isAvailableForDonation"
        :images="ad.gallery"
        :address="ad.address"
        :show-address="ad.showAddress"
        :rent-price="ad.rentPrice"
        :rentPriceToBeDetermined="ad.rentPriceToBeDetermined"
        :rentPriceDescription="ad.translationOrDefault.rentPriceDescription"
        :rentPriceRange="ad.rentPriceRange"
        :sale-price="ad.salePrice"
        :salePriceToBeDetermined="ad.salePriceToBeDetermined"
        :salePriceDescription="ad.translationOrDefault.salePriceDescription"
        :salePriceRange="ad.salePriceRange"
        :tradeDescription="ad.translationOrDefault.tradeDescription"
        :donationDescription="ad.translationOrDefault.donationDescription"
        :conditions="ad.translationOrDefault.conditions"
        :organization="ad.organization"
        :surfaceSize="ad.translationOrDefault.surfaceSize"
        :equipment="ad.translationOrDefault.equipment"
        :surfaceDescription="ad.translationOrDefault.surfaceDescription"
        :professionalKitchenEquipment="ad.professionalKitchenEquipment"
        :professionalKitchenEquipmentOther="ad.translationOrDefault.professionalKitchenEquipmentOther"
        :deliveryTruckType="ad.deliveryTruckType"
        :deliveryTruckTypeOther="ad.translationOrDefault.deliveryTruckTypeOther"
        :dayAvailability="ad.dayAvailability"
        :eveningAvailability="ad.eveningAvailability"
        :availabilityRestriction="ad.availabilityRestriction"
        :refrigerated="ad.refrigerated"
        :canHaveDriver="ad.canHaveDriver"
        :canSharedRoad="ad.canSharedRoad"
        :certification="ad.certification"
        :allergen="ad.allergen"
        @submitForm="editAd"
        :btnLabel="$t('btn.edit-ad-save')"
        :transferBtnLabel="$t('btn.transfer-ad')"
        :canTransfer="isAdmin"
        :disabledBtn="isSubmitted"
      />
    </template>
    <form-complete
      v-else
      :title="$t('form-complete.create-ad.title')"
      :description="$t('form-complete.create-ad.description')"
      :image="require('@/assets/icons/checklist-yellow.svg')"
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
    isConnected() {
      return this.user && this.user.isConnected;
    },
    isAdmin() {
      return !this.me || this.me.type === this.$consts.enums.USER_TYPE_ADMIN;
    },
    adId() {
      return this.$route.params.id.split("-").last();
    }
  },
  methods: {
    async editAd(input) {
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

query AdById($id: ID!, $language: ContentLanguage!) {
  ad(id: $id) {
    id
    isAvailableForSale
    isAvailableForRent
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
    rentPrice
    rentPriceToBeDetermined
    rentPriceRange
    salePrice
    salePriceToBeDetermined
    salePriceRange
    organization
    refrigerated
    canSharedRoad
    canHaveDriver
    professionalKitchenEquipment
    deliveryTruckType
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
</graphql>
