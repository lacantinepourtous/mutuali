<template>
  <div class="w-100">
    <div class="section section--md mt-4 mt-md-5 mb-4">
      <h1>{{ $t("page-title.conversations-list") }}</h1>
    </div>

    <template v-if="conversations.length > 0">
      <div
        v-for="conversation in conversations"
        :key="conversation.graphql.id"
        class="section section--md my-4"
        :class="{ 'section--border-top pt-4': conversations[0] && conversation !== conversations[0] }"
      >
        <conversation-snippet :conversation="conversation.graphql" :class="getCategoryGroupByCategory(conversation.graphql.ad.category).color" :twilioConversation="conversation.twilio" :userId="userId" />
      </div>
    </template>
    <div v-else class="no-conversation">
      <img class="no-conversation__img my-5" :src="require('@/assets/ambiance/empty-conversation.svg')" alt="" />
      <p>{{ $t("conversation-list.no-conversation") }}</p>
    </div>
  </div>
</template>

<script>
import { CONTENT_LANG_FR } from "@/consts/langs";
import { TWILIO_EVENT_MESSAGE_ADDED, TWILIO_EVENT_PARTICIPANT_JOINED } from "@/consts/twilio";

import ConversationSnippet from "@/components/conversation/snippet";

import TwilioService from "@/services/twilio";

import { AdCategory } from "@/mixins/ad-category";
import EventBus from "@/helpers/event-bus";
import debounce from "@/helpers/debounce";

export default {
  mixins: [AdCategory],
  mounted() {
    EventBus.$on(TWILIO_EVENT_MESSAGE_ADDED, this.onMessageAdded);
    EventBus.$on(TWILIO_EVENT_PARTICIPANT_JOINED, this.onParticipantJoined);
  },
  beforeDestroy() {
    EventBus.$off(TWILIO_EVENT_MESSAGE_ADDED, this.onMessageAdded);
    EventBus.$off(TWILIO_EVENT_PARTICIPANT_JOINED, this.onParticipantJoined);
  },
  data() {
    return {
      conversations: []
    };
  },
  components: {
    ConversationSnippet
  },
  methods: {
    onMessageAdded: function() {
      // Since the data is not 100% real time, we wait 750 ms before updating the sort
      // https://tinyurl.com/m6s482d3
      let vm = this;
      let debounceFn = debounce(async () => {
        await vm.updateConversations();
        vm.sortConversations();
      }, 750);
      debounceFn();
    },
    onParticipantJoined: function() {
      // Since the data is not 100% real time, we wait 2 seconds before updating the sort
      // https://tinyurl.com/m6s482d3
      let vm = this;
      let debounceFn = debounce(() => {
        vm.$apollo.queries.me.refresh();
      }, 2000);
      debounceFn();
    },
    sortConversations: function() {
      this.conversations.sort((a, b) => {
        let updatedDateA = a.twilio.dateUpdated;
        if (a.twilio.lastMessage) {
          updatedDateA = a.twilio.lastMessage.dateCreated;
        }

        let updatedDateB = b.twilio.dateUpdated;
        if (b.twilio.lastMessage) {
          updatedDateB = b.twilio.lastMessage.dateCreated;
        }

        return updatedDateA < updatedDateB ? 1 : updatedDateA > updatedDateB ? -1 : 0;
      });
    },
    updateConversations: async function() {
      let conversations = [];

      await Promise.all(
        this.me.conversations.map(async (x) => {
          let twilio = await TwilioService.getConversationBySid(x.sid);
          conversations.push({ graphql: x, twilio });
        })
      );

      this.conversations = conversations;
    }
  },
  apollo: {
    me: {
      query() {
        return this.$options.query.Conversations;
      },
      fetchPolicy: "network-only",
      variables() {
        return {
          language: CONTENT_LANG_FR
        };
      },
      async result({ data }) {
        if (data) {
          await this.updateConversations();
          this.sortConversations();
        }
      }
    }
  },
  computed: {
    userId: function() {
      if (this.me) {
        return this.me.id;
      }
      return "";
    }
  }
};
</script>

<graphql>
query Conversations($language: ContentLanguage!) {
  me {
    id
    conversations {
      id
      sid
      ad {
        id
        category
        gallery {
          id
          src
          alt
        }
        translationOrDefault(language: $language) {
          id
          title
        }
      }
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
    }
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
</style>
