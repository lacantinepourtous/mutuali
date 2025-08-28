<template>
  <b-navbar-nav>
    <b-nav-text v-b-toggle.conversation class="h5 mb-0 px-2">
      <span class="sr-only">
        <span class="when-open">{{ $t("sr.close") }}</span>
        <span class="when-closed">{{ $t("sr.open") }}</span>
        <span>{{ $t("sr.user-menu-toggle") }}</span>
      </span>
      <b-icon-info-circle aria-hidden="true" />
    </b-nav-text>
    <b-sidebar sidebar-class="border-top bg-white" id="conversation" right backdrop no-header>
      <b-list-group v-if="otherParticipantProfile" flush class="border-bottom text-primary">
        <b-list-group-item class="p-0" disabled>
          <user-profile-snippet class="user-profile-snippet" :id="otherParticipantProfile.id" :has-link="false" no-border :showUnderline="false" />
        </b-list-group-item>
        <b-list-group-item :to="{ name: $consts.urls.URL_USER_PROFILE_DETAIL, params: { id: otherParticipantProfile.id } }">{{
          $t("btn.conversation-sidebar-show-profile", { name: otherParticipantProfile.publicName })
        }}</b-list-group-item>
      </b-list-group>

      <div class="sidebar__rating-buttons" v-if="canRate">
        <b-button variant="link" class="sidebar__rating-button" @click="rate">
          <b-icon-star-half class="mr-2" aria-hidden="true"></b-icon-star-half>
          {{ $t("conversation.btn-rate") }}
        </b-button>
      </div>

      <a
        class="sidebar__report"
        :href="
          `mailto:${contactUsEmail}?subject=${$t('email.report-user-subject')}&body=${$t('email.report-user-body', {
            url: urlForReport
          })}`
        "
        >{{ $t("btn.report-user-ad") }}</a
      >
    </b-sidebar>
  </b-navbar-nav>
</template>

<script>
import UserProfileSnippet from "@/components/user-profile/snippet";
import { URL_USER_PROFILE_DETAIL } from "@/consts/urls";
import { VUE_APP_MUTUALI_CONTACT_MAIL } from "@/helpers/env";

export default {
  components: { UserProfileSnippet },
  props: {
    conversationId: {
      type: String,
      required: false,
      default: ""
    },
    otherParticipantId: {
      type: String,
      required: false,
      default: ""
    }
  },
  methods: {
    rate() {
      if (this.conversation) {
        this.$router.push({
          name: this.$consts.urls.URL_RATE,
          params: { 
            id: this.conversation.id
          }
        });
      }
    }
  },
  computed: {
    contactUsEmail: function() {
      return VUE_APP_MUTUALI_CONTACT_MAIL;
    },
    otherParticipantProfile: function() {
      if (this.userProfile) {
        return this.userProfile;
      } else if (!this.conversation || !this.me) {
        return null;
      }
      let otherParticipant = this.conversation.participants.find((x) => x.user !== null && x.user.id !== this.me.id);
      return otherParticipant ? otherParticipant.user.profile : null;
    },
    urlForReport: function() {
      if (this.otherParticipantProfile !== null) {
        return encodeURI(
          `${window.location.protocol}//${window.location.hostname}${
            this.$router.resolve({
              name: URL_USER_PROFILE_DETAIL,
              params: { id: this.otherParticipantProfile.id }
            }).href
          }`
        );
      }

      return "";
    },
    canRate: function() {
      return this.otherParticipantProfile && this.conversation && this.conversation.ad;
    }
  },
  apollo: {
    me: {
      query() {
        return this.$options.query.Me;
      }
    },
    userProfile: {
      query() {
        return this.$options.query.OtherParticipantProfile;
      },
      skip() {
        return this.otherParticipantId === "";
      },
      variables() {
        return {
          id: this.otherParticipantId
        };
      }
    },
    conversation: {
      query() {
        return this.$options.query.ConversationById;
      },
      skip() {
        return this.conversationId === "";
      },
      variables() {
        return {
          id: this.conversationId
        };
      }
    }
  }
};
</script>

<graphql>
query ConversationById($id: ID!) {
  conversation(id: $id) {
    id
    participants {
      id
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
    ad {
      id
    }
  }
}

query Me {
  me {
    id
  }
}

query OtherParticipantProfile($id: ID!) {
  userProfile(id: $id) {
    id
    publicName
  }
}
</graphql>

<style lang="scss">
.nav-return {
  .b-sidebar {
    margin-top: $nav-return-height;
    height: calc(100vh - #{$nav-return-height});
  }
  .b-sidebar-backdrop {
    margin-top: $nav-return-height;
    height: calc(100vh - #{$nav-return-height});
  }
}

.sidebar__report {
  background: url("~@/assets/icons/flag-red.svg") no-repeat 0 0;
  background-size: 18px auto;
  padding: 0 $spacer 0 calc(#{$spacer} + 8px);
  color: $red;
  position: absolute;
  bottom: 10px;
  left: 10px;
  text-decoration: underline;
  text-decoration-color: transparent;
  text-decoration-thickness: 2px;
  transition: color 0.2s ease-in-out, text-decoration 0.2s ease-in-out;

  &:hover {
    color: $red-darker;
    text-decoration-color: currentColor;
    text-decoration-thickness: 2px;
  }
}

.collapsed .when-open,
.not-collapsed .when-closed {
  display: none;
}

.sidebar__rating-buttons {
  display: flex;
  flex-direction: column;
  padding: $spacer;
  border-bottom: 1px solid $gray-200;
}

.sidebar__rating-button {
  color: $primary;
  text-align: left;
  padding: 0;
  margin-bottom: $spacer * 0.5;
  text-decoration: underline;
  text-decoration-color: transparent;
  text-decoration-thickness: 2px;
  transition: color 0.2s ease-in-out, text-decoration 0.2s ease-in-out;

  &:hover {
    color: $primary;
    text-decoration-color: currentColor;
    text-decoration-thickness: 2px;
  }

  &:last-child {
    margin-bottom: 0;
  }
}
</style>

<style lang="scss" scoped>
.user-profile-snippet {
  background-color: $green-lighter;
  padding: 16px 14px;
}
</style>
