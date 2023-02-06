<template>
  <div class="section section--sm my-4 w-100">
    <h1 class="my-4">{{ $t("page-title.confirm-email") }}</h1>
    <template v-if="showError">
      <p class="lead">{{ $t("confirm-email.error-text") }}</p>
      <b-button variant="primary" block @click="resendEmail">{{ $t("btn.resend-confirm-email") }}</b-button>
    </template>
    <template v-else>
      <p class="lead">{{ $t("confirm-email.confirm-text") }}</p>
    </template>
  </div>
</template>

<script>
import AuthentificationService from "@/services/authentification";
import UserService from "@/services/user";
import NotificationService from "@/services/notification";

import i18n from "@/helpers/i18n";
import { TOKEN_STATUS_INVALID, TOKEN_TYPE_CONFIRM_EMAIL, TOKEN_STATUS_USER_CONFIRMED } from "@/consts/token";
import { URL_LOGIN } from "@/consts/urls";

export default {
  data() {
    return {
      token: this.$router.currentRoute.query.token,
      email: this.$router.currentRoute.query.email,
      showError: false,
      resendEmailDisabled: false
    };
  },
  async mounted() {
    let result = await UserService.verifyToken(this.email, this.token, TOKEN_TYPE_CONFIRM_EMAIL);
    let returnPath = this.$router.currentRoute.query.returnPath;

    if (result.status === TOKEN_STATUS_INVALID) {
      this.showError = true;
    } else if (result.status === TOKEN_STATUS_USER_CONFIRMED) {
      this.$router.push({ name: URL_LOGIN, query: { email: this.email, returnPath } });
    } else {
      await AuthentificationService.confirmEmail(this.token, this.email);
      NotificationService.showInfo(i18n.instance().t("notification.confirm-email"));
      this.$router.push({ name: URL_LOGIN, query: { email: this.email, returnPath } });
    }
  },
  methods: {
    resendEmail: async function () {
      await AuthentificationService.resendConfirmationEmail(this.email);
      NotificationService.showSuccess(i18n.instance().t("notification.resend-complete", { email: this.email }));
      this.resendEmailDisabled = true;
    }
  },
  gqlErrors() {
    return {
      NO_NEED_TO_CONFIRM(error) {
        this.$router.push({ name: URL_LOGIN, params: { email: this.email } });
      }
    };
  }
};
</script>
