<template>
  <div class="gray-lighter bg-light footer-app">
    <b-nav small>
      <b-nav-item class="footer-app__nav-item navbar-nav mx-2" :to="{ name: $consts.urls.URL_LANDING }">
        <b-icon icon="question-circle" class="mr-1" aria-hidden="true"></b-icon>
        {{ $t("btn.about") }}
      </b-nav-item>
      <b-nav-item
        class="footer-app__nav-item navbar-nav mx-2"
        href="https://mutuali.notion.site/mutuali/Foire-aux-questions-0b482ef447694f73926b495675f1685e"
        target="_blank"
        >{{ $t("btn.faq") }}</b-nav-item
      >
      <b-nav-item class="footer-app__nav-item navbar-nav mx-2" :href="`mailto:${contactUsEmail}`">
        {{ $t("btn.contact-us") }}
      </b-nav-item>
      <b-nav-item class="footer-app__nav-item navbar-nav ml-auto mr-2" @click="changeLang"
        ><span class="sr-only">{{ $t("btn.change-lang") }}</span>
        <span aria-hidden>{{ $t("btn.change-lang-shortcut") }}</span></b-nav-item
      >
    </b-nav>
  </div>
</template>

<script>
import i18nHelpers from "@/helpers/i18n";
import { VUE_APP_MUTUALI_CONTACT_MAIL } from "@/helpers/env";

export default {
  methods: {
    changeLang: function () {
      i18nHelpers.changeLang();
    }
  },
  computed: {
    contactUsEmail: function () {
      return VUE_APP_MUTUALI_CONTACT_MAIL;
    }
  }
};
</script>

<style lang="scss" scoped>
.footer-app {
  &__nav-item {
    .nav-link {
      color: $gray-900;
      font-weight: 500;
      transition: color 0.2s ease-in-out, text-decoration 0.2s ease-in-out;
      text-decoration: underline;
      text-underline-offset: 2px;
      text-decoration-color: transparent;
      text-decoration-thickness: 2px;
      &:hover,
      &.router-link-active {
        text-decoration-color: $green;
      }

      &.router-link-active {
        color: $green;
      }
    }
  }
}
</style>
