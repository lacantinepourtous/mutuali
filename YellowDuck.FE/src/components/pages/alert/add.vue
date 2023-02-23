<template>
  <div class="w-100">
    <portal v-if="!alertCreated" :to="$consts.enums.PORTAL_HEADER">
      <nav-close :to="{ name: $consts.urls.URL_LIST_AD }"></nav-close>
    </portal>
    <template v-if="!alertCreated">
      <div class="section section--md section--padding-x section--border-bottom my-4">
        <h1 class="my-4">{{ $t("page-title.add-alert") }}</h1>
      </div>
      <alert-form @submitForm="createAlert" :disabledBtn="isSubmitted" :btnLabel="$t('btn.save-alert')" />
    </template>

    <form-complete
      v-else
      :title="$t('form-complete.create-alert.title')"
      :description="$t('form-complete.create-alert.description')"
      :image="require('@/assets/icons/checklist.png')"
      :ctas="formCompleteCtas"
    />
  </div>
</template>

<script>
import NavClose from "@/components/nav/close";
import FormComplete from "@/components/generic/form-complete";
import AlertForm from "@/components/alert/form";

import { URL_ROOT } from "@/consts/urls";

import { createAlert } from "@/services/alert";

export default {
  components: {
    AlertForm,
    FormComplete,
    NavClose
  },
  data() {
    return {
      alertCreated: false,
      isSubmitted: false,
      formCompleteCtas: [{ action: () => this.$router.push({ name: URL_ROOT }), text: this.$t("btn.return-dashboard") }]
    };
  },
  gqlErrors() {
    return {
      IMAGE_NOT_FOUND(error) {
        return this.$t("error.image-upload");
      }
    };
  },
  methods: {
    createAlert: async function(input) {
      this.isSubmitted = true;
      let result = await createAlert(input);

      if (result) {
        this.alertCreated = true;
        window.scrollTo(0, 0);
      }
      this.isSubmitted = false;
    }
  }
};
</script>
