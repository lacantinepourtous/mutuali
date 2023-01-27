import Vue from "vue";
import VueI18n from "vue-i18n";

import { LANG_FR, LANG_EN } from "@/consts/langs";
import { FORMAT_MONTH_YEAR } from "@/consts/formats";
import { LOCAL_STORAGE_LOCALE } from "@/consts/local-storage";

import {
  VUE_APP_I18N_FALLBACK_LOCALE
} from "@/helpers/env";

import advancedFormat from "dayjs/plugin/advancedFormat";
import dayjs from "dayjs";
import "dayjs/locale/fr";
import "dayjs/locale/en";

const supportedLocales = [LANG_FR, LANG_EN];
let initialLocale = null;

// First check in querystring
const params = new Proxy(new URLSearchParams(window.location.search), {
  get: (searchParams, prop) => searchParams.get(prop),
});
if(supportedLocales.includes(params.lang)) {
  initialLocale = params.lang;
}
// After check in localstorage
else {
  initialLocale = localStorage.getItem(LOCAL_STORAGE_LOCALE);
  // Finally, use the default
  if (!supportedLocales.includes(initialLocale)) initialLocale = supportedLocales[0];
}

dayjs.extend(advancedFormat);
dayjs.locale(initialLocale);

Vue.use(VueI18n);

const instanceI18n = new VueI18n({
  locale: initialLocale,
  fallbackLocale: VUE_APP_I18N_FALLBACK_LOCALE || LANG_FR,
  messages: loadLocaleMessages()
});

function loadLocaleMessages() {
  const locales = require.context("@/locales", true, /[A-Za-z0-9-_,\s]+\.json$/i);
  const messages = {};
  locales.keys().forEach((key) => {
    const matched = key.match(/([A-Za-z0-9-_]+)\./i);
    if (matched && matched.length > 1) {
      const locale = matched[1];
      messages[locale] = locales(key);
    }
  });

  return messages;
}

function setLang(lang) {
  localStorage.setItem(LOCAL_STORAGE_LOCALE, lang);
  location.reload();
}

const i18n = {
  instance: function() {
    return instanceI18n;
  },
  locale: () => {
    return instanceI18n.locale;
  },
  setLang,
  changeLang: () => {
    let lang = instanceI18n.locale === LANG_EN ? LANG_FR : LANG_EN;
    setLang(lang);
  },
  getLocalizedDate: (date, format) => {
    return dayjs(date).format(format);
  },
  getLocalizedMonthYear: (date) => {
    return i18n.getLocalizedDate(date, FORMAT_MONTH_YEAR);
  },
  getLocalizedDateDiff: (startDate, endDate, format) => {
    return dayjs(endDate).diff(dayjs(startDate), format);
  },
  isLangValid: (lang) => supportedLocales.includes(lang)
};

export default i18n;
