import i18nHelpers from "@/helpers/i18n";

export default {
  install(Vue, options) {
    let router = options.router;
    Vue.prototype.$t = (key, values) => {
      if (router.currentRoute.query.debugI18n !== undefined) {
        return key;
      } else {
        return i18nHelpers.instance().t(key, values);
      }
    };
    Vue.prototype.$tc = (key, values) => {
      if (router.currentRoute.query.debugI18n !== undefined) {
        return key;
      } else {
        return i18nHelpers.instance().tc(key, values);
      }
    };
  }
};
