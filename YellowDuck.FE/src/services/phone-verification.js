import NotificationService from "@/services/notification";
import i18nHelpers from "@/helpers/i18n";
import { VUE_APP_ROOT_API } from "@/helpers/env";



export default {
  sendValidationCode: async function(PhoneNumberOrEmail) {
    const response = await fetch(`${VUE_APP_ROOT_API}/phone/verify-request`, {
      method: "POST",
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ PhoneNumberOrEmail })
    });

    const responseJson = await response.json();

    if (!responseJson.success) {
      NotificationService.showError(i18nHelpers.instance().t(responseJson.messageKey));
      return false;
    }

    if (!response.ok && response.status !== 400) {
      throw new Error(i18nHelpers.instance().t("error.unexpected"));
    }

    return responseJson.success;
  },
  verifyValidationCode: async function(PhoneNumberOrEmail, code) {
    const response = await fetch(`${VUE_APP_ROOT_API}/phone/verify`, {
      method: "POST",
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ PhoneNumberOrEmail, code })
    });

    const responseJson = await response.json();

    if (!responseJson.success) {
      NotificationService.showError(i18nHelpers.instance().t(responseJson.messageKey));
      return false;
    }

    if (!response.ok && response.status !== 400) {
      throw new Error(i18nHelpers.instance().t("error.unexpected"));
    }

    return responseJson.success;
  }
};

