import Vue from "vue";

import i18nHelpers from "@/helpers/i18n";
import { ValidationProvider, ValidationObserver, extend } from "vee-validate";
import { digits, email, numeric, required, image } from "vee-validate/dist/rules";

extend("required", {
  ...required,
  message: i18nHelpers.instance().t("validator.required")
});

extend("requiredNumeric", {
  ...required,
  message: i18nHelpers.instance().t("validator.required-numeric")
});

extend("digits", {
  ...digits,
  message: i18nHelpers.instance().t("validator.digits")
});

extend("email", {
  ...email,
  message: i18nHelpers.instance().t("validator.email")
});

extend("numeric", {
  ...numeric,
  message: i18nHelpers.instance().t("validator.numeric")
});

extend("image", {
  ...image,
  message: i18nHelpers.instance().t("validator.image")
});

extend("password", {
  validate(value) {
    return new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{10,})").test(value);
  },
  message: i18nHelpers.instance().t("validator.password")
});

extend("samePassword", {
  params: ["target"],
  validate(value, { target }) {
    return value === target;
  },
  message: i18nHelpers.instance().t("validator.samePassword")
});

extend("url", {
  validate(value) {
    return RegExp(
      "^((http|https)://)(www.)?(?!.*(http|https|www.))[a-zA-Z0-9_-]+((?!.*[/]{2})+.[a-zA-Z-0-9_+/]+)+((/)[w#]+)*(/w+?[a-zA-Z0-9_]+=w+(&[a-zA-Z0-9_]+=w+)*)?$"
    ).test(value);
  },
  message: i18nHelpers.instance().t("validator.url")
});

extend("postalCode", {
  validate(value) {
    return new RegExp(/^[A-Z]\d[A-Z][ -]?\d[A-Z]\d$/i).test(value);
  },
  message: i18nHelpers.instance().t("validator.postal-code")
});

extend("emptyHtml", {
  validate(value) {
    return value.replace(/<[^/>][^>]*><\/[^>]+>/gi, "").trim() !== "";
  },
  message: i18nHelpers.instance().t("validator.required")
});

extend("haveLatLong", {
  validate(value) {
    return value.latitude && value.longitude;
  },
  message: i18nHelpers.instance().t("validator.lat-long-required")
});

extend("dateMin", {
  params: ["target"],
  validate: (value, { target }) => {
    return value >= target;
  },
  message: i18nHelpers.instance().t("validator.date-min")
});

extend("atLeastOneChecked", {
  params: ["hasOneChecked"],
  validate(value, { hasOneChecked }) {
    return hasOneChecked;
  },
  message: i18nHelpers.instance().t("validator.atLeastOneChecked")
});

extend("isNEQ", {
  validate(value) {
    return new RegExp("^[1-7]\\d{9}$").test(value);
  },
  message: i18nHelpers.instance().t("validator.isNEQ")
});

extend("isValidPhoneNumber", {
  ...required,
  message: i18nHelpers.instance().t("validator.isValidPhoneNumber")
});
// Register globally
Vue.component("ValidationObserver", ValidationObserver);
Vue.component("ValidationProvider", ValidationProvider);
