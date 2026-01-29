<template>
  <component 
    :is="isUnavailable ? 'div' : 'router-link'" 
    class="conversation-snippet" 
    :class="{ 'conversation-snippet--unavailable': isUnavailable }"
    :to="isUnavailable ? null : { name: $consts.urls.URL_CONVERSATION_DETAIL, params: { id: this.id } }"
  >
    <div class="conversation-snippet__pic">
      <img
        v-if="adGallery && adGallery[0]"
        class="conversation-snippet__img"
        :src="`${adGallery[0].src}?mode=crop&width=64&height=64`"
        :alt="adGallery[0].alt ? adGallery[0].alt : ''"
      />
    </div>
    <div class="conversation-snippet__main">
      <div class="conversation-snippet__head">
        <div v-if="unread" class="conversation-snippet__notification"></div>
        <h2 v-if="otherParticipantName" class="conversation-snippet__title h6 font-family-base m-0">
          {{ otherParticipantName }}
        </h2>
        <p v-if="lastMessageDate" class="ml-auto mb-0 text-primary text-nowrap small">
          {{ lastMessageDate }}
        </p>
      </div>
      <div v-if="lastMessageComputed" class="text-truncate small mt-1" :class="{ 'font-weight-bolder': unread }">
        {{ lastMessageComputed }}
      </div>
      <p v-if="adTitle" class="conversation-snippet__ad-title small mt-2 mb-0">
        <svg class="conversation-snippet__ad-title-icon" width="18" height="41" viewBox="0 0 18 41" fill="currentColor" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
          <path fill-rule="evenodd" clip-rule="evenodd" d="M8.60552 40.7989C2.88217 32.8965 0 22.2477 0 8.69393C0 3.89609 3.85199 0.0137671 8.60552 0C13.359 0.0137671 17.211 3.89609 17.211 8.69393C17.211 22.2477 14.3357 32.8965 8.60552 40.7989ZM8.605 14.21C11.7006 14.21 14.21 11.7006 14.21 8.605C14.21 5.50944 11.7006 3 8.605 3C5.50944 3 3 5.50944 3 8.605C3 11.7006 5.50944 14.21 8.605 14.21Z" />
        </svg>
        {{ adTitle }}
      </p>
    </div>
    <div v-if="isUnavailable" class="conversation-snippet__unavailable-overlay">
      <small class="text-muted">{{ $t('conversation.unavailable') }}</small>
    </div>
  </component>
</template>

<script>
import dayjs from "dayjs";

import i18nHelpers from "@/helpers/i18n";

import { SHORT_MONTH_DATE, SHORT_HOUR_MINUTE } from "@/consts/formats";
import { TWILIO_EVENT_MESSAGE_ADDED } from "@/consts/twilio";

import TwilioService from "@/services/twilio";

import EventBus from "@/helpers/event-bus";

import debounce from "@/helpers/debounce";

const ONE_MINUTE = 1000 * 60;
const ONE_HOUR = ONE_MINUTE * 60;
const ONE_DAY = ONE_HOUR * 24;
const TWO_DAY = ONE_DAY * 2;

export default {
  async mounted() {
    await this.updateLastMessage();
    EventBus.$on(TWILIO_EVENT_MESSAGE_ADDED, this.onMessageAdded);
    let vm = this;
    if (this.updateLastMessageDateTimeout === null) {
      this.updateLastMessageDateTimeout = setTimeout(() => vm.updateLastMessageDate(), ONE_MINUTE);
    }
  },
  beforeDestroy() {
    EventBus.$off(TWILIO_EVENT_MESSAGE_ADDED, this.onMessageAdded);
    clearTimeout(this.updateLastMessageDateTimeout);
  },
  data() {
    return {
      lastMessage: null,
      unreadMessageCount: 0,
      lastMessageDate: "",
      updateLastMessageDateTimeout: null
    };
  },
  props: {
    conversation: {
      type: Object,
      required: true
    },
    twilioConversation: {
      type: Object,
      required: true
    },
    userId: {
      type: String,
      required: true
    },
    isUnavailable: {
      type: Boolean,
      default: false
    }
  },
  methods: {
    updateLastMessage: async function () {
      if (this.isUnavailable) {
        // Ne pas essayer de récupérer les messages si la conversation est indisponible
        this.lastMessage = null;
        this.unreadMessageCount = 0;
        this.updateLastMessageDate();
        return;
      }
      this.lastMessage = await this.twilioConversation.getMessages();
      this.updateUnreadMessagesStatus(true);
      this.updateLastMessageDate();
    },
    onMessageAdded: function (event) {
      if (event.conversation.sid === this.conversation.sid) {
        // Since the getUnreadMessagesCount is not 100% real time, we wait 750 ms before displayed the badge
        // https://tinyurl.com/m6s482d3
        let vm = this;
        let debounceFn = debounce(() => {
          vm.updateUnreadMessagesStatus();
        }, 750);
        debounceFn();
      }
    },
    updateUnreadMessagesStatus: async function () {
      if (this.isUnavailable) {
        this.unreadMessageCount = 0;
        return;
      }
      this.unreadMessageCount = await TwilioService.getConversationUnreadMessagesCount(this.conversation.sid);
    },
    updateLastMessageDate: function () {
      if (this.twilioConversation) {
        let updatedDate = this.twilioConversation.dateUpdated;

        if (this.twilioConversation.lastMessage) {
          updatedDate = this.twilioConversation.lastMessage.dateCreated;
        }

        let diff = dayjs(Date.now()).diff(dayjs(updatedDate));
        //eslint-disable-next-line
        if (diff > TWO_DAY) {
          this.lastMessageDate = i18nHelpers.getLocalizedDate(updatedDate, SHORT_MONTH_DATE);
        } else if (diff > ONE_DAY) {
          this.lastMessageDate = this.$t("label.conversation-yesterday");
        } else if (diff > ONE_MINUTE) {
          this.lastMessageDate = i18nHelpers.getLocalizedDate(this.dateUpdated, SHORT_HOUR_MINUTE);
        } else {
          this.lastMessageDate = this.$t("label.conversation-now");
        }
      }
    }
  },
  computed: {
    id: function () {
      return this.conversation.id;
    },
    adTitle: function () {
      return this.conversation.ad.translationOrDefault.title;
    },
    adGallery: function () {
      return this.conversation.ad.gallery;
    },
    unread: function () {
      return this.unreadMessageCount === null || this.unreadMessageCount > 0;
    },
    otherParticipantName: function () {
      let otherParticipant = this.conversation.participants.find((x) => x.user !== null && x.user.id !== this.userId);
      return otherParticipant !== null ? otherParticipant.user.profile.publicName : "";
    },
    lastMessageComputed: function () {
      if (this.isUnavailable) {
        return this.$t('conversation.unavailable-message');
      }
      if (this.lastMessage) {
        if (this.lastMessage.items.length > 0) {
          return this.lastMessage.items.last().body;
        }
      }
      return "";
    }
  },
  watch: {
    twilioConversation: {
      handler(val) {
        this.updateLastMessage();
      },
      deep: true
    }
  }
};
</script>

<style lang="scss">
.conversation-snippet {
  display: flex;
  width: 100%;

  &:hover {
    text-decoration: none;
    color: inherit;

    .conversation-snippet__title {
      text-decoration-color: currentColor;
    }
  }

  &__pic {
    background: $secondary;
    display: block;
    flex: 0 0 auto;
    height: $spacer * 2;
    width: $spacer * 2;
    margin-right: $spacer;
  }

  &__img {
    display: block;
    height: 100%;
    width: 100%;
    object-fit: cover;
    object-position: center;
  }

  &__main {
    display: block;
    flex: 0 0 auto;
    width: calc(100% - #{$spacer * 3});
  }

  &__head {
    align-items: center;
    display: flex;
    width: 100%;
  }

  &__notification {
    background: $info;
    border-radius: $spacer;
    display: block;
    flex: 0 0 auto;
    height: $spacer / 2;
    width: $spacer / 2;
    margin-right: $spacer / 2;
  }

  &__ad-title {
    display: flex;
    align-items: center;
    column-gap: $spacer / 2;

    &-icon {
      color: var(--accent-color);
      width: 8px;
    }
  }

  &__title {
    transition: color 0.1s ease-in-out, text-decoration 0.2s ease-in-out;
    text-decoration: underline;
    text-underline-offset: 2px;
    text-decoration-thickness: 2px;
    text-decoration-color: transparent;
  }

  &--unavailable {
    opacity: 0.5;
    cursor: not-allowed;
    position: relative;

    &:hover {
      color: inherit;
      text-decoration: none;

      .conversation-snippet__title {
        text-decoration-color: transparent;
      }
    }
  }

  &__unavailable-overlay {
    position: absolute;
    top: 50%;
    right: $spacer;
    transform: translateY(-50%);
    background: rgba(255, 255, 255, 0.9);
    padding: $spacer / 4 $spacer / 2;
    border-radius: $border-radius-sm;
    border: 1px solid $border-color;
  }
}
</style>
