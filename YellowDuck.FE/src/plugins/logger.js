import LoggerService from "@/services/logger";

export default {
  install(Vue) {
    Vue.prototype.$logger.logInformation = (msg) => LoggerService.logInformation(msg);
    Vue.prototype.$logger.logWarning = (msg) => LoggerService.logWarning(msg);
    Vue.prototype.$logger.logError = (msg) => LoggerService.logError(msg);
    Vue.prototype.$logger.logCritical = (msg) => LoggerService.logCritical(msg);
  }
};
