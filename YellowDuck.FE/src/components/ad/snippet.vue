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
      >
        <template #description>
          <p v-if="price" class="text-muted mb-0 mt-1">
            {{ price }} <span v-if="priceDescription" class="text-lowercase">{{ priceDescription }}</span>
          </p>
        </template>

        <template #actions>
          <b-button
            v-if="canTransfer"
            :to="{ name: $consts.urls.URL_AD_TRANSFER, params: { id } }"
            variant="secondary"
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
            <b-button v-else @click="publishAd" variant="outline-primary" size="sm" class="mt-2 ml-2">
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
    priceDescription: String,
    price: String,
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
    isPublished: Boolean,
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
}
</style>
