<template>
  <div class="w-100">
    <portal :to="$consts.enums.PORTAL_HEADER">
      <nav-close :to="{ name: $consts.urls.URL_LOGIN }"></nav-close>
    </portal>
    <div class="section section--sm">
      <h1 class="my-4">{{ $t("page-title.create-admin") }}</h1>
      <s-form class="my-4" @submit="confirmAdmin">
        <s-form-input
          id="email"
          :label="$t('label.email')"
          name="email"
          rules="required|email"
          v-model="email"
          type="email"
          disabled
          :placeholder="$t('placeholder.email')"
          required
        />
        <s-form-input
          id="password"
          :label="$t('label.password')"
          :description="$t('description.password')"
          name="password"
          rules="required|password"
          v-model="password"
          type="password"
          required
        />
        <s-form-input
          id="passwordConfirmation"
          :label="$t('label.password-confirmation')"
          name="passwordConfirmation"
          rules="required|samePassword:@password"
          v-model="passwordConfirmation"
          type="password"
          required
        />
        <s-form-input
          id="firstname"
          :label="$t('label.firstname')"
          name="firstname"
          rules="required"
          v-model="firstname"
          type="text"
          disabled
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
          disabled
          :placeholder="$t('placeholder.lastname')"
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

import AuthentificationService from "@/services/authentification";
import UserService from "@/services/user";
import NotificationService from "@/services/notification";

import i18n from "@/helpers/i18n";
import { TOKEN_TYPE_ADMIN_INVITATION, TOKEN_STATUS_USER_CONFIRMED } from "@/consts/token";
import { URL_LOGIN } from "@/consts/urls";

export default {
  components: {
    NavClose,
    SForm,
    SFormInput
  },
  data() {
    return {
      token: this.$router.currentRoute.query.token,
      email: this.$router.currentRoute.query.email,
      firstname: "",
      lastname: "",
      password: "",
      userId: "",
      passwordConfirmation: "",
      isSubmitted: false
    };
  },
  async mounted() {
    let result = await UserService.verifyToken(this.email, this.token, TOKEN_TYPE_ADMIN_INVITATION);
    let returnPath = this.$router.currentRoute.query.returnPath;

    if (result.status === TOKEN_STATUS_USER_CONFIRMED) {
      this.$router.push({ name: URL_LOGIN, query: { email: this.email, returnPath } });
    } else {
      this.userId = result.user.id;
      this.firstname = result.user.profile.firstName;
      this.lastname = result.user.profile.lastName;
    }
  },
  methods: {
    resendEmail: async function () {
      await AuthentificationService.resendConfirmationEmail(this.email);
      NotificationService.showSuccess(i18n.instance().t("notification.resend-complete", { email: this.email }));
    },
    confirmAdmin: async function () {
      this.isSubmitted = true;

      const input = {
        emailAddress: this.email,
        inviteToken: this.token,
        password: this.password
      };
      await UserService.completeAdminRegistration(input);
      await AuthentificationService.login(this.email, this.password);

      this.isSubmitted = false;
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
