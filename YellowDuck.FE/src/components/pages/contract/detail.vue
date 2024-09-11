<template>
  <div v-if="contract" class="fab-container">
    <portal :to="$consts.enums.PORTAL_HEADER">
      <nav-close :to="{ name: $consts.urls.URL_CONVERSATION_DETAIL, params: { id: this.conversationId } }"></nav-close>
    </portal>

    <div class="mb-4">
      <ad-snippet :id="adId" :title="adTitle" :image="adImage" :organization="adOrganization" />

      <div class="section section--md">
        <h1 class="my-4">{{ $t("page-title.contract-detail") }}</h1>
      </div>
      <div class="section section--md section--border-top my-4">
        <h2 class="font-family-base font-weight-bold mt-4 mb-3">{{ $t("label.contract-participants") }}</h2>
        <h3 class="text-secondary mt-3 mb-2">{{ $t("label.contract-owner") }}</h3>
        <div class="mb-4">{{ ownerOrganizationName }} / {{ ownerName }}</div>
        <h3 class="text-secondary mt-3 mb-2">{{ $t("label.contract-tenant") }}</h3>
        <div class="mb-4">{{ tenantOrganizationName }} / {{ tenantName }}</div>
      </div>
      <div class="section section--md section--border-top my-4">
        <h2 class="font-family-base font-weight-bold mt-4 mb-3">{{ $t("label.contract-terms") }}</h2>
        <div class="mb-4" v-html="contractTerms"></div>
      </div>
      <div class="section section--md section--border-top my-4">
        <h2 class="font-family-base font-weight-bold mt-4 mb-3">{{ $t("label.contract-date") }}</h2>
        <div class="mb-4">{{ $t("description.contract-date", { startDate: contractStartDate, endDate: contractEndDate }) }}</div>
        <h3 class="text-secondary mt-3 mb-2">{{ $t("label.contract-date-precision") }}</h3>
        <div class="mb-4">{{ contractDatePrecision }}</div>
      </div>
      <div class="section section--md section--border-top my-4">
        <h2 class="font-family-base font-weight-bold mt-4 mb-3">{{ $t("label.contract-price") }}</h2>
        <div class="mb-4">{{ contractPrice }}</div>
      </div>
      <div v-if="haveFiles" class="section section--md section--border-top my-4">
        <h2 class="font-family-base font-weight-bold mt-4 mb-3">{{ $t("label.contract-files") }}</h2>
        <b-button v-for="file in contractFiles" :key="file"
          ><a :href="file">{{ getFileName(file) }}</a></b-button
        >
      </div>
      <div class="section section--md section--border-top my-4">
        <h2 class="font-family-base font-weight-bold mt-4 mb-3">{{ $t("contract-form.verify-before-accept-and-pay") }}</h2>
        <p v-html="contractVerificationDescription" class="font-family-base mt-4 mb-3"></p>
      </div>
      <div class="section section--md section--border-top my-4">
        <p v-html="$t('contract-form.practical-info-mutuali')" class="font-family-base mt-4 mb-3"></p>
      </div>
      <div :class="{ 'fab-container__fab': canEditContract }">
        <div class="section section--md">
          <b-button
            v-if="canEditContract"
            :to="{ name: $consts.urls.URL_CONTRACT_EDIT, params: { id: contractId } }"
            variant="primary"
            size="lg"
            class="text-truncate"
            block
          >
            <b-icon icon="pencil" class="mr-2" aria-hidden="true"></b-icon>
            {{ $t("btn.edit-contract") }}
          </b-button>
          <template v-else-if="canPayContract">
            <s-form-checkbox
              id="disclaimer"
              :label="$t('label.disclaimer-accepted')"
              name="disclaimer"
              :rules="{ required: { allowFalse: false } }"
              v-model="disclaimerAccepted"
              required
            />
            <b-button
              :disabled="!disclaimerAccepted"
              @click="acceptAndPay"
              variant="primary"
              size="lg"
              class="text-truncate"
              block
            >
              {{ $t("btn.accept-pay") }}
            </b-button>
          </template>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import AdSnippet from "@/components/ad/snippet";
import NavClose from "@/components/nav/close";
import SFormCheckbox from "@/components/form/s-form-checkbox";

import i18nHelpers from "@/helpers/i18n";

import PaymentService from "@/services/payment";

import {
  CATEGORY_PROFESSIONAL_KITCHEN,
  CATEGORY_DELIVERY_TRUCK,
  CATEGORY_STORAGE_SPACE,
  CATEGORY_STORAGE_OTHER
} from "@/consts/categories";
import { CONTRACT_STATUS_SUBMITTED } from "@/consts/contract-status";
import { CONTENT_LANG_FR } from "@/consts/langs";
import { SHORT_DAY_MONTH_YEAR } from "@/consts/formats";

export default {
  components: {
    AdSnippet,
    NavClose,
    SFormCheckbox
  },
  methods: {
    getFileName(file) {
      let last = file.split("/").last();
      return last.substring(0, last.indexOf("?"));
    },
    async acceptAndPay() {
      let result = await PaymentService.createCheckoutSession(this.contractId);
      window.location = result.data.createCheckoutSession.checkoutSession.checkoutLink;
    }
  },
  data() {
    return {
      disclaimerAccepted: false
    };
  },
  computed: {
    contractVerificationDescription: function () {
      switch (this.adCategory) {
        case CATEGORY_PROFESSIONAL_KITCHEN: {
          return this.$t("contract-form.verify-before-accept-and-pay.kitchen");
        }
        case CATEGORY_DELIVERY_TRUCK: {
          return this.$t("contract-form.verify-before-accept-and-pay.truck");
        }
        case CATEGORY_STORAGE_SPACE: {
          return this.$t("contract-form.verify-before-accept-and-pay.storage");
        }
        case CATEGORY_STORAGE_OTHER: {
          return this.$t("contract-form.verify-before-accept-and-pay.other");
        }
      }

      return "";
    },
    canEditContract: function () {
      return this.isAdOwnByCurrentUser && this.contract.status === CONTRACT_STATUS_SUBMITTED;
    },
    canPayContract: function () {
      return this.contract.status === CONTRACT_STATUS_SUBMITTED;
    },
    isAdOwnByCurrentUser: function () {
      return this.me ? this.owner.id === this.me.id : false;
    },
    haveFiles: function () {
      return this.contractFiles.length > 0;
    },
    contractId: function () {
      return this.$route.params.id.split("-").last();
    },
    adId: function () {
      return this.contract.conversation.ad.id;
    },
    adTitle: function () {
      return this.contract.conversation.ad.translationOrDefault.title;
    },
    adOrganization: function () {
      return this.contract.conversation.ad.organization;
    },
    adImage: function () {
      return this.contract.conversation.ad.gallery[0];
    },
    adPrice: function () {
      return this.$format.formatMoney(this.contract.conversation.ad.price);
    },
    adCategory: function () {
      return this.contract.conversation.ad.category;
    },
    adPriceDescription: function () {
      return this.contract.conversation.ad.translationOrDefault.priceDescription;
    },
    contractStartDate: function () {
      return i18nHelpers.getLocalizedDate(this.contract.startDate, SHORT_DAY_MONTH_YEAR);
    },
    contractEndDate: function () {
      return i18nHelpers.getLocalizedDate(this.contract.endDate, SHORT_DAY_MONTH_YEAR);
    },
    contractDatePrecision: function () {
      return this.contract.datePrecision;
    },
    contractTerms: function () {
      return this.contract.terms;
    },
    contractPrice: function () {
      return this.$format.formatMoney(this.contract.price);
    },
    contractFiles: function () {
      return this.contract.files;
    },
    owner: function () {
      return this.contract.owner;
    },
    ownerOrganizationName: function () {
      return this.owner !== null ? this.owner.profile.organizationName : "";
    },
    ownerName: function () {
      return this.owner !== null ? this.owner.profile.publicName : "";
    },
    tenant: function () {
      return this.contract.tenant;
    },
    tenantName: function () {
      return this.tenant !== null ? this.tenant.profile.publicName : "";
    },
    tenantOrganizationName: function () {
      return this.owner !== null ? this.tenant.profile.organizationName : "";
    },
    conversationId: function () {
      return this.contract.conversation.id;
    }
  },
  apollo: {
    contract: {
      query() {
        return this.$options.query.ContractById;
      },
      variables() {
        return {
          id: this.contractId,
          language: CONTENT_LANG_FR
        };
      }
    },
    me: {
      query() {
        return this.$options.query.Me;
      }
    }
  }
};
</script>

<graphql>
query ContractById($id: ID!, $language: ContentLanguage!) {
  contract(id: $id) {
    id
    conversation {
      id
      ad {
        id
        gallery {
          id
          src
          alt
        }
        isAvailableForRent
        isAvailableForSale
        isAvailableForTrade
        isAvailableForDonation
        rentPrice
        salePrice
        rentPriceToBeDetermined
        salePriceToBeDetermined
        category
        translationOrDefault(language: $language) {
          id
          title
          rentPriceDescription
          salePriceDescription
          tradeDescription
          donationDescription
        }
        organization
      }
    }
    owner {
      id
      profile {
        id
        ... on UserProfileGraphType {
          publicName
          organizationName
        }
      }
    }
    tenant {
      id
      profile {
        id
        ... on UserProfileGraphType {
          publicName
          organizationName
        }
      }
    }
    datePrecision
    files
    price
    startDate
    endDate
    terms
    status
  }
}

query Me {
  me {
    id
  }
}
</graphql>

<style lang="scss"></style>
