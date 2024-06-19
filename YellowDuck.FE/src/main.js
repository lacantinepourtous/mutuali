import Vue from "vue";
import { BootstrapVue, IconsPlugin } from "bootstrap-vue";
import PortalVue from "portal-vue";
import * as GmapVue from "gmap-vue";
import VueGtag from "vue-gtag";

import "@/scss/app.scss";

import App from "@/app.vue";
import router from "@/router";
import i18nHelpers from "@/helpers/i18n";
import GlobalErrorHandler from "@/helpers/global-error-handler";
import "@/helpers/vee-validate";

import AuthentificationService from "@/services/authentification";
import TwilioService from "@/services/twilio";

import CustomVueI18n from "@/plugins/i18n";
import Consts from "@/plugins/consts";
import Sluggify from "@/plugins/sluggify";
import Format from "@/plugins/format";
import Array from "@/plugins/array";
import Date from "@/plugins/date";
import TermsFeed from "@/plugins/terms-feed";
import VueCookies from 'vue-cookies'

import Apollo from "@/graphql/vue-apollo";

import { VUE_APP_GOOGLE_PLACES_API_KEY, VUE_APP_GA_MEASUREMENT_ID } from "@/helpers/env";

Vue.use(BootstrapVue);
Vue.use(PortalVue);
Vue.use(IconsPlugin);

Vue.use(CustomVueI18n, { router });
Vue.use(Sluggify);
Vue.use(Consts);
Vue.use(Format);
Vue.use(Array);
Vue.use(Date);
Vue.use(TermsFeed, { language: i18nHelpers.locale(), websiteName: "Mutuali" });
Vue.use(VueCookies, { expires: '14d' })

Vue.use(GmapVue, {
  load: {
    key: VUE_APP_GOOGLE_PLACES_API_KEY,
    libraries: "places,geometry",
    language: i18nHelpers.locale() === "fr" ? "fr-CA" : "en-ca"
  },
  installComponents: true
});

if (VUE_APP_GA_MEASUREMENT_ID !== "") {
  Vue.use(
    VueGtag,
    {
      config: {
        id: VUE_APP_GA_MEASUREMENT_ID,
        params: {
          anonymize_ip: true
        }
      },
      bootstrap: TermsFeed.hasConsent("tracking")
    },
    router
  );
}

Vue.config.productionTip = false;
Vue.config.errorHandler = GlobalErrorHandler;

window.addEventListener("error", function(e) {
  GlobalErrorHandler(e);
});

TwilioService.loadLocalState().then(() => {
  AuthentificationService.loadLocalState().then(() => {
    new Vue({
      router: router,
      i18n: i18nHelpers.instance(),
      apolloProvider: Apollo.instance,
      render: (h) => h(App)
    }).$mount("#app");
  });
});
