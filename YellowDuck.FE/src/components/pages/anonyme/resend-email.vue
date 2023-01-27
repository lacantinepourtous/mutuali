<template>
  <div class="section section--sm my-4 w-100">
    <p class="lead">{{ $t("form-complete.create-account.description") }}</p>
  </div>
</template>

<script>
import AuthentificationService from "@/services/authentification";
import NotificationService from "@/services/notification";

import { URL_LOGIN } from "@/consts/urls";

export default {
  computed: {
    email: function () {
      return this.$route.query.email;
    }
  },
  async mounted() {
    await AuthentificationService.resendConfirmationEmail(this.email);
    NotificationService.showSuccess(this.$t("notification.resend-complete", { email: this.email }));
    this.$router.push({ name: URL_LOGIN });
  }
};
</script>