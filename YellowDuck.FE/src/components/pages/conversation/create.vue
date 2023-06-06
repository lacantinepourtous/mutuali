<template>
  <div v-if="ad" class="conversation-detail">
    <portal :to="$consts.enums.PORTAL_HEADER">
      <div>
        <nav-return :aria-title="$t('sr.conversation-nav')" :to="{ name: $consts.urls.URL_LIST_CONVERSATION }">
          <conversation-sidebar :other-participant-id="otherParticipantId" />
        </nav-return>
        <ad-snippet
          :id="adId"
          :title="adTitle"
          :image="adImage"
          :price="adPrice"
          :priceDescription="adPriceDescription"
          :organization="adOrganization"
          snippet-is-link
        />
      </div>
    </portal>
    <template>
      <div class="conversation-detail__bubble">
        <div class="conversation-detail__bubble-section section section--md my-3 d-flex flex-column">
          <conversation-bubble
            :body="conversationInitialBody"
            :is-current-user="false"
            :is-system="true"
            :attributes="conversationInitialAttributes"
            :date-updated="new Date(Date.now())"
            @createConversation="createConversationWithAutoMessage"
          />
        </div>
      </div>
      <div class="conversation-detail__send">
        <div class="section section--md">
          <send-message-form @sendMessage="createConversation" :other-participant-name="otherParticipantName" />
        </div>
      </div>
    </template>
  </div>
</template>

<script>
import ConversationBubble from "@/components/conversation/bubble";
import NavReturn from "@/components/nav/return";
import AdSnippet from "@/components/ad/snippet";
import SendMessageForm from "@/components/conversation/form";
import ConversationSidebar from "@/components/conversation/sidebar";

import ConversationService from "@/services/conversation";
import TwilioService from "@/services/twilio";

import { URL_CONVERSATION_DETAIL } from "@/consts/urls";
import { CONTENT_LANG_FR } from "@/consts/langs";
import { CREATE_CONVERSATION } from "@/consts/message-actions";

export default {
  data() {
    return {
      creatingConversation: false
    };
  },
  components: {
    ConversationBubble,
    AdSnippet,
    NavReturn,
    SendMessageForm,
    ConversationSidebar
  },
  methods: {
    createConversation: async function (message) {
      if (!this.creatingConversation) {
        this.creatingConversation = true;
        let result = await ConversationService.createConversation(this.adId);
        let conversation = result.data.createConversation.conversation;

        await TwilioService.addMessageToConversation(conversation.sid, message);

        this.$router.replace({ name: URL_CONVERSATION_DETAIL, params: { id: conversation.id } });
      }
    },
    createConversationWithAutoMessage: async function () {
      this.createConversation(this.$t("btn.create-conversation-auto-message"));
    }
  },
  computed: {
    adId: function () {
      return this.$route.params.adId.split("-").last();
    },
    adTitle: function () {
      return this.ad.translationOrDefault.title;
    },
    adImage: function () {
      return this.ad.gallery[0];
    },
    adPrice: function () {
      return this.ad.priceToBeDetermined ? this.$t("price.toBeDetermined") : this.$format.formatMoney(this.ad.price);
    },
    adOrganization: function () {
      return this.ad.organization;
    },
    adPriceDescription: function () {
      return this.ad.priceToBeDetermined ? "" : this.ad.translationOrDefault.priceDescription;
    },
    otherParticipantId: function () {
      return this.ad.user.profile.id;
    },
    otherParticipantName: function () {
      return this.ad.user.profile.publicName;
    },
    conversationInitialAttributes: function () {
      return { Action: CREATE_CONVERSATION };
    },
    conversationInitialBody: function () {
      return this.$t("text.initial-conversation-body", { name: this.otherParticipantName });
    }
  },
  apollo: {
    ad: {
      query() {
        return this.$options.query.AdById;
      },
      variables() {
        return {
          id: this.adId,
          language: CONTENT_LANG_FR
        };
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
      title
      priceDescription
      conditions
    }
    gallery {
      id
      src
      alt
    }
    user {
      id
      profile {
        id
        ... on UserProfileGraphType {
          publicName
        }
      }
    }
    price
    priceToBeDetermined
    organization
  }
}
</graphql>

<style lang="scss">
$no-conversation-img-width: 170px;

.no-conversation {
  align-items: center;
  display: flex;
  flex-direction: column;
  justify-content: center;

  &__img {
    width: $no-conversation-img-width;
  }
}

.conversation-detail {
  display: flex;
  flex: 1 1 auto;
  flex-direction: column;
  width: 100%;

  &__bubble {
    display: block;
    flex: 1 1 auto;
    width: 100%;
  }

  &__bubble-section {
    min-height: calc(100% - #{$spacer} * 2);
  }

  &__send {
    background: $body-bg;
    border-top: 1px solid $gray-200;
    display: block;
    flex: 0 0 auto;
    padding: $spacer 0;
    position: sticky;
    bottom: 0;
    width: 100%;
  }
}
</style>
