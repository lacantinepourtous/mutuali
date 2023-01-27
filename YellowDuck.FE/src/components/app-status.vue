<template>
  <p
    v-if="isUnavailable || isOutOfDate"
    class="section section--padding-x app-status"
    :class="{ 'app-status--danger': isUnavailable }"
  >
    <template v-if="isUnavailable">
      <b-icon-exclamation-circle aria-hidden="true"></b-icon-exclamation-circle>
      {{ $t("app-status.unavailable") }}
    </template>
    <span v-else-if="isOutOfDate" class="d-flex align-items-center justify-content-between">
      <span class="d-block mr-2">
        {{ $t("app-status.out-of-date") }}
      </span>
      <b-button size="sm" pill variant="light" :aria-label="$t('app-status.refresh')" @click="refresh">
        <b-icon-arrow-clockwise aria-hidden="true"></b-icon-arrow-clockwise>
      </b-button>
    </span>
  </p>
</template>

<script>
import Vue from "vue";
import * as Sentry from "@sentry/vue";
import { Integrations } from "@sentry/tracing";
import router from "@/router";

import AuthentificationService from "@/services/authentification";

import { LOCAL_STORAGE_AUTHTOKEN } from "@/consts/local-storage";
import { VUE_APP_ROOT_API, VUE_APP_ENV, VUE_APP_SENTRY_DSN, VUE_APP_SENTRY_ORIGINS } from "@/helpers/env";

const CHECK_STATUS_INTERVAL_MS = 60000;
const CHECK_TOKEN_INTERVAL_MS = 30000;

export default {
  data() {
    return {
      status: null,
      error: null,
      version: null,
      intervalCheckStatus: null,
      intervalCheckToken: null
    };
  },
  computed: {
    isOutOfDate() {
      if (!this.status) return false;

      return this.version !== this.status.version;
    },
    isUnavailable() {
      if (this.error) return true;
      if (!this.status) return false;

      return !this.status.ready;
    }
  },
  methods: {
    setupSentry() {
      Sentry.init({
        Vue,
        release: `mutuali@${this.version}`,
        dsn: VUE_APP_SENTRY_DSN,
        integrations: [
          new Integrations.BrowserTracing({
            routingInstrumentation: Sentry.vueRouterInstrumentation(router),
            tracingOrigins: [...VUE_APP_SENTRY_ORIGINS.split(";"), /^\//]
          })
        ],
        tracesSampleRate: 0.5,
        environment: VUE_APP_ENV
      });
    },
    async checkStatus() {
      try {
        let response = await fetch(`${VUE_APP_ROOT_API}/status`);
        this.status = await response.json();
        this.error = null;

        if (!this.version) {
          this.version = this.status.version;
          this.setupSentry();
        } else if (this.isOutOfDate) {
          this.stop();
        }
      } catch (error) {
        this.error = error;
      }
    },
    async checkToken() {
      let authUserToken = AuthentificationService.getUserToken();
      if (authUserToken === "") {
        authUserToken = null;
      }
      let localUserToken = localStorage.getItem(LOCAL_STORAGE_AUTHTOKEN);
      if (authUserToken !== localUserToken) {
        // reload current page since a login or logout is set in another tab
        this.$router.go();
      }
    },
    start() {
      if (this.intervalCheckStatus || this.intervalCheckToken) {
        this.stop();
      }

      this.intervalCheckStatus = setInterval(() => this.checkStatus(), CHECK_STATUS_INTERVAL_MS);
      this.intervalCheckToken = setInterval(() => this.checkToken(), CHECK_TOKEN_INTERVAL_MS);
    },
    stop() {
      if (this.intervalCheckStatus) {
        clearInterval(this.intervalCheckStatus);
        this.intervalCheckStatus = null;
      }
      if (this.intervalCheckToken) {
        clearInterval(this.intervalCheckToken);
        this.intervalCheckToken = null;
      }
    },
    refresh() {
      window.location.reload();
    }
  },
  mounted() {
    this.checkStatus();
    this.start();
  },
  destroyed() {
    this.stop();
  }
};
</script>

<style lang="scss">
.app-status {
  background: $gray-900;
  color: $white;
  font-size: $small-font-size;
  margin: 0;
  padding-top: $spacer / 2;
  padding-bottom: $spacer / 2;

  .btn.rounded-pill .b-icon {
    margin: 0 -2.25px;
  }

  &--danger {
    background: $danger;
  }
}
</style>
