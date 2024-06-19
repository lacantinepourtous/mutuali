<template>
  <div class="w-100">
    <portal v-if="!userCreated" :to="$consts.enums.PORTAL_HEADER">
      <nav-close :to="{ name: $consts.urls.URL_LOGIN }"></nav-close>
    </portal>
    <div v-if="!userCreated" class="section section--sm">
      <h1 class="my-4">{{ $t("page-title.create-user") }}</h1>
      <subscribe-user-form class="my-4" @submitForm="subscribeUser" />
    </div>
    <form-complete
      v-else
      :title="$t('form-complete.create-account.title')"
      :description="$t('form-complete.create-account.description')"
      :htmlTitle="$t('form-complete.create-account.html-title')"
      :htmlDescription="
        $t('form-complete.create-account.html-description', {
          email: encodeURIComponent(email)
        })
      "
      :image="require('@/assets/icons/adult.svg')"
      :linkTitle="$t('form-complete.link-title')"
      :links="formCompleteLinks"
    />
  </div>
</template>

<script>
import NavClose from "@/components/nav/close";
import FormComplete from "@/components/generic/form-complete";
import SubscribeUserForm from "@/components/subscription/form";

import UserService from "@/services/user";

import { VUE_APP_MUTUALI_CONTACT_MAIL } from "@/helpers/env";

export default {
  components: {
    NavClose,
    SubscribeUserForm,
    FormComplete
  },
  data() {
    return {
      userCreated: false,
      formCompleteLinks: [
        {
          href: `mailto:${VUE_APP_MUTUALI_CONTACT_MAIL}`,
          text: this.$t("form-complete.contact-us.link")
        }
      ],
      isSubmitting: false,
      email: ""
    };
  },
  methods: {
    subscribeUser: async function (input) {
      this.isSubmitting = true;
      await UserService.createUserAccount(input, this.$router.currentRoute.query.returnPath);
      this.userCreated = true;
      this.email = input.email;
      window.scrollTo(0, 0);

      this.isSubmitting = false;
    }
  },
  gqlErrors() {
    return {
      IDENTITY_RESULT(error) {
        return error.extensions.data.DuplicateEmail ? this.$t("error.duplicate-email") : this.$t("error.unexpected");
      }
    };
  }
};
</script>
