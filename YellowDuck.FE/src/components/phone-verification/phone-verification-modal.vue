<template>
  <b-modal v-model="showModal" :title="title" hide-footer centered @hidden="resetPinCode">
    <p>{{ $t("confirm-phone.description") }}</p>
    <p v-if="validationError" class="text-danger">{{ $t("confirm-phone.invalid-code") }}</p>

    <s-form class="my-4" @submit="validatePhoneNumber">
      <s-form-input
        id="pin"
        :label="$t('label.pin')"
        :description="$t('description.pin')"
        name="pin"
        rules="required|length:6"
        v-model="pin"
        type="number"
        required
      />
      <b-button type="submit" variant="primary" class="mr-2">{{ $t("confirm-phone.validate-btn") }}</b-button>
      <b-button @click="resendCode" variant="link" :disabled="resendCountdown > 0" :class="{ 'text-muted': resendCountdown > 0 }">
        {{
          resendCountdown > 0
            ? $t("confirm-phone.resend-code-btn") + ` (${resendCountdown}s)`
            : $t("confirm-phone.resend-code-btn")
        }}
      </b-button>
    </s-form>
  </b-modal>
</template>

<script>
import SForm from "@/components/form/s-form";
import SFormInput from "@/components/form/s-form-input";
import PhoneVerificationService from "@/services/phone-verification";

export default {
  name: "PhoneVerificationModal",
  components: {
    SForm,
    SFormInput
  },
  props: {
    value: Boolean,
    phoneNumber: String,
    email: String,
    title: String
  },
  data() {
    return {
      pin: null,
      resendCountdown: 30,
      resendTimer: null,
      validationError: false
    };
  },
  computed: {
    showModal: {
      get() {
        return this.value;
      },
      set(value) {
        this.$emit("input", value);
      }
    }
  },
  methods: {
    resetPinCode() {
      this.pin = null;
      this.validationError = false;
    },
    startResendCountdown() {
      this.resendCountdown = 30;
      clearInterval(this.resendTimer);
      this.resendTimer = setInterval(() => {
        if (this.resendCountdown > 0) {
          this.resendCountdown--;
        } else {
          clearInterval(this.resendTimer);
        }
      }, 1000);
    },
    async validatePhoneNumber() {
      const success = await PhoneVerificationService.verifyValidationCode(this.phoneNumber || this.email, this.pin);
      if (success) {
        this.$emit("validation-success");
        this.showModal = false;
        this.validationError = false;
      } else {
        this.validationError = true;
      }
    },
    async resendCode() {
      if (this.resendCountdown > 0) return;
      await PhoneVerificationService.sendValidationCode(this.phoneNumber || this.email);
      this.resetPinCode();
      this.startResendCountdown();
    }
  },
  beforeDestroy() {
    clearInterval(this.resendTimer);
  },
  watch: {
    value: {
      async handler(newValue) {
        if (newValue) {
          if (this.phoneNumber) {
            // Envoyer le code de validation lors de l'ouverture de la modal
            await PhoneVerificationService.sendValidationCode(this.phoneNumber);
          }
          this.startResendCountdown();
        }
      },
      immediate: false
    }
  }
};
</script>

<style scoped>
.text-muted {
  cursor: not-allowed;
}
</style>
