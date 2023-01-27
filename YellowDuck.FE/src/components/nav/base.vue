<template>
  <b-navbar class="gray-lighter nav-base" variant="light">
    <b-navbar-brand class="link-scale" :to="{ name: $consts.urls.URL_ROOT }">
      <span class="nav-base__logo" :class="{ 'nav-base__logo--condensed': isConnected }">
        <span class="sr-only">{{ $t("sr.homepage") }}</span>
      </span>
    </b-navbar-brand>
    <slot :sIsConnected="isConnected"></slot>
  </b-navbar>
</template>

<graphql>
query LocalUser {
  user @client {
    isConnected
  }
}
</graphql>

<script>
export default {
  computed: {
    isConnected: function () {
      return this.user && this.user.isConnected;
    }
  },
  apollo: {
    user: {
      query() {
        return this.$options.query.LocalUser;
      }
    }
  }
};
</script>

<style lang="scss">
.nav-base {
  min-height: $navbar-padding-y * 2 + $navbar-brand-padding-y * 2 + $navbar-brand-height;

  &__logo {
    background: url("~@/assets/logo.svg") no-repeat;
    background-size: auto 100%;
    display: block;
    height: 26px;
    width: 113px;

    @include media-breakpoint-down(xs) {
      &--condensed {
        width: 28px;
      }
    }
  }
}
</style>
