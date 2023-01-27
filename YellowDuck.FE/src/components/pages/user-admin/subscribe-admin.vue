<template>
  <div class="w-100">
    <portal :to="$consts.enums.PORTAL_HEADER">
      <nav-close :to="{ name: $consts.urls.URL_LIST_USERS }"></nav-close>
    </portal>
    <div class="section section--sm">
      <h1 class="h2 my-4">{{ $t("page-title.create-admin") }}</h1>
      <s-form class="my-4" @submit="subscribeUser">
        <s-form-input
          id="firstname"
          :label="$t('label.firstname')"
          name="firstname"
          rules="required"
          v-model="firstname"
          type="text"
          :placeholder="$t('placeholder.firstname')"
          required
        />
        <s-form-input
          id="lastname"
          :label="$t('label.lastname')"
          name="lastname"
          rules="required"
          v-model="lastname"
          type="text"
          :placeholder="$t('placeholder.lastname')"
          required
        />
        <s-form-input
          id="email"
          :label="$t('label.email')"
          name="email"
          rules="required|email"
          v-model="email"
          type="email"
          :placeholder="$t('placeholder.email')"
          required
        />
        <b-button :disabled="isSubmitted" type="submit" variant="primary" size="lg" block>{{ $t("btn.submit") }}</b-button>
      </s-form>
    </div>
  </div>
</template>

<script>
import NavClose from "@/components/nav/close";
import SForm from "@/components/form/s-form";
import SFormInput from "@/components/form/s-form-input";

import { URL_LIST_USERS } from "@/consts/urls";

import UserService from "@/services/user";
import NotificationService from "@/services/notification";

export default {
  components: {
    NavClose,
    SForm,
    SFormInput
  },
  data() {
    return {
      email: "",
      firstname: "",
      lastname: "",
      isSubmitted: false
    };
  },
  methods: {
    subscribeUser: async function () {
      this.isSubmitted = true;

      let input = {
        email: this.email,
        firstName: this.firstname,
        lastName: this.lastname
      };

      await UserService.createAdminAccount(input);
      NotificationService.showSuccess(this.$t("notification.resend-complete", { email: this.email }));
      this.$router.push({ name: URL_LIST_USERS });

      this.isSubmitted = false;
    }
  }
};
</script>
