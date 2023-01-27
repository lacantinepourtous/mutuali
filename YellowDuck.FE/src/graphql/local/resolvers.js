import MutationHelper from "@/graphql/local/mutation-helper";

import { LocalUser } from "./user.graphql";
import { LocalShowMenu } from "./app.graphql";
import { LocalNotifications } from "./notification.graphql";
import { TwilioToken } from "./twilio.graphql";

export default {
  Mutation: {
    updateShowMenu(_, { showMenu }, { cache }) {
      MutationHelper.updateCache(cache, { query: LocalShowMenu }, (x) => {
        x.app.showMenu = showMenu;
      });

      return showMenu;
    },
    updateLocalUser(_, { user }, { cache }) {
      const data = MutationHelper.updateCache(cache, { query: LocalUser }, (x) => {
        if (user.isConnected !== null && user.isConnected !== undefined) {
          x.user.isConnected = user.isConnected;
        }

        if (user.accessToken !== null && user.accessToken !== undefined) {
          x.user.accessToken = user.accessToken;
        }

        if (user.refreshToken !== null && user.refreshToken !== undefined) {
          x.user.refreshToken = user.refreshToken;
        }
      });

      return data.user;
    },
    addNotification(_, { notification }, { cache }) {
      MutationHelper.updateCache(cache, { query: LocalNotifications }, (x) => {
        x.notifications.push(notification);
      });

      return notification;
    },
    removeNotificationById(_, { id }, { cache }) {
      MutationHelper.updateCache(cache, { query: LocalNotifications }, (x) => {
        x.notifications = x.notifications.filter((item) => item.id !== id);
      });

      return id;
    },
    removeNotificationByType(_, { type }, { cache }) {
      MutationHelper.updateCache(cache, { query: LocalNotifications }, (x) => {
        x.notifications = x.notifications.filter((item) => item.type !== type);
      });

      return type;
    },
    updateTwilioToken(_, { token }, { cache }) {
      MutationHelper.updateCache(cache, { query: TwilioToken }, (x) => {
        x.twilio.token = token;
      });
    }
  }
};
