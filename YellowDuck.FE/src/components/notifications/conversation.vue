<template>
  <span></span>
</template>

<graphql>
query LocalUser {
  user @client {
    isConnected
  }
}
</graphql>

<script>
import { TWILIO_EVENT_MESSAGE_ADDED } from "@/consts/twilio";

import EventBus from "@/helpers/event-bus";
import debounce from "@/helpers/debounce";

let audio = new Audio(require("@/assets/sound/notification.mp3"));

export default {
  beforeDestroy() {
    EventBus.$off(TWILIO_EVENT_MESSAGE_ADDED, this.onMessageAdded);
  },
  methods: {
    onMessageAdded: function (event) {
      // Since the getUnreadMessagesCount is not 100% real time, we wait 3 seconds before displayed the badge
      // https://tinyurl.com/m6s482d3
      let debounceFn = debounce(async () => {
        if (audio.paused) {
          audio.play();
        } else {
          audio.currentTime = 0;
        }
      }, 750);
      debounceFn();
    }
  },
  apollo: {
    user: {
      query() {
        return this.$options.query.LocalUser;
      },
      result({ data }) {
        if (data && data.user.isConnected) {
          //clear EventBus before readded since we can pass more then one time here
          EventBus.$off(TWILIO_EVENT_MESSAGE_ADDED, this.onMessageAdded);
          EventBus.$on(TWILIO_EVENT_MESSAGE_ADDED, this.onMessageAdded);
        }
      }
    }
  }
};
</script>