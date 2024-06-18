<template>
  <div>
    <b-modal
      v-model="acceptTosModalOpen"
      :title="$t('accept-tos.title')"
      hide-footer
      hide-header-close
      no-close-on-backdrop
      no-close-on-esc
      centered
    >
      <p>{{ $t("accept-tos.text") }}</p>
      <p>
        <span v-html="$t('text.landing-page-terms-of-use')"></span><br />
        <span v-html="$t('text.landing-page-privacy-policy')"></span>
      </p>
      <p>{{ $t("accept-tos.question") }}</p>

      <b-button @click="acceptLatestTos" variant="primary">{{ $t("accept-tos.button") }}</b-button>
      <b-button @click="logout" variant="link">{{ $t("accept-tos.logout") }}</b-button>
    </b-modal>
  </div>
</template>

<script>
import UserService from "@/services/user";
import NotificationService from "@/services/notification";
import AuthentificationService from "@/services/authentification";
export default {
  apollo: {
    user: {
      query() {
        return this.$options.query.LocalUser;
      }
    },
    me: {
      query() {
        return this.$options.query.Me;
      },
      skip() {
        return !this.user || !this.user.isConnected;
      }
    }
  },
  computed: {
    hasAcceptedLatest() {
      if (!this.me || !this.me.profile || !this.me.profile.acceptedTos) {
        return null;
      }
      return this.me.profile.acceptedTos.hasAcceptedLatest;
    },
    acceptTosModalOpen() {
      return this.hasAcceptedLatest === false;
    }
  },
  methods: {
    acceptLatestTos() {
      UserService.acceptTos(this.me.profile.acceptedTos.latestVersionAvailable)
        .then(() => {
          this.$apollo.queries.me.refetch();
        })
        .catch((error) => {
          this.$apollo.queries.me.refetch();
          NotificationService.error(error);
        });
    },
    async logout() {
      await AuthentificationService.logout();
    }
  }
};
</script>

<graphql>
query LocalUser {
  user @client {
    isConnected
  }
}

query Me {
  me {
    id
    type
    profile {
      id
      ... on UserProfileGraphType {
        acceptedTos {
          id
          hasAcceptedLatest
          latestVersionAvailable
        }
      }
    }
  }
}
</graphql>
