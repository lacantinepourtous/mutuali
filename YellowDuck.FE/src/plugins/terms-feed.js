export default {
  install(Vue, options) {
    const defaultOptions = {
      noticeBannerType: "simple",
      consentType: "express",
      palette: "light",
      language: "en",
      pageLoadConsentLevels: ["strictly-necessary", "functionality"],
      noticeBannerRejectButtonHide: false,
      preferencesCenterCloseButtonHide: false,
      pageRefreshConfirmationButtons: false,
      websiteName: ""
    };

    options = { ...defaultOptions, ...options };

    let cookieConsent = null;

    const initializeCookieConsent = () => {
      if (window.cookieconsent) {
        cookieConsent = window.cookieconsent.run({
          notice_banner_type: options.noticeBannerType,
          consent_type: options.consentType,
          palette: options.palette,
          language: options.language,
          page_load_consent_levels: options.pageLoadConsentLevels,
          notice_banner_reject_button_hide: options.noticeBannerRejectButtonHide,
          preferences_center_close_button_hide: options.preferencesCenterCloseButtonHide,
          page_refresh_confirmation_buttons: options.pageRefreshConfirmationButtons,
          website_name: options.websiteName
        });
        return true;
      }
      return false;
    };

    const maxAttempts = 50;
    const checkInterval = 100;
    let attempts = 0;

    const checkCookieConsent = () => {
      attempts++;
      if (initializeCookieConsent()) return;
      if (attempts < maxAttempts) setTimeout(checkCookieConsent, checkInterval);
    };

    checkCookieConsent();

    Vue.prototype.$termsFeed = {
      getCookieConsent: () => cookieConsent,
      hasConsent,
      openPreferencesCenter() {
        if (cookieConsent) {
          cookieConsent.openPreferencesCenter();
        }
      },
      onChange(onChangeAction) {
        if (typeof onChangeAction === "function") {
          window.addEventListener("cc_userConsentSaved", () => {
            const acceptedLevels = cookieConsent && cookieConsent.userConsent && cookieConsent.userConsent.acceptedLevels ? cookieConsent.userConsent.acceptedLevels : {};
            if (Object.keys(acceptedLevels).length > 0) {
              onChangeAction(acceptedLevels);
            } else {
              const fallback = {};
              const levels = ["strictly-necessary", "functionality", "tracking", "targeting"];
              levels.forEach(level => {
                fallback[level] = hasConsent(level);
              });
              onChangeAction(fallback);
            }
          });
        }
      }
    };
  },
  hasConsent
};

function hasConsent(consentLevel) {
  const levels = window.cookieconsent && window.cookieconsent.cookieConsentObject && window.cookieconsent.cookieConsentObject.userConsent && window.cookieconsent.cookieConsentObject.userConsent.acceptedLevels;
  if (levels) return !!levels[consentLevel];

  const termsFeedConsentLevel = readCookie("cookie_consent_level");
  if (termsFeedConsentLevel) {
    const decoded = decodeURIComponent(termsFeedConsentLevel);
    const startIndex = decoded.indexOf(consentLevel) + consentLevel.length + 2;
    const value = decoded.substring(startIndex, startIndex + 5);
    return value.includes("true");
  }

  return false;
}

function readCookie(name) {
  var nameEQ = name + "=";
  var ca = document.cookie.split(';');
  for (var i = 0; i < ca.length; i++) {
    var c = ca[i];
    while (c.charAt(0) == ' ') c = c.substring(1, c.length);
    if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
  }
  return null;
}  