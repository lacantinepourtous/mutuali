<template>
  <b-navbar-nav v-if="otherParticipantProfile">
    <b-nav-item v-if="canRate">
      <b-button class="line-height-none" variant="secondary" @click="rate">
        <b-icon-star-half variant="warning" class="mr-1" aria-hidden="true"></b-icon-star-half>
        <span class="sr-only-sm">{{ btnRateLabel }}</span>
      </b-button>
    </b-nav-item>
    <b-nav-item>
      <b-button class="line-height-none" variant="secondary" v-b-toggle.conversation>
      <span class="sr-only">
        <span class="when-open">{{ $t("sr.close") }}</span>
        <span class="when-closed">{{ $t("sr.open") }}</span>
        <span>{{ $t("sr.user-menu-toggle") }}</span>
      </span>
      <b-icon-person-lines-fill class="mr-1" aria-hidden="true" />
      <span class="sr-only-sm">{{ otherParticipantProfile.publicName }}</span>
      </b-button>
    </b-nav-item>
    <b-sidebar sidebar-class="border-top bg-white" id="conversation" right backdrop no-header>
      <b-list-group flush class="border-bottom text-primary">
        <b-list-group-item class="p-0" disabled>
          <user-profile-snippet class="user-profile-snippet" :id="otherParticipantProfile.id" :has-link="false" no-border :showUnderline="false" />
        </b-list-group-item>
        <b-list-group-item :to="{ name: $consts.urls.URL_USER_PROFILE_DETAIL, params: { id: otherParticipantProfile.id } }">{{
          $t("btn.conversation-sidebar-show-profile", { name: otherParticipantProfile.publicName })
        }}</b-list-group-item>
      </b-list-group>

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
      return this.userProfile;
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
    },
    userIsOwner: function() {
      return this.conversation.ad.user.id === this.me.id;
    },
    adHasBeenRated: function() {
      return this.conversation.ad.adRating && this.conversation.ad.adRating.id ? true : false;
    },
    ownerHasBeenRatedByMe: function() {
      if (this.userIsOwner) {
        return false;
      }
      return this.conversation.ad.user.userRatings.some((rating) => rating.raterUser.id === this.me.id);
    },
    userHasBeenRatedByMe: function() {
      const otherParticipant = this.conversation.participants.find((x) => x.user !== null && x.user.id !== this.me.id);
      if  (!otherParticipant || !otherParticipant.user.userRatings || !this.userIsOwner) {
        return false;
      }
      return otherParticipant.user.userRatings.some((rating) => rating.raterUser.id === this.me.id);
    },
    btnRateLabel: function() {
      let label = "";
      if (this.adHasBeenRated || (this.userIsOwner && this.userHasBeenRatedByMe)) {
        label = this.$t("conversation.btn-rerate");
      } else {
        label = this.$t("conversation.btn-rate");
      }
      if (this.userIsOwner) {
        label += " " + this.$t("conversation.btn-rate-user");
      } else {
        label += " " + this.$t("conversation.btn-rate-ad");
      }
      return label;
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
        userRatings {
          id
          raterUser {
            id
          }
        }
      }
    }
    ad {
      id
      user {
        id
        userRatings {
          id
          raterUser {
            id
          }
        }
      }
    }
    adRating {
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
</style>

<style lang="scss" scoped>
.user-profile-snippet {
  background-color: $green-lighter;
  padding: 16px 14px;
}
</style>
