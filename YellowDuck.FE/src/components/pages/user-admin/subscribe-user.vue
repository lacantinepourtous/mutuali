<template>
  <div class="w-100">
    <portal v-if="!userCreated" :to="$consts.enums.PORTAL_HEADER">
      <nav-close :to="{ name: $consts.urls.URL_LIST_USERS }"></nav-close>
    </portal>
    <div v-if="!userCreated" class="section section--sm">
      <h1 class="my-4">{{ $t("page-title.create-user-as-admin") }}</h1>
      <subscribe-user-form class="my-4" @submitForm="subscribeUser" />
    </div>
  </div>
</template>

<script>
import NavClose from "@/components/nav/close";
import SubscribeUserForm from "@/components/subscription/form";

import UserService from "@/services/user";
import NotificationService from "@/services/notification";

import { URL_LIST_USERS } from "@/consts/urls";
import { VUE_APP_MUTUALI_CONTACT_MAIL } from "@/helpers/env";

export default {
  components: {
    NavClose,
    SubscribeUserForm
  },
  data() {
    return {
      userCreated: false,
      formCompleteLinks: [{ href: `mailto:${VUE_APP_MUTUALI_CONTACT_MAIL}`, text: this.$t("form-complete.contact-us.link") }],
      isSubmitted: false
    };
  },
  methods: {
    subscribeUser: async function (input) {
      this.isSubmitted = true;

      await UserService.createUserAccount(input, this.$router.currentRoute.query.returnPath);
      NotificationService.showSuccess(this.$t("notification.resend-complete", { email: input.email }));
      this.$router.push({ name: URL_LIST_USERS });

      this.isSubmitted = false;
    }
  }
};
</script>
