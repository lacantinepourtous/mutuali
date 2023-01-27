import Apollo from "@/graphql/vue-apollo";
import {
  NOTIFICATION_TYPE_SUCCESS,
  NOTIFICATION_TYPE_INFO,
  NOTIFICATION_TYPE_WARNING,
  NOTIFICATION_TYPE_ERROR
} from "@/consts/notifications";
import { CreateLocalNotification, RemoveLocalNotificationById, RemoveLocalNotificationByType } from "./notification.graphql";
import id from "@/helpers/unique-id";

export default {
  showSuccess: async (text, duration = 6000, dismissible = true) =>
    await addNotification(NOTIFICATION_TYPE_SUCCESS, text, dismissible, duration),
  showInfo: async (text, duration = 6000, dismissible = true) =>
    await addNotification(NOTIFICATION_TYPE_INFO, text, dismissible, duration),
  showWarning: async (text, duration = 8000, dismissible = true) =>
    await addNotification(NOTIFICATION_TYPE_WARNING, text, dismissible, duration),
  showError: async (text, duration = 8000, dismissible = true) =>
    await addNotification(NOTIFICATION_TYPE_ERROR, text, dismissible, duration),
  removeNotificationById: async (id) => {
    return await Apollo.instance.defaultClient.mutate({
      mutation: RemoveLocalNotificationById,
      variables: {
        id
      }
    });
  },
  removeNotificationByType: async (type) => {
    return await Apollo.instance.defaultClient.mutate({
      mutation: RemoveLocalNotificationByType,
      variables: {
        type
      }
    });
  }
};

async function addNotification(type, text, dismissible, duration) {
  let notification = {
    __typename: "Notification",
    id: id.generate(),
    type,
    text,
    duration,
    dismissible
  };

  await Apollo.instance.defaultClient.mutate({
    mutation: CreateLocalNotification,
    variables: { notification }
  });

  return notification.id;
}
