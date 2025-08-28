<template>
  <div class="equipment-snippet focus-overflow w-100">
    <div class="section section--full-width-mobile" :class="`section--${sectionWidth}`">
      <UiSnippet
        :snippetIsLink="snippetIsLink"
        :hideActions="hideSnippetActions"
        :title="title"
        :noWrapTitle="noWrapTitle"
        :smallTitle="smallTitle"
        :suptitle="organization"
        :imageSrc="image.src"
        :imageAlt="image.alt"
        :mainRoute="{ name: $consts.urls.URL_AD_DETAIL, params: { id: this.id } }"
        :mainRouteLabel="$t('btn.ad-detail')"
        :is-admin-only="isAdminOnly"
        :is-published="isPublished"
        :is-locked="isLocked"
      >
        <template v-if="priceDetails" #description>
          <ul class="equipment-snippet__types">
            <li v-if="priceDetails.isAvailableForRent" class="equipment-snippet__types-item">
              <b-img :src="require('@/assets/icons/rent.svg')" alt="" height="16" block></b-img>
              <div class="equipment-snippet__types-text">
                <div class="equipment-snippet__types-title">{{ $t("label.forRent") }}</div>
                <div
                  class="equipment-snippet__types-price"
                  :class="{ 'equipment-snippet__types-price--sm': priceDetails.rentPriceToBeDetermined }"
                >
                  {{ priceDetails.rentPrice }}
                </div>
              </div>
            </li>
            <li v-if="priceDetails.isAvailableForSale" class="equipment-snippet__types-item">
              <b-img :src="require('@/assets/icons/sale.svg')" alt="" height="16" block></b-img>
              <div class="equipment-snippet__types-text">
                <div class="equipment-snippet__types-title">{{ $t("label.forSale") }}</div>
                <div
                  class="equipment-snippet__types-price"
                  :class="{ 'equipment-snippet__types-price--sm': priceDetails.salePriceToBeDetermined }"
                >
                  {{ priceDetails.salePrice }}
                </div>
              </div>
            </li>
            <li v-if="priceDetails.isAvailableForTrade" class="equipment-snippet__types-item">
              <b-img :src="require('@/assets/icons/trade.svg')" alt="" height="16" block></b-img>
              <div class="equipment-snippet__types-text">
                <div class="equipment-snippet__types-title">{{ $t("label.forTrade") }}</div>
              </div>
            </li>
            <li v-if="priceDetails.isAvailableForDonation" class="equipment-snippet__types-item">
              <b-img :src="require('@/assets/icons/donation.svg')" alt="" height="16" block></b-img>
              <div class="equipment-snippet__types-text">
                <div class="equipment-snippet__types-title">{{ $t("label.forDonation") }}</div>
              </div>
            </li>
          </ul>
        </template>

        <template #actions>
          <b-button
            v-if="canTransfer"
            :to="{ name: $consts.urls.URL_AD_TRANSFER, params: { id } }"
            variant="outline-admin"
            size="sm"
            class="mt-2 ml-2"
          >
            <b-icon icon="arrow-right" class="mr-1" aria-hidden="true"></b-icon>
            {{ $t("btn.transfer") }}
          </b-button>
          <template v-if="showManageBtn">
            <b-button v-if="isPublished" @click="unpublishAd" variant="outline-primary" size="sm" class="mt-2 ml-2">
              <b-icon icon="eye-slash-fill" class="mr-1" aria-hidden="true"></b-icon>
              {{ $t("btn.unpublish") }}
            </b-button>
            <b-button v-else @click="publishAd" :disabled="isLocked" variant="outline-primary" size="sm" class="mt-2 ml-2">
              <b-icon icon="eye-fill" class="mr-1" aria-hidden="true"></b-icon>
              {{ $t("btn.publish") }}
            </b-button>
          </template>
          <div v-if="conversation">
            <b-button
              v-if="haveContract"
              variant="primary"
              size="sm"
              class="mt-2"
              :to="{ name: $consts.urls.URL_CONTRACT_DETAIL, params: { id: contractId } }"
            >
              <b-icon icon="file-earmark-check" class="mr-1" aria-hidden="true"></b-icon>
              {{ $t("btn.contract-detail") }}
            </b-button>
            <!-- Disable for Pilote version -->
            <!--template v-else-if="canCreateContract">
              <b-button
                v-if="isAccountOnboardingComplete"
                variant="primary"
                size="sm"
                class="mt-2"
                :to="{ name: $consts.urls.URL_CREATE_CONTRACT, params: { id: conversationId } }"
                target="_blank"
              >
                <b-icon icon="pencil-square" class="mr-1" aria-hidden="true"></b-icon>
                {{ $t("btn.create-contract") }}
              </b-button>
              <b-button
                v-else
                variant="primary"
                size="sm"
                class="mt-2"
                :to="{ name: $consts.urls.URL_ADD_PAYMENT, params: { id: conversationId } }"
                target="_blank"
              >
                <b-icon icon="pencil-square" class="mr-1" aria-hidden="true"></b-icon>
                {{ $t("btn.create-contract") }}
              </b-button>
            </template-->
          </div>
        </template>
      </UiSnippet>
    </div>
  </div>
</template>

<script>
import { unpublishAd, publishAd } from "@/services/ad";
import UiSnippet from "@/components/ui/snippet";

export default {
  components: {
    UiSnippet
  },
  props: {
    id: {
      type: String,
      required: true
    },
    title: {
      type: String,
      required: true
    },
    titleTag: {
      type: String,
      default: "h2"
    },
    image: {
      type: Object,
      required: true
    },
    priceDetails: Object,
    sectionWidth: {
      type: String,
      default: "md"
    },
    organization: {
      type: String
    },
    snippetIsLink: Boolean,
    hideSnippetActions: Boolean,
    smallTitle: Boolean,
    noWrapTitle: Boolean,
    conversation: Object,
    conversationId: String,
    canCreateContract: Boolean,
    isAccountOnboardingComplete: Boolean,
    showAdDetailBtn: Boolean,
    showManageBtn: Boolean,
    isPublished: {
      type: Boolean,
      default: true
    },
    isLocked: {
      type: Boolean,
      default: false
    },
    isAdminOnly: Boolean,
    canTransfer: Boolean
  },
  computed: {
    haveContract: function () {
      return this.conversation.contract !== null;
    },
    contractId: function () {
      return this.conversation.contract.id;
    }
  },
  methods: {
    unpublishAd: async function () {
      await unpublishAd(this.id);
    },
    publishAd: async function () {
      await publishAd(this.id);
    }
  }
};
</script>

<style lang="scss">
.equipment-snippet {
  & {
    background-color: $body-bg;
    border-bottom: 1px solid $gray-200;
  }

  &:first-child {
    border-top: 1px solid $gray-200;
  }

  &__ad-link {
    display: block;
    text-decoration: none;
  }

  &__types {
    display: flex;
    flex-wrap: wrap;
    row-gap: $spacer / 1.5;
    column-gap: $spacer;
    list-style-type: none;
    padding-left: 0;
    margin: $spacer 0 0;

    &-item {
      display: flex;
      flex: 1 1 0;
      column-gap: $spacer / 2;
      text-transform: uppercase;
      font-weight: 700;
      color: $gray-700;
      line-height: 1.4;
    }

    &-title {
      font-size: 10px;
      @include media-breakpoint-up(lg) {
        font-size: 12px;
      }
    }

    &-price {
      font-size: 13px;
      text-wrap: nowrap;
      @include media-breakpoint-up(lg) {
        font-size: 14px;
      }

      &--sm {
        font-size: 12px;
        text-transform: none;
        font-weight: 400;
      }
    }
  }
}
</style>
