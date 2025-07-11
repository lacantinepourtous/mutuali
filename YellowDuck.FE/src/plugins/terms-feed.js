export default {
  install(Vue, options) {
    var defaultOptions = {
      noticeBannerType: "simple",
      consentType: "express",
      palette: "light",
      language: "en",
      pageLoadConsentLevles: ["strictly-necessary", "functionality"],
      noticeBannerRejectButtonHide: false,
      preferencesCenterCloseButtonHide: false,
      pageRefreshConfirmationButtons: false,
      websiteName: ""
    };
    options = { ...defaultOptions, ...options };

    const script = document.createElement('script');
    script.src = '/static/vendors/termsfeed-cookie-consent.js';
    script.async = true;
    document.body.appendChild(script);

    var cookieConsent = null;

    script.onload = () => {
      cookieConsent = window.cookieconsent.run({
        "notice_banner_type": options.noticeBannerType,
        "consent_type": options.consentType,
        "palette": options.palette,
        "language": options.language,
        "page_load_consent_levels": options.pageLoadConsentLevles,
        "notice_banner_reject_button_hide": options.noticeBannerRejectButtonHide,
        "preferences_center_close_button_hide": options.preferencesCenterCloseButtonHide,
        "page_refresh_confirmation_buttons": options.pageRefreshConfirmationButtons,
        "website_name": options.websiteName
      });
    };

    Vue.prototype.$termsFeed = {
      cookieConsent,
      hasConsent: this.hasConsent,
      openPreferencesCenter() {
        if (cookieConsent) {
          cookieConsent.openPreferencesCenter();
        }
      },
      onChange(onChangeAction) {
        if (typeof onChangeAction === "function") {
          window.addEventListener("cc_userConsentSaved", (function () {
            window.console.log("User consent saved: ", cookieConsent.userConsent.acceptedLevels);
            onChangeAction(cookieConsent.userConsent.acceptedLevels);
          }).bind(this));
        }
      }
    };
  },
  hasConsent(consentLevel) {
    if (window.cookieconsent && window.cookieconsent.cookieConsentObject && window.cookieconsent.cookieConsentObject.userConsent && window.cookieconsent.cookieConsentObject.userConsent.acceptedLevels) {
      return window.cookieconsent.cookieConsentObject.userConsent.acceptedLevels[consentLevel];
    }
    else { // else, read the cookie directly
      const termsFeedConsentLevel = readCookie("cookie_consent_level");

      let consent = false;

      // Accepted consentLevel param are "strictly-necessary", "functionality", "tracking" and "targeting"
      if (termsFeedConsentLevel) {
        const consentString = decodeURIComponent(termsFeedConsentLevel);
        const startIndex = consentString.indexOf(consentLevel) + consentLevel.length + 2;
        const consentValueSubstr = consentString.substring(startIndex, startIndex + 5); // should return "true," or "false"
        consent = consentValueSubstr.includes("true");
      }

      return consent;
    }

  }
};

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