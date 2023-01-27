<template>
  <div v-if="notifications && notifications.length > 0">
    <notification-alert
      v-for="notification in notifications"
      v-on:dismiss="onNotificationDismiss"
      :item="notification"
      :key="notification.id"
    />
  </div>
</template>

<graphql>
query LocalNotifications {
  notifications @client {
    id
    type
    text
    duration
    dismissible
  }
}
</graphql>

<script>
import NotificationService from "@/services/notification";
import NotificationAlert from "@/components/notifications/notification-alert";

export default {
  components: {
    NotificationAlert
  },
  apollo: {
    notifications: {
      query() {
        return this.$options.query.LocalNotifications;
      }
    }
  },
  methods: {
    onNotificationDismiss: function (id) {
      NotificationService.removeNotificationById(id);
    }
  }
};
</script>
