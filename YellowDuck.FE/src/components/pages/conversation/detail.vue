<template>
  <div class="conversation-detail">
    <portal :to="$consts.enums.PORTAL_HEADER">
      <div>
        <nav-return :aria-title="$t('sr.conversation-nav')" :to="{ name: $consts.urls.URL_LIST_CONVERSATION }">
          <conversation-sidebar :conversation-id="conversationId" :other-participant-id="otherParticipantId" />
        </nav-return>
        <ad-snippet
          v-if="conversation"
          :id="adId"
          :title="adTitle"
          title-tag="h1"
          :image="adImage"
          :organization="adOrganization"
          :conversation="conversation"
          :conversationId="conversationId"
          :canCreateContract="canCreateContract"
          :isAccountOnboardingComplete="isAccountOnboardingComplete"
          :isLocked="adIsLocked"
          :isPublished="adIsPublished"
          smallTitle
          noWrapTitle
          snippetIsLink
        />
      </div>
    </portal>
    <template v-if="conversation">
      <div class="conversation-detail__bubble">
        <div v-for="day in messagesByDay" :key="day.date.toString()" class="section section--md my-3 d-flex flex-column">
          <conversation-date :date="day.date" />
          <conversation-bubble
            v-for="message in day.messages"
            :key="message.sid"
            :body="message.body"
            :medias="message.attachedMedia"
            :contract-id="contractId"
            :conversation-id="conversationId"
            :attributes="message.attributes"
            :date-updated="message.dateUpdated"
            :is-current-user="isCurrentUser(message)"
            :is-system="isSystemUser(message)"
            @loadContractId="loadContractId"
          />
        </div>
      </div>
      <div class="conversation-detail__send">
        <div class="section section--md">
          <send-message-form :conversation-sid="conversationSid" :conversation-id="conversationId" :other-participant-name="otherParticipantName" />
        </div>
      </div>
    </template>
  </div>
</template>

<script>
import NavReturn from "@/components/nav/return";
import ConversationBubble from "@/components/conversation/bubble";
import ConversationDate from "@/components/conversation/date";
import ConversationSidebar from "@/components/conversation/sidebar";
import AdSnippet from "@/components/ad/snippet";
import SendMessageForm from "@/components/conversation/form";

import { CONTENT_LANG_FR } from "@/consts/langs";
import { TWILIO_EVENT_MESSAGE_SENT, TWILIO_EVENT_MESSAGE_ADDED } from "@/consts/twilio";

import TwilioService from "@/services/twilio";
import { getContractIdByConversationId } from "@/services/contract";

import EventBus from "@/helpers/event-bus";

export default {
  mounted() {
    EventBus.$on(TWILIO_EVENT_MESSAGE_SENT, this.onMessageSent);
    EventBus.$on(TWILIO_EVENT_MESSAGE_ADDED, this.onMessageAdded);
  },
  beforeDestroy() {
    EventBus.$off(TWILIO_EVENT_MESSAGE_SENT, this.onMessageSent);
    EventBus.$off(TWILIO_EVENT_MESSAGE_ADDED, this.onMessageAdded);
  },
  components: {
    ConversationBubble,
    ConversationDate,
    ConversationSidebar,
    AdSnippet,
    NavReturn,
    SendMessageForm
  },
  data() {
    return {
      twilioConversation: {},
      messagesByDay: [],
      conversationContractId: ""
    };
  },
  computed: {
    adId: function () {
      return this.conversation.ad.id;
    },
    adTitle: function () {
      return this.conversation.ad.translationOrDefault.title;
    },
    adImage: function () {
      return this.conversation.ad.gallery[0];
    },
    adOrganization: function () {
      return this.conversation.ad.organization;
    },
    adOwnerId: function () {
      return this.conversation.ad.user.id;
    },
    adIsLocked: function () {
      return this.conversation.ad.locked;
    },
    adIsPublished: function () {
      return this.conversation.ad.isPublish;
    },
    conversationId: function () {
      return this.$route.params.id.split("-").last();
    },
    conversationSid: function () {
      return this.conversation.sid;
    },
    otherParticipant: function () {
      if (!this.conversation || !this.me) {
        return null;
      }
      let otherParticipant = this.conversation.participants.find((x) => x.user !== null && x.user.id !== this.me.id);
      return otherParticipant !== null ? otherParticipant : null;
    },
    otherParticipantId: function () {
      return this.otherParticipant !== null ? this.otherParticipant.user.profile.id : "";
    },
    otherParticipantName: function () {
      return this.otherParticipant !== null ? this.otherParticipant.user.profile.publicName : "";
    },
    canCreateContract: function () {
      return this.me ? this.adOwnerId === this.me.id : false;
    },
    haveContract: function () {
      return this.conversation.contract !== null;
    },
    contractId: function () {
      return this.haveContract ? this.conversation.contract.id : this.conversationContractId;
    },
    isAccountOnboardingComplete: function () {
      let account = this.me ? this.me.stripeAccount : null;
      return account !== null ? account.accountOnboardingComplete : false;
    }
  },
  methods: {
    loadContractId: async function () {
      this.conversationContractId = await getContractIdByConversationId(this.conversationId);
    },
    isCurrentUser: function (message) {
      let participant = this.conversation.participants.find((x) => x.sid === message.participantSid);
      if (participant.user !== null) {
        return participant.user.id === this.me.id;
      }
      return false;
    },
    isSystemUser: function (message) {
      let participant = this.conversation.participants.find((x) => x.sid === message.participantSid);
      return participant.user === null;
    },
    onMessageSent: async function () {
      await this.scrollBottom();
      await this.refreshMessages();
      this.scrollBottom();
    },
    onMessageAdded: async function (event) {
      await this.refreshMessages();
      this.scrollBottom();
    },
    refreshMessages: async function () {
      await TwilioService.setAllMessagesReadOnConversation(this.conversationSid);
      let messages = await TwilioService.getConversationMessages(this.conversationSid);
      this.messagesByDay = await this.getMessagesByDay(messages);
    },
    getMessagesByDay: async function (messages) {
      let result = [];
      let currentDate = null;
      let day = null;

      for (let i = 0; i < messages.length; i++) {
        let message = messages[i];
        if (
          currentDate === null ||
          currentDate.getFullYear() !== message.dateUpdated.getFullYear() ||
          currentDate.getDate() !== message.dateUpdated.getDate() ||
          currentDate.getMonth() !== message.dateUpdated.getMonth()
        ) {
          currentDate = message.dateUpdated;
          day = { date: currentDate, messages: [] };
          result.push(day);
        }
        for (let media of message.attachedMedia) {
          media.temporaryUrl = await media.getContentTemporaryUrl();
        }
        day.messages.push(message);
      }

      return result;
    },
    scrollBottom: async function () {
      new Promise((resolve, reject) => {
        // Timeout because of render race condition
        setTimeout(() => {
          window.scrollTo(0, document.body.scrollHeight);
          resolve();
        }, 0);
      });
    }
  },
  apollo: {
    me: {
      query() {
        return this.$options.query.Me;
      }
    },
    conversation: {
      query() {
        return this.$options.query.ConversationById;
      },
      variables() {
        return {
          id: this.conversationId,
          language: CONTENT_LANG_FR
        };
      },
      async result({ data }) {
        if (data) {
          this.twilioConversation = await TwilioService.getConversationBySid(data.conversation.sid);
          await this.refreshMessages();
          this.scrollBottom();
        }
      }
    }
  }
};
</script>

<graphql>
query ConversationById($id: ID!, $language: ContentLanguage!) {
  conversation(id: $id) {
    id
    sid
    participants {
      id
      sid
      user {
        id
        profile {
          id
          ... on UserProfileGraphType {
            publicName
          }
        }
      }
    }
    contract {
      id
      status
    }
    ad {
      id
      gallery {
        id
        src
        alt
      }
      translationOrDefault(language: $language) {
        id
        title
      }
      user {
        id
      }
      organization
      isPublish
      locked
    }
  }
}

query Me {
  me {
    id
    stripeAccount {
      id
      accountOnboardingComplete
      accountOnboardingLink
    }
  }
}
</graphql>

<style lang="scss">
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
