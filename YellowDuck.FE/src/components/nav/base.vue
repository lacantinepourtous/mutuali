<template>
  <b-navbar class="gray-lighter nav-base" variant="light" :aria-label="ariaTitle || null">
    <b-navbar-brand :to="{ name: $consts.urls.URL_ROOT }">
      <span class="nav-base__logo">
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
  props: {
    ariaTitle: {
      type: String,
      default: ""
    }
  },
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
  width: 100%;

  &__logo {
    background: url("~@/assets/logos/logo-mutuali.svg") no-repeat;
    background-size: auto 100%;
    display: block;
    height: 28px;
    width: 107px;

    @include media-breakpoint-down(xs) {
      width: 28px;
    }
  }
}
</style>
