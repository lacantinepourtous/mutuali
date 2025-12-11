<template>
  <ul class="transaction-types">
    <li v-if="adPriceDetails.isAvailableForRent">
      <AdTypeCard
        :title="$t('label.forRent')"
        :price="adPriceDetails.rentPrice"
        :price-to-be-determined="adPriceDetails.rentPriceToBeDetermined"
        :modality="adPriceDetails.rentPriceDescription"
        :footnote="adPriceDetails.rentPriceRange"
      >
        <IconRent class="transaction-types__category-icon" />
      </AdTypeCard>
    </li>
    <li v-if="adPriceDetails.isAvailableForSale">
      <AdTypeCard
        :title="$t('label.forSale')"
        :price="adPriceDetails.salePrice"
        :price-to-be-determined="adPriceDetails.salePriceToBeDetermined"
        :modality="adPriceDetails.salePriceDescription"
        :footnote="adPriceDetails.salePriceRange"
      >
        <IconSale class="transaction-types__category-icon" />
      </AdTypeCard>
    </li>
    <li v-if="adPriceDetails.isAvailableForTrade">
      <AdTypeCard :title="$t('label.forTrade')" :description="adPriceDetails.tradeDescription">
        <IconTrade class="transaction-types__category-icon" />
      </AdTypeCard>
    </li>
    <li v-if="adPriceDetails.isAvailableForDonation">
      <AdTypeCard :title="$t('label.forDonation')" :description="adPriceDetails.donationDescription">
        <IconDonation class="transaction-types__category-icon" />
      </AdTypeCard>
    </li>
    <li v-if="ad.category === CATEGORY_SUBCONTRACTING">
      <AdTypeCard :title="$t('select.category-subcontracting')">
        <IconWorker class="transaction-types__category-icon" />
        <template #text>
          <span class="transaction-types__emphase">
            {{ $t('text.ad-subcontracting-info', {name: ad.user.profile.publicName}) }}
          </span>
        </template>
      </AdTypeCard>
    </li>
    <li v-if="ad.category === CATEGORY_HUMAN_RESOURCES">
      <AdTypeCard :title="$t('label.ad-human-resource-field')">
        <IconWorker class="transaction-types__category-icon" />
        <template #text>
          <span class="transaction-types__emphase">
            {{ getHumanResourceFieldLabel(ad.humanResourceField) }}
          </span>
        </template>
      </AdTypeCard>
    </li>
  </ul>
</template>

<script>
import {
  CATEGORY_SUBCONTRACTING,
  CATEGORY_HUMAN_RESOURCES
} from "@/consts/categories";


import { AdHumanResourceField } from "@/mixins/ad-human-resource-field";
import { AdCategory } from "@/mixins/ad-category";
import { PriceDetails } from "@/mixins/price-details";

import AdTypeCard from "@/components/ad/type-card";
import IconRent from "@/components/icon/rent";
import IconSale from "@/components/icon/sale";
import IconTrade from "@/components/icon/trade";
import IconDonation from "@/components/icon/donation";
import IconWorker from "@/components/icon/worker";

export default {
  mixins: [PriceDetails, AdCategory, AdHumanResourceField],
  components: {
    AdTypeCard,
    IconRent,
    IconSale,
    IconTrade,
    IconDonation,
    IconWorker
  },
  props: {
    ad: {
      type: Object,
      default() {
        return {
          category: "",
          user: {
            profile: {
              publicName: ""
            }
          }
        };
      }
    }
  },
  data() {
    return {
      CATEGORY_SUBCONTRACTING,
      CATEGORY_HUMAN_RESOURCES
    };
  },
  computed: {
    adPriceDetails() {
      return this.getPriceDetailsFromAd(this.ad);
    },
  },
};
</script>

<style lang="scss" scoped>
.transaction-types {
  list-style-type: none;
  padding-left: 0;

  @include media-breakpoint-up(lg) {
    display: flex;
    column-gap: $spacer;
  }

  & > li {
    margin-bottom: $spacer / 2;

    @include media-breakpoint-up(lg) {
      flex: 1 1 0;
      margin-bottom: 0;
    }
  }

  &__category-icon {
    color: var(--accent-color);
  }

  &__emphase {
    font-size: 20px;
    font-weight: 700;
  }
}
</style>
