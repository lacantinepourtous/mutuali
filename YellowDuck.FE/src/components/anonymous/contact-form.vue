<template>
  <div>
    <s-form @submit="submitForm">
      <s-form-input
        v-model="form.fullName"
        id="fullName"
        :label="$t('contactform.fullname-label')"
        name="fullName"
        rules="required"
        required
        :disabled="formDisabled"
      />
      <s-form-input
        v-model="form.organizationName"
        id="organizationName"
        :label="$t('contactform.organizationName-label')"
        name="organizationName"
        rules="required"
        required
        :disabled="formDisabled"
      />
      <s-form-input
        v-model="form.emailOrPhone"
        id="emailOrPhone"
        :label="$t('contactform.emailOrPhone-label')"
        name="emailOrPhone"
        rules="required"
        :description="$t('contactform.emailOrPhone-description')"
        required
        :disabled="formDisabled"
      />
      <b-button type="submit" variant="primary" size="lg" :disabled="formDisabled">
        <template v-if="form.isSent"> {{ $t("contactform.sent-btn") }} </template>
        <template v-else-if="form.isSending"> {{ $t("contactform.sending-btn") }} </template>
        <template v-else> {{ $t("contactform.send-btn") }} </template>
      </b-button>
    </s-form>
  </div>
</template>

<script>
import SForm from "@/components/form/s-form";
import SFormInput from "@/components/form/s-form-input";

import NotificationService from "@/services/notification";

import { VUE_APP_ROOT_API } from "@/helpers/env";

export default {
  components: {
    SForm,
    SFormInput
  },
  props: {
    origin: {
      type: String,
      default: "Général"
    }
  },
  data() {
    return {
      form: {
        isSending: false,
        isSent: false,
        fullName: "",
        organizationName: "",
        emailOrPhone: ""
      }
    };
  },
  computed: {
    formDisabled() {
      return this.form.isSending || this.form.isSent;
    }
  },
  methods: {
    submitForm: async function () {
      this.form.isSending = true;
      try {
        let response = await fetch(`${VUE_APP_ROOT_API}/contact`, {
          method: "POST",
          headers: {
            Accept: "application/json",
            "Content-Type": "application/json"
          },
          body: JSON.stringify({
            fullName: this.form.fullName,
            organizationName: this.form.organizationName,
            emailOrPhone: this.form.emailOrPhone,
            origin: this.origin
          })
        });
        if (response.status < 200 || response.status >= 300) {
          throw new Error("Erreur serveur inatendue lors de l'envoie du courriel");
        }
        NotificationService.showInfo(this.$t("notification.contact-form-success"));
        this.form.isSent = true;
      } catch (error) {
        NotificationService.showError(this.$t("notification.contact-form-error"));
      }
      this.form.isSending = false;
    }
  }
};
</script>
