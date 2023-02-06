<template>
  <div class="w-100">
    <div class="section section--sm">
      <h1 class="my-4">{{ $t("page-title.edit-profile") }}</h1>
      <edit-profile
        v-if="this.me"
        :user-id="this.me.id"
        :profile-id="this.me.profile.id"
        :user-type="this.me.type"
        @submitForm="saveProfile"
      />
    </div>
  </div>
</template>

<script>
import EditProfile from "@/components/user-profile/edit";

import UserService from "@/services/user";
import NotificationService from "@/services/notification";

export default {
  components: {
    EditProfile
  },
  apollo: {
    me: {
      query() {
        return this.$options.query.Me;
      }
    }
  },
  methods: {
    saveProfile: async function (input) {
      await UserService.updateUserProfile(input);
      NotificationService.showSuccess(this.$t("notification.edit-profile-saved"));
      window.scrollTo(0, 0);
    }
  }
};
</script>
<graphql>
query Me {
  me {
    id
    type
    profile {
      id
    }
  }
}
</graphql>
