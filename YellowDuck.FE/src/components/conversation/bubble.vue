<template>
  <div
    class="conversation-bubble"
    :class="{
      'conversation-bubble--current': isCurrentUser,
      'conversation-bubble--system': isSystem,
      'conversation-bubble--create': isCreateConversation
    }"
  >
    <div class="conversation-bubble__top" :class="{ 'conversation-bubble__top--system green': isSystem }">
      <p class="small mb-1" v-html="bodyWithLink"></p>
    </div>

    <div class="conversation-bubble__bottom" :class="{ 'conversation-bubble__bottom--system green-lighter': isSystem }">
      <p v-if="hasActions" class="text-light font-weight-bold">
        <span v-if="isRatingRequest && contractId" class="conversation-bubble__link">
          <b-icon icon="star-half" class="mr-2" aria-hidden="true"></b-icon>
          <router-link :to="{ name: $consts.urls.URL_CONTRACT_RATING, params: { id: this.contractId } }">
            {{ $t("conversation.btn-rate-transaction") }}
          </router-link>
        </span>
        <span v-if="isContractCreatedOrUpdated && contractId" class="conversation-bubble__link">
          <b-icon icon="file-earmark-check" class="mr-2" aria-hidden="true"></b-icon>
          <router-link :to="{ name: $consts.urls.URL_CONTRACT_DETAIL, params: { id: contractId } }">
            {{ $t("btn.contract-detail") }}</router-link
          >
        </span>
        <span v-if="isCreateConversation" class="conversation-bubble__link text-white h6 font-family-base">
          <b-icon-reply-fill class="mr-2 mt-1 lead" aria-hidden="true"></b-icon-reply-fill>
          <b-button
            @click="onCreateConversation"
            variant="link"
            class="conversation-bubble__link-btn p-0 text-left text-white font-weight-bold text-decoration-underline"
          >
            {{ $t("btn.create-conversation-auto-message") }}
          </b-button>
        </span>
      </p>
      <p class="small text-right m-0" :class="{ 'text-muted': isCurrentUser }">
        <img class="mt-n1 mr-1" :src="messageIcon(isSystem)" :alt="$t('alt.message-sent-at')" />
        {{ messageDate }}
      </p>
    </div>
  </div>
</template>

<script>
import linkifyHtml from "linkifyjs/html";
import { SHORT_HOUR_MINUTE } from "@/consts/formats";
import { RATING_REQUEST, CONTRACT_CREATED, CONTRACT_UPDATED, CREATE_CONVERSATION } from "@/consts/message-actions";
import i18nHelpers from "@/helpers/i18n";

export default {
  props: {
    body: {
      type: String,
      required: true
    },
    dateUpdated: {
      type: Date,
      required: true
    },
    isCurrentUser: {
      type: Boolean,
      required: true
    },
    isSystem: {
      type: Boolean,
      required: true
    },
    attributes: {
      type: Object,
      required: false,
      default() {
        return {};
      }
    },
    contractId: {
      type: String,
      required: false,
      default: ""
    }
  },
  data() {
    return {
      handledActions: [RATING_REQUEST, CONTRACT_CREATED, CONTRACT_UPDATED, CREATE_CONVERSATION]
    };
  },
  computed: {
    bodyWithLink: function() {
      return linkifyHtml(this.body, {
        defaultProtocol: "https",
        target: {
          url: "_blank"
        }
      });
    },
    messageDate: function() {
      return i18nHelpers.getLocalizedDate(this.dateUpdated, SHORT_HOUR_MINUTE);
    },
    hasActions: function() {
      return "Action" in this.attributes && this.handledActions.includes(this.attributes.Action);
    },
    isRatingRequest: function() {
      return this.hasActions && this.attributes.Action === RATING_REQUEST;
    },
    isCreateConversation: function() {
      return this.hasActions && this.attributes.Action === CREATE_CONVERSATION;
    },
    isContractCreatedOrUpdated: function() {
      let result = this.hasActions && [CONTRACT_CREATED, CONTRACT_UPDATED].includes(this.attributes.Action);
      if (result && (this.contractId === "" || this.contractId === null)) {
        this.$emit("loadContractId");
      }
      return result;
    }
  },
  methods: {
    messageIcon: function(isSystem) {
      return isSystem ? require("@/assets/icons/mutuali.svg") : require("@/assets/icons/user.svg");
    },
    onCreateConversation: function() {
      this.$emit("createConversation");
    }
  }
};
</script>

<style lang="scss">
.conversation-bubble {
  display: flex;
  flex: 0 1 auto;
  flex-direction: column;
  border: 1px solid $gray-200;
  border-radius: $btn-border-radius;
  margin: $spacer 0;
  max-width: 75%;

  &--current {
    border-color: $yellow;
    margin-left: auto;
  }

  &--system {
    background-color: $primary;
    border-width: 0;
    border-radius: $border-radius-md;
    color: $white;
    margin-left: auto;
    margin-right: auto;
    overflow: hidden;
    width: 100%;
  }

  &--create {
    margin-top: auto;
    margin-bottom: 0;
  }

  &:not(&--current) {
    margin-right: auto;
  }

  &--current + &--current,
  &:not(&--current) + &:not(&--current) {
    margin-top: $spacer / -2;
  }

  &__top {
    padding: $spacer * 0.75 $spacer 0;

    &--system {
      padding: $spacer * 0.75 $spacer;
    }
  }

  &__bottom {
    padding: 0 $spacer $spacer * 0.75;

    &--system {
      background-color: rgba(255, 255, 255, 0.2);
      padding: $spacer * 0.75 $spacer;
    }
  }

  &__link {
    display: flex;
    line-height: 1.2;
  }

  &__link-btn {
    &:hover,
    &:focus {
      color: $gray-100 !important;
    }
  }
}
</style>
