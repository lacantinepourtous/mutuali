const overrides = window.yellowduck_env || {};

export const VUE_APP_I18N_LOCALE = overrides.VUE_APP_I18N_LOCALE || process.env.VUE_APP_I18N_LOCALE;
export const VUE_APP_I18N_FALLBACK_LOCALE = overrides.VUE_APP_I18N_FALLBACK_LOCALE || process.env.VUE_APP_I18N_FALLBACK_LOCALE;
export const VUE_APP_ROOT_API = overrides.VUE_APP_ROOT_API || process.env.VUE_APP_ROOT_API;
export const VUE_APP_GRAPHQL_HTTP = overrides.VUE_APP_GRAPHQL_HTTP || process.env.VUE_APP_GRAPHQL_HTTP;
export const VUE_APP_MUTUALI_CONTACT_MAIL = overrides.VUE_APP_MUTUALI_CONTACT_MAIL || process.env.VUE_APP_MUTUALI_CONTACT_MAIL;
export const VUE_APP_GOOGLE_PLACES_API_KEY = overrides.VUE_APP_GOOGLE_PLACES_API_KEY || process.env.VUE_APP_GOOGLE_PLACES_API_KEY;
export const VUE_APP_ENV = overrides.VUE_APP_ENV || process.env.VUE_APP_ENV;
export const VUE_APP_GA_MEASUREMENT_ID = overrides.VUE_APP_GA_MEASUREMENT_ID || process.env.VUE_APP_GA_MEASUREMENT_ID || "";
export const VUE_APP_SENTRY_DSN = overrides.VUE_APP_SENTRY_DSN || process.env.VUE_APP_SENTRY_DSN || "";
export const VUE_APP_SENTRY_ORIGINS = overrides.VUE_APP_SENTRY_ORIGINS || process.env.VUE_APP_SENTRY_ORIGINS || "";