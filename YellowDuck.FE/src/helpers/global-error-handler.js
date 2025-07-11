import i18n from "@/helpers/i18n";
import NotificationService from "@/services/notification";
import Logger from "@/services/logger";

export default function (err, vm) {
  if (err.error) err = err.error;
  if (err.handled) return;

  // Ignorer les erreurs de scripts externes
  if (isErrorFromExternalScript(err)) {
    err.handled = true;
    return;
  }

  err.handled = true;
  showErrors(getErrorMessages(err, vm));
}

function isErrorFromExternalScript(err) {
  // Les erreurs de scripts tiers ont souvent une stack null
  // et un message générique comme "Script error."
  // TermsFeed faisait parfois des erreurs de ce type et one ne veut pas les afficher
  const isScriptError = err.message === "Script error." && !err.stack;
  return isScriptError;
}

function showErrors(messages) {
  if (!Array.isArray(messages)) {
    messages = [messages];
  }

  for (const message of messages) {
    // eslint-disable-next-line no-console
    console.log("message", message);
    NotificationService.showError(message);
  }
}

function getErrorMessages(err, vm) {
  if (vm && err.graphQLErrors) {
    return err.graphQLErrors.map((gqlError) => {
      gqlError.operationType = err.gqlOperationType;
      return getGraphQLErrorMessage(gqlError, vm);
    });
  } else {
    return getUnknownErrorMessage(err);
  }
}

function getGraphQLErrorMessage(gqlError, vm) {
  if (gqlError.extensions && gqlError.extensions.code) {
    const errorCode = gqlError.extensions.code;
    return findGraphQLErrorMessage(errorCode, gqlError, vm);
  }
}

function findGraphQLErrorMessage(errorCode, gqlError, vm) {
  const gqlErrorsConfig = getGraphQLErrorsConfigFromViewModel(vm);
  if (gqlErrorsConfig && gqlErrorsConfig[errorCode]) {
    let errorMessage = gqlErrorsConfig[errorCode];
    if (typeof errorMessage === "function") {
      errorMessage = errorMessage.bind(vm)(gqlError);
    }
    return errorMessage;
  } else if (vm.$parent) {
    return findGraphQLErrorMessage(errorCode, gqlError, vm.$parent);
  } else {
    return getUnknownErrorMessage(errorCode);
  }
}

function getGraphQLErrorsConfigFromViewModel(vm) {
  let config = vm.$options.gqlErrors;
  if (!config) return null;

  if (typeof config === "function") {
    config = config.bind(vm)();
  }

  return config;
}

function getUnknownErrorMessage(error) {
  // On log les erreurs non-gérées pour aider à les identifier

  if (process.env.NODE_ENV !== "production") {
    // eslint-disable-next-line no-console
    console.error(error);
  }

  Logger.logError(error);

  // Et on affiche le message générique
  return i18n.instance().t("error.unexpected");
}
