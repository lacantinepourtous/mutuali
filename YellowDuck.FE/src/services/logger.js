import { VUE_APP_ROOT_API } from "@/helpers/env";

const MAX_MSG_QUEUE = 5;

const LOG_LEVEL_INFORMATION = "Information";
const LOG_LEVEL_WARNING = "Warning";
const LOG_LEVEL_ERROR = "Error";
const LOG_LEVEL_CRITICAL = "Critical";

let logMessage = [];
let sendMsgTimeout = null;

export default {
  logInformation: (msg) => log(msg, LOG_LEVEL_INFORMATION),
  logWarning: (msg) => log(msg, LOG_LEVEL_WARNING),
  logError: (msg) => log(msg, LOG_LEVEL_ERROR),
  logCritical: (msg) => log(msg, LOG_LEVEL_CRITICAL)
};

function log(message, level) {
  if (!message) message = "";
  else message = message.toString();

  logMessage.push({ message, level });
  resetOrSend();
}

async function sendMsg() {
  try {
    await fetch(`${VUE_APP_ROOT_API}/log`, {
      method: "post",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json"
      },
      body: JSON.stringify(logMessage)
    });
  } catch (error) {
    //silent error
    return;
  }
  logMessage = [];
}

function resetOrSend() {
  if (logMessage.length >= MAX_MSG_QUEUE) {
    sendMsg();
    clearTimeout(sendMsgTimeout);
  } else {
    resetTimeout();
  }
}

function resetTimeout() {
  clearTimeout(sendMsgTimeout);
  startTimeout();
}

function startTimeout() {
  sendMsgTimeout = setTimeout(sendMsg, 1000);
}
