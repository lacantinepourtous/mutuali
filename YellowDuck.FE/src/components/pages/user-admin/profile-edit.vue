<template>
  <div class="w-100">
    <portal v-if="!userCreated" :to="$consts.enums.PORTAL_HEADER">
      <nav-close :to="{ name: $consts.urls.URL_LIST_USERS }"></nav-close>
    </portal>
    <div class="section section--sm">
      <h1 class="h2 my-4">{{ $t("page-title.edit-profile") }}</h1>
      <edit-profile
        v-if="this.user"
        :user-id="this.user.id"
        :profile-id="this.user.profile.id"
        :user-type="this.user.type"
        @submitForm="saveProfile"
      />
    </div>
  </div>
</template>

<script>
import NavClose from "@/components/nav/close";
import EditProfile from "@/components/user-profile/edit";

import UserService from "@/services/user";
import NotificationService from "@/services/notification";

export default {
  components: {
    NavClose,
    EditProfile
  },
  apollo: {
    user: {
      query() {
        return this.$options.query.UserProfileByUserId;
      },
      variables() {
        return {
          id: this.$route.params.id
        };
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
query UserProfileByUserId($id: ID!) {
  user(id: $id) {
    id
    type
    profile {
      id
    }
  }
}
</graphql>
