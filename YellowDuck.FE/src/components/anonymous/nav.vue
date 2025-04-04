<template>
  <div class="anonymous-nav">
    <div class="anonymous-nav__content section section--xl border-b">
      <b-navbar toggleable="lg" type="light" class="px-0 pt-4 pb-3">
        <b-navbar-brand class="d-flex align-items-center mr-lg-4" :to="{ name: $consts.urls.URL_ROOT }">
          <b-img class="anonymous-nav__logo" alt="Mutuali" :src="require('@/assets/logos/logo-mutuali-cibim.svg')"></b-img>
        </b-navbar-brand>

        <b-navbar-toggle class="anonymous-nav__toggle" target="nav-collapse"></b-navbar-toggle>

        <b-collapse id="nav-collapse" is-nav>
          <b-navbar-nav class="mb-3 mb-lg-0">
            <b-nav-item class="anonymous-nav__item mr-lg-3" :to="{ name: $consts.urls.URL_ABOUT }">{{
              $t("nav.about")
            }}</b-nav-item>
            <b-nav-item-dropdown class="anonymous-nav__item mr-lg-3" :text="$t('nav.how-it-work')">
              <b-dropdown-item :to="{ name: $consts.urls.URL_SHARING_EQUIPMENT }">{{$t("nav.how-it-work.post-ad")}}</b-dropdown-item>
              <b-dropdown-item :to="{ name: $consts.urls.URL_LOOKING_FOR_EQUIPMENT }">{{$t("nav.how-it-work.view-ad")}}</b-dropdown-item>
              <b-dropdown-item :to="{ name: $consts.urls.URL_ROOT, hash: '#introVideo' }">{{$t("nav.how-it-work.video")}}</b-dropdown-item>
              <b-dropdown-item href="https://mutuali.notion.site/mutuali/Foire-aux-questions-0b482ef447694f73926b495675f1685e" target="_blank">{{$t("nav.how-it-work.faq")}}</b-dropdown-item>
            </b-nav-item-dropdown>
            <b-nav-item class="anonymous-nav__item" :to="{ name: $consts.urls.URL_CONTACT }">{{
              $t("nav.contact-us")
            }}</b-nav-item>
          </b-navbar-nav>

          <!-- Right aligned nav items -->
          <b-navbar-nav class="d-flex flex-row ml-auto">
            <template v-if="!isConnected">
              <li class="nav-item mr-2">
                <router-link class="btn btn-outline-primary" :to="{ name: $consts.urls.URL_LOGIN }">
                  {{ this.$t("btn.landing-connect") }}
                </router-link>
              </li>
              <li class="nav-item">
                <router-link class="btn btn-admin" :to="{ name: $consts.urls.URL_USER_SUBSCRIBE }">
                  {{ this.$t("btn.landing-create-account") }}
                </router-link>
              </li>
            </template>
            <template v-else>
              <li class="nav-item">
                <router-link
                  class="btn btn-primary"
                  :to="{
                    name: $consts.urls.URL_USER_PROFILE_DETAIL,
                    params: { id: userProfileId }
                  }"
                >
                  <b-icon-person-circle aria-hidden="true"></b-icon-person-circle>
                  {{ userProfilePublicName }}
                </router-link>
              </li>
            </template>
            <b-nav-item class="anonymous-nav__item ml-2" @click="changeLang">
              <span aria-hidden="true">{{ $t("btn.change-lang-shortcut") }}</span>
              <span class="sr-only">{{ $t("btn.change-lang") }}</span>
            </b-nav-item>
          </b-navbar-nav>
        </b-collapse>
      </b-navbar>
    </div>
  </div>
</template>

<script>
import i18nHelpers from "@/helpers/i18n";

export default {
  computed: {
    isConnected: function () {
      return this.user && this.user.isConnected;
    },
    userProfileId: function () {
      return this.me ? this.me.profile.id : "";
    },
    userProfilePublicName: function () {
      return this.me ? this.me.profile.publicName : "";
    }
  },
  apollo: {
    me: {
      query() {
        return this.$options.query.Me;
      },
      skip() {
        return !this.isConnected;
      }
    },
    user: {
      query() {
        return this.$options.query.LocalUser;
      }
    }
  },
  methods: {
    changeLang: function () {
      i18nHelpers.changeLang();
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
    profile {
      id
      ... on UserProfileGraphType {
        publicName
      }
    }
  }
}
</graphql>

<style lang="scss">
$logo-width: 144px;
.anonymous-nav {
  &__content {
    border-bottom: solid 2px $green-lighter;
  }

  &__logo {
    min-width: $logo-width;
  }

  &__toggle {
    border: 0;
    outline-color: $gray-600;
  }

  .navbar-light {
    .anonymous-nav__item {
      .nav-link {
        color: $gray-800;
        transition: color 0.2s ease-in-out, text-decoration 0.2s ease-in-out;
        text-decoration: underline;
        text-underline-offset: 2px;
        text-decoration-color: transparent;
        text-decoration-thickness: 2px;
        &:hover,
        &.router-link-active {
          text-decoration-color: $primary;
        }

        &.router-link-active {
          color: $primary;
        }
      }
    }
  }
  
}
</style>
