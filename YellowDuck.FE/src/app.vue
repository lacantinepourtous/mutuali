<template>
  <div class="app">
    <conversation-notification />
    <header class="app__top" :class="{ 'app__top--app': showAppHeader }">
      <app-status />
      <skip-to-content />
      <template v-if="showAppHeader">
        <portal-target v-if="showHeaderPortal" :name="$consts.enums.PORTAL_HEADER" slim></portal-target>
        <nav-header v-else />
      </template>
      <anonymous-nav v-else />
      <notification-container />
    </header>
    <main class="app__middle" id="main">
      <router-view />
      <accept-tos />
    </main>
    <footer class="app__bottom">
      <nav-footer v-if="showAppHeader && !showHeaderPortal" />
      <anonymous-footer v-else-if="!showAppHeader" />
    </footer>
  </div>
</template>

<graphql>
query LocalShowMenu {
  app @client {
    showMenu
  }
}
</graphql>

<script>
import AnonymousNav from "@/components/anonymous/nav";
import AnonymousFooter from "@/components/anonymous/footer-bottom";
import AppStatus from "@/components/app-status";
import AcceptTos from "@/components/user-profile/accept-tos";
import ConversationNotification from "@/components/notifications/conversation";
import NotificationContainer from "@/components/notifications/notification-container";
import NavHeader from "@/components/nav/header";
import NavFooter from "@/components/nav/footer";
import SkipToContent from "@/components/nav/skip-to-content";
import { bootstrap } from "vue-gtag";
import { Wormhole } from "portal-vue";

export default {
  computed: {
    showHeaderPortal() {
      return Wormhole.hasContentFor(this.$consts.enums.PORTAL_HEADER);
    },
    showAppHeader() {
      return this.app && this.app.showMenu;
    }
  },
  apollo: {
    app: {
      query() {
        return this.$options.query.LocalShowMenu;
      }
    }
  },
  components: {
    AnonymousNav,
    AnonymousFooter,
    AppStatus,
    AcceptTos,
    NotificationContainer,
    NavHeader,
    NavFooter,
    ConversationNotification,
    SkipToContent
  },
  gqlErrors() {
    return {
      UNAUTHORIZED_ACCESS(error) {
        return this.$t("error.unauthorized");
      }
    };
  },
  mounted() {
    this.$termsFeed.onChange((acceptedLevels) => {
      if (acceptedLevels.tracking) {
        bootstrap();
      }
    });
  }
};
</script>

<style lang="scss">
html,
body {
  height: 100%;
}

.app {
  display: flex;
  flex-direction: column;
  min-height: 100%;

  &__top {
    &--app {
      position: sticky;
      top: 0;
      z-index: 3;
    }
  }

  &__top,
  &__bottom {
    display: block;
    flex: 0 0 auto;
  }

  &__middle {
    display: flex;
    flex: 1 1 auto;
  }
}
</style>
