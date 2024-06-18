<template>
  <div class="w-100">
    <portal :to="$consts.enums.PORTAL_HEADER">
      <nav-close :to="{ name: $consts.urls.URL_LOGIN }"></nav-close>
    </portal>
    <template v-if="emailSent">
      <form-complete
        :title="$t('form-complete.error-reset-password.title')"
        :description="$t('form-complete.error-reset-password.description', { email })"
        :image="require('@/assets/icons/adult.svg')"
      />
    </template>
    <template v-else-if="showError">
      <form-complete
        :title="$t('form-complete.error-reset-password.title')"
        :description="$t('reset-password.error-text', { email })"
        :image="require('@/assets/icons/adult.svg')"
        :ctas="formCompleteErrorCtas"
      />
    </template>
    <div v-else-if="!emailSent" class="section section--sm">
      <h1 class="my-4">{{ $t("page-title.reset-password") }}</h1>
      <s-form class="my-4" @submit="resetPassword">
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
        <b-button type="submit" variant="primary" size="lg" block>{{ $t("btn.submit") }}</b-button>
      </s-form>
    </div>
  </div>
</template>

<script>
import NavClose from "@/components/nav/close";
import SForm from "@/components/form/s-form";
import SFormInput from "@/components/form/s-form-input";
import FormComplete from "@/components/generic/form-complete";

import AuthentificationService from "@/services/authentification";
import UserService from "@/services/user";

import { TOKEN_TYPE_RESET_PASSWORD, TOKEN_STATUS_INVALID } from "@/consts/token";
import { URL_LOGIN } from "@/consts/urls";

export default {
  components: {
    NavClose,
    SForm,
    SFormInput,
    FormComplete
  },
  data() {
    return {
      token: this.$router.currentRoute.query.token,
      email: this.$router.currentRoute.query.email,
      password: "",
      passwordConfirmation: "",
      showError: false,
      emailSent: false,
      formCompleteErrorCtas: [
        {
          action: () => this.resendEmail(),
          text: this.$t("btn.resend-reset-password")
        }
      ]
    };
  },
  async mounted() {
    let result = await UserService.verifyToken(this.email, this.token, TOKEN_TYPE_RESET_PASSWORD);

    if (result.status === TOKEN_STATUS_INVALID) {
      this.showError = true;
      window.scrollTo(0, 0);
    }
  },
  methods: {
    resetPassword: async function () {
      await UserService.resetPassword(this.email, this.password, this.token);
      await AuthentificationService.login(this.email, this.password);
    },
    resendEmail: async function () {
      await UserService.sendPasswordReset(this.email);
      this.emailSent = true;
      window.scrollTo(0, 0);
    },
    returnHome: function () {
      this.$router.push({ name: URL_LOGIN });
    }
  }
};
</script>
