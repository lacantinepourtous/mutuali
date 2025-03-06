import Apollo from "@/graphql/vue-apollo";

import NotificationService from "@/services/notification";
import TwilioService from "@/services/twilio";

import router from "@/router";

import i18nHelpers from "@/helpers/i18n";
import { VUE_APP_ROOT_API } from "@/helpers/env";

import { URL_ROOT } from "@/consts/urls";
import { USER_TYPE_USER, USER_TYPE_ADMIN, USER_TYPE_ANONYME } from "@/consts/enums";
import { CLAIM_UTYPE_USER, CLAIM_UTYPE_ADMIN } from "@/consts/claims";
import {
  LOCAL_STORAGE_AUTHTOKEN,
  LOCAL_STORAGE_DEVICEID,
  LOCAL_STORAGE_REFRESHTOKEN,
  LOCAL_STORAGE_RENEWTOKEN_STATUS,
  LOCAL_STORAGE_TWILIOTOKEN,
  LOCAL_STORAGE_MAP_LATLONG,
  LOCAL_STORAGE_MAP_ZOOMLEVEL
} from "@/consts/local-storage";

import {
  UpdateLocalUser,
  ResendConfirmationEmail,
  ConfirmEmail,
  LocalUserIsConnected,
  LocalUserToken,
  LocalUser,
  VerifyEmail
} from "./user.graphql";

export default {
  login: async function(username, password) {
    let deviceId = localStorage.getItem(LOCAL_STORAGE_DEVICEID);

    if (deviceId === null) {
      deviceId = "mutualiapp-" + new Date().getTime();
      localStorage.setItem(LOCAL_STORAGE_DEVICEID, deviceId);
    }

    let requestData = {
      username,
      password,
      deviceId
    };
    let response = null;
    try {
      response = await fetch(`${VUE_APP_ROOT_API}/token/login`, {
        method: "post",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json"
        },
        body: JSON.stringify(requestData)
      });
    } catch (error) {
      NotificationService.showError(i18nHelpers.instance().t("error.unexpected"));
      return { success: false };
    }

    let result = await response.json();
    if (response.status === 200) {
      if (result === "2FA code sent") {
        return { success: true, twoFaRequired: true };
      }
      
      await setUser(result.token, result.refreshToken);

      let path = router.currentRoute.query.returnPath || { name: URL_ROOT };
      router.push(path);
      
      return { success: true, twoFaRequired: false };
    } else if (result === "Email not confirmed") {
      NotificationService.showError(i18nHelpers.instance().t("notification.login-email-not-confirmed", { email: username }));
    } else if (result === "User is locked out") {
      NotificationService.showError(i18nHelpers.instance().t("notification.user-locked-out"));
    } else {
      NotificationService.showError(i18nHelpers.instance().t("notification.login-error"));
    }
    return { success: false };
  },
  renewToken: async function() {
    if (localStorage.getItem(LOCAL_STORAGE_RENEWTOKEN_STATUS)) {
      await waitRefreshToken();
      return;
    }

    localStorage.setItem(LOCAL_STORAGE_RENEWTOKEN_STATUS, "active");

    let token = localStorage.getItem(LOCAL_STORAGE_AUTHTOKEN);
    let refreshToken = localStorage.getItem(LOCAL_STORAGE_REFRESHTOKEN);
    let deviceId = localStorage.getItem(LOCAL_STORAGE_DEVICEID);

    let requestData = {
      token,
      refreshToken,
      deviceId
    };

    let response = await fetch(`${VUE_APP_ROOT_API}/token/refresh`, {
      method: "post",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json"
      },
      body: JSON.stringify(requestData)
    });

    if (response.status === 200) {
      let result = await response.json();
      await setUser(result.token, result.refreshToken);
    } else {
      await logout();
    }

    localStorage.removeItem(LOCAL_STORAGE_RENEWTOKEN_STATUS);
  },
  logout: logout,
  loadLocalState: async function() {
    let token = localStorage.getItem(LOCAL_STORAGE_AUTHTOKEN);
    let refreshToken = localStorage.getItem(LOCAL_STORAGE_REFRESHTOKEN);

    localStorage.removeItem(LOCAL_STORAGE_RENEWTOKEN_STATUS);

    if (token !== null && refreshToken !== null) {
      await setUser(token, refreshToken);
    }
  },
  getUserIsLogged: function() {
    let result = Apollo.instance.defaultClient.readQuery({
      query: LocalUserIsConnected
    });

    return result.user.isConnected;
  },
  getUserType: function() {
    let result = Apollo.instance.defaultClient.readQuery({
      query: LocalUser
    });

    if (result.user.isConnected) {
      let userToken = JSON.parse(atob(result.user.accessToken.split(".")[1]));
      return uTypeToGraphqlType(userToken.utype);
    }

    return USER_TYPE_ANONYME;
  },
  getUserToken: function() {
    let result = Apollo.instance.defaultClient.readQuery({
      query: LocalUserToken
    });

    return result.user.accessToken;
  },
  getUserId: function() {
    let result = Apollo.instance.defaultClient.readQuery({
      query: LocalUser
    });

    if (result.user.isConnected) {
      let userToken = JSON.parse(atob(result.user.accessToken.split(".")[1]));
      return userToken.nameid;
    }

    return "";
  },
  resendConfirmationEmail: async function(email) {
    await Apollo.instance.defaultClient.mutate({
      mutation: ResendConfirmationEmail,
      variables: {
        input: {
          email
        }
      }
    });
  },
  confirmEmail: async function(token, email) {
    await Apollo.instance.defaultClient.mutate({
      mutation: ConfirmEmail,
      variables: {
        input: {
          email,
          token
        }
      }
    });
  },
  verifyEmail: async function(email) {
    await Apollo.instance.defaultClient.mutate({
      mutation: VerifyEmail,
      variables: {
        input: {
          email
        }
      }
    });
  }
};

function uTypeToGraphqlType(utype) {
  let type = USER_TYPE_ANONYME;
  switch (utype) {
    case CLAIM_UTYPE_ADMIN:
      type = USER_TYPE_ADMIN;
      break;
    case CLAIM_UTYPE_USER:
      type = USER_TYPE_USER;
      break;
  }
  return type;
}

async function setUser(token, refreshToken) {
  localStorage.setItem(LOCAL_STORAGE_AUTHTOKEN, token);
  localStorage.setItem(LOCAL_STORAGE_REFRESHTOKEN, refreshToken);

  let userToken = JSON.parse(atob(token.split(".")[1]));
  let user = {
    type: uTypeToGraphqlType(userToken.utype),
    accessToken: token,
    refreshToken: refreshToken
  };

  await Apollo.instance.defaultClient.mutate({
    mutation: UpdateLocalUser,
    variables: {
      user
    }
  });

  if (user.type === USER_TYPE_USER) {
    await TwilioService.initTwilioClient();
  }

  user = {
    isConnected: true
  };
  await Apollo.instance.defaultClient.mutate({
    mutation: UpdateLocalUser,
    variables: {
      user
    }
  });
}

async function waitRefreshToken() {
  return new Promise((resolve, reject) => {
    let timerId = setInterval(function() {
      if (!localStorage.getItem(LOCAL_STORAGE_RENEWTOKEN_STATUS)) {
        clearInterval(timerId);
        resolve();
      }
    }, 50);
  });
}

async function logout() {
  localStorage.removeItem(LOCAL_STORAGE_AUTHTOKEN);
  localStorage.removeItem(LOCAL_STORAGE_REFRESHTOKEN);
  localStorage.removeItem(LOCAL_STORAGE_TWILIOTOKEN);
  localStorage.removeItem(LOCAL_STORAGE_MAP_LATLONG);
  localStorage.removeItem(LOCAL_STORAGE_MAP_ZOOMLEVEL);
  localStorage.removeItem(LOCAL_STORAGE_RENEWTOKEN_STATUS);

  window.location.href = "/";
}
