<template>
  <div class="w-100">
    <div class="section section--sm">
      <h1 class="my-4">{{ $t("page-title.account-settings") }}</h1>
      <h2 class="h4 my-4">{{ $t("page-subtitle.account-settings.change-password") }}</h2>
      <s-form class="my-4" @submit="saveAccountSettings">
        <s-form-input
          id="currentPassword"
          :label="$t('label.old-password')"
          name="current-password"
          rules="required"
          v-model="currentPassword"
          type="password"
          required
        />
        <s-form-input
          id="newPassword"
          :label="$t('label.new-password')"
          :description="$t('description.password')"
          name="newPassword"
          rules="required|password"
          v-model="newPassword"
          type="password"
          required
        />
        <s-form-input
          id="passwordConfirmation"
          :label="$t('label.new-password-confirmation')"
          name="passwordConfirmation"
          rules="required|samePassword:@newPassword"
          v-model="passwordConfirmation"
          type="password"
          required
        />
        <h2 class="h4 my-4">{{ $t("page-subtitle.account-settings.change-email") }}</h2>
        <s-form-input id="emailLinkedToAccount" :label="$t('label.email')" label-sr-only name="email" v-model="email" disabled />
        <b-button :disabled="!me" type="submit" variant="primary" size="lg" block>{{ $t("btn.update") }}</b-button>
      </s-form>
    </div>
  </div>
</template>

<script>
import SForm from "@/components/form/s-form";
import SFormInput from "@/components/form/s-form-input";

import UserService from "@/services/user";
import NotificationService from "@/services/notification";

export default {
  components: {
    SForm,
    SFormInput
  },
  data() {
    return {
      currentPassword: "",
      newPassword: "",
      passwordConfirmation: ""
    };
  },
  apollo: {
    me: {
      query() {
        return this.$options.query.Me;
      }
    }
  },
  gqlErrors() {
    return {
      IDENTITY_RESULT(error) {
        return error.extensions.data.PasswordMismatch ? this.$t("error.incorrect-current-password") : this.$t("error.unexpected");
      }
    };
  },
  methods: {
    saveAccountSettings: async function () {
      let input = {
        userId: this.me.id,
        currentPassword: this.currentPassword,
        newPassword: this.newPassword
      };
      await UserService.saveAccountSettings(input);
      NotificationService.showSuccess(this.$t("notification.account-settings-saved"));
      window.scrollTo(0, 0);
    }
  },
  computed: {
    email: function () {
      return this.me !== undefined ? this.me.email : "";
    }
  }
};
</script>
<graphql>
query Me {
  me {
    id
    email
    profile {
      id
    }
  }
}
</graphql>
