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
      body: JSON.stringify({ PhoneNumberOrEmail: this.cleanPhoneNumberOrEmail(PhoneNumberOrEmail) })
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
  verifyValidationCode: async function(phone, email, code) {
    var body = {
      code: code
    }

    if (phone) {
      body.phone = this.cleanPhoneNumberOrEmail(phone);
    }

    if (email) {
      body.email = this.cleanPhoneNumberOrEmail(email);
    }

    const response = await fetch(`${VUE_APP_ROOT_API}/phone/verify`, {
      method: "POST",
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(body)
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
  cleanPhoneNumberOrEmail: function(PhoneNumberOrEmail) {
    const cleanedValue = PhoneNumberOrEmail.trim().replace(/\s/g, '');
    return cleanedValue.includes('@') ? cleanedValue : cleanedValue.replace(/\D/g, '');
  }
};

