<template>
  <div class="w-100">
    <div class="section section--sm">
      <h1 class="h2 my-4">{{ $t("page-title.login") }}</h1>
      <s-form class="my-4" @submit="login">
        <s-form-input
          id="email"
          :label="$t('label.username')"
          name="email"
          rules="required|email"
          v-model="email"
          type="email"
          :placeholder="$t('placeholder.email')"
          required
        />
        <s-form-input
          id="password"
          :label="$t('label.password')"
          name="password"
          rules="required"
          v-model="password"
          type="password"
          required
        />
        <b-button type="submit" variant="admin" size="lg" block>{{ $t("btn.login-submit") }}</b-button>
        <b-button variant="outline-primary" block class="mt-4" :to="{ name: $consts.urls.URL_FORGOT_PASSWORD }">{{
          $t("btn.forgot-password")
        }}</b-button>
      </s-form>
    </div>
    <div class="section section--sm section--border-top">
      <div class="my-4">
        <h2 class="h3 text-primary">{{ $t("page-title.no-account") }}</h2>
        <p v-html="$t('login.lead-no-accound')"></p>
        <b-button tag="a" :to="{ name: $consts.urls.URL_USER_SUBSCRIBE, query: { returnPath } }" variant="outline-primary">{{
          $t("btn.create-user")
        }}</b-button>
      </div>
    </div>
  </div>
</template>

<script>
import SForm from "@/components/form/s-form";
import SFormInput from "@/components/form/s-form-input";
import AuthentificationService from "@/services/authentification";

export default {
  components: { SForm, SFormInput },
  data() {
    return {
      email: this.$router.currentRoute.query.email || "",
      password: ""
    };
  },
  computed: {
    returnPath() {
      return this.$router.currentRoute.query.returnPath;
    }
  },
  methods: {
    login: async function () {
      await AuthentificationService.login(this.email, this.password);
    }
  }
};
</script>
