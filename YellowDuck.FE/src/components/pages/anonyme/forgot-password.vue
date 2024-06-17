<template>
  <div class="w-100">
    <portal v-if="!emailSent" :to="$consts.enums.PORTAL_HEADER">
      <nav-close :to="{ name: $consts.urls.URL_LOGIN }"></nav-close>
    </portal>
    <div v-if="!emailSent" class="section section--sm">
      <h1 class="my-4">{{ $t("page-title.forgot-password") }}</h1>
      <s-form class="my-4" @submit="sendPasswordReset">
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
    <form-complete
      v-else
      :title="$t('form-complete.reset-password.title')"
      :description="$t('form-complete.reset-password.description', { email })"
      :image="require('@/assets/icons/adult.svg')"
    />
  </div>
</template>

<script>
import NavClose from "@/components/nav/close";
import SForm from "@/components/form/s-form";
import SFormInput from "@/components/form/s-form-input";
import FormComplete from "@/components/generic/form-complete";

import UserService from "@/services/user";

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
      emailSent: false,
      email: "",
      isSubmitted: false
    };
  },
  methods: {
    sendPasswordReset: async function () {
      this.isSubmitted = true;
      await UserService.sendPasswordReset(this.email);
      this.emailSent = true;
      window.scrollTo(0, 0);
      this.isSubmitted = false;
    },
    returnHome: function () {
      this.$router.push({ name: URL_LOGIN });
    }
  }
};
</script>
