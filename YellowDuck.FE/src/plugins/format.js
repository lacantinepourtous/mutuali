import i18n from "@/helpers/i18n";
import { DEFAULT_DATE_FORMAT, DEFAULT_DATETIME_FORMAT, FORMAT_MONTH_YEAR, SHORT_DATE_FORMAT } from "@/consts/formats";

export default {
  install(Vue, options) {
    Vue.prototype.$format = {
      shortDate: (datetime) => {
        return i18n.dayjs(datetime).format(SHORT_DATE_FORMAT);
      },
      shortMonthDay: (datetime) => {
        return i18n.dayjs(datetime).format(i18n.t("format-shortMonthDay"));
      },
      date: (datetime, format) => {
        if (!format) {
          format = DEFAULT_DATE_FORMAT;
        }
        return i18n.dayjs(datetime).format(format);
      },
      dateTime: (datetime, format) => {
        if (!format) {
          format = DEFAULT_DATETIME_FORMAT;
        }
        return i18n.dayjs(datetime).format(format);
      },
      dateMonthYear: (datetime) => {
        return i18n.dayjs(datetime).format(FORMAT_MONTH_YEAR);
      },
      dateDay(datetime) {
        const locale = i18n.locale() === "fr" ? "fr-CA" : "en-CA";
        if (locale === "fr-CA") {
          return i18n.dayjs(datetime).format("dddd");
        } else {
          return null;
        }
      },
      dateLong(datetime) {
        const locale = i18n.locale() === "fr" ? "fr-CA" : "en-CA";
        if (locale === "fr-CA") {
          return i18n.dayjs(datetime).format("D MMMM YYYY");
        } else {
          return i18n.dayjs(datetime).format("dddd, MMMM D, YYYY");
        }
      },
      dateDiff: (startDate, endDate, format) => {
        if (!format) {
          format = DEFAULT_DATE_FORMAT;
        }
        return i18n.dayjs(endDate).diff(i18n.dayjs(startDate), format);
      },
      nl2br(text) {
        if (!text) return text;

        return Vue.prototype.$format.escapeHtml(text).replace(/\n/g, "<br/>");
      },
      escapeHtml(html) {
        // https://stackoverflow.com/a/30930653
        return document.createElement("div").appendChild(document.createTextNode(html)).parentNode.innerHTML;
      },
      stripHtml(html) {
        const el = document.createElement("div");
        el.innerHTML = html;
        return el.innerText.trim();
      },
      formatMoney(amount, decimalCount = 2, decimal = ",", thousands = "") {
        try {
          decimalCount = Math.abs(decimalCount);
          decimalCount = isNaN(decimalCount) ? 2 : decimalCount;

          const negativeSign = amount < 0 ? "-" : "";

          let i = parseInt((amount = Math.abs(Number(amount) || 0).toFixed(decimalCount))).toString();
          let j = i.length > 3 ? i.length % 3 : 0;

          return (
            negativeSign +
            (j ? i.substr(0, j) + thousands : "") +
            i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousands) +
            (decimalCount
              ? decimal +
                Math.abs(amount - i)
                  .toFixed(decimalCount)
                  .slice(2)
              : "") +
            " $ CA"
          );
        } catch (e) {
          return amount;
        }
      }
    };
  }
};
