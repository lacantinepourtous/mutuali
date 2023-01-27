<template>
  <spinner v-if="isLoading" />
  <form-complete
    v-else-if="checkoutCancelled"
    :title="$t('form-complete.cancel-checkout.title')"
    :description="$t('form-complete.cancel-checkout.description')"
    :image="require('@/assets/icons/aucune_commande.png')"
    :ctas="formCompleteCtas"
  />
  <form-complete
    v-else
    :title="$t('form-complete.cancel-checkout-error.title')"
    :description="$t('form-complete.cancel-checkout-error.description')"
    :image="require('@/assets/icons/institution.png')"
    :ctas="formErrorCtas"
  />
</template>

<script>
import Spinner from "@/components/generic/spinner";
import FormComplete from "@/components/generic/form-complete";

import { URL_ROOT, URL_CONTRACT_DETAIL } from "@/consts/urls";
import { VUE_APP_MUTUALI_CONTACT_MAIL } from "@/helpers/env";

import PaymentService from "@/services/payment";

export default {
  components: { Spinner, FormComplete },
  data() {
    return {
      isLoading: true,
      checkoutCancelled: false,
      formCompleteCtas: [
        {
          action: () => this.$router.push({ name: URL_CONTRACT_DETAIL, params: { id: this.$route.query.contractId } }),
          text: this.$t("btn.display-detail-contract")
        },
        { action: () => this.$router.push({ name: URL_ROOT }), text: this.$t("btn.return-dashboard") }
      ],
      formErrorCtas: [
        {
          action: () =>
            (window.location.href =
              "mailto:" + VUE_APP_MUTUALI_CONTACT_MAIL + "?subject=" + this.$t("email.cancel-payment-problem.subject")),
          text: this.$t("btn.contact-us")
        },
        { action: () => this.$router.push({ name: URL_ROOT }), text: this.$t("btn.return-dashboard") }
      ]
    };
  },
  async mounted() {
    let result = await PaymentService.cancelCheckoutSession(this.$route.query.contractId);
    this.checkoutCancelled = result.cancelCheckoutSession.checkoutSession.checkoutSessionCancel;
    this.isLoading = false;
  },
  gqlErrors() {
    return {
      CHECKOUT_SESSION_NOT_FOUND(error) {
        this.isLoading = false;
        return this.$t("error.cancel-payment");
      },
      CHECKOUT_SESSION_ALREADY_CANCEL(error) {
        this.isLoading = false;
        return this.$t("error.cancel-payment");
      },
      CHECKOUT_SESSION_ALREADY_SUCCEEDED(error) {
        this.isLoading = false;
        return this.$t("error.cancel-payment");
      },
      CANT_CANCEL_STRIPE_CHECKOUT_SESSION(error) {
        this.isLoading = false;
        return this.$t("error.cancel-payment");
      }
    };
  }
};
</script>
