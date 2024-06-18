<template>
  <div style="display: contents">
    <spinner v-if="isLoading" />
    <form-complete
      v-else
      :title="$t('add-payment.title')"
      :description="$t('add-payment.description')"
      :image="require('@/assets/icons/adult.svg')"
      :ctas="formCompleteCtas"
    />
  </div>
</template>

<script>
import { URL_CONVERSATION_DETAIL, URL_CREATE_CONTRACT } from "@/consts/urls";

import PaymentService from "@/services/payment";

import Spinner from "@/components/generic/spinner";
import FormComplete from "@/components/generic/form-complete";

export default {
  components: {
    Spinner,
    FormComplete
  },
  data: function () {
    return {
      isLoading: true,
      conversationId: this.$route.params.id.split("-").last(),
      formCompleteCtas: [
        {
          action: () => this.redirectToStripeOnboarding(),
          text: this.$t("btn.add-payment-info")
        },
        {
          action: () => this.$router.push({ name: URL_CONVERSATION_DETAIL, params: { id: this.conversationId } }),
          text: this.$t("btn.cancel-add-payment-info")
        }
      ]
    };
  },
  methods: {
    redirectToStripeOnboarding: async function () {
      let stripeOnboardingLink = await PaymentService.getStripeOnboardingLink(this.$route.fullPath);
      window.location = stripeOnboardingLink;
    }
  },
  apollo: {
    me: {
      query() {
        return this.$options.query.Me;
      },
      async result({ data }) {
        if (data) {
          if (data.me.stripeAccount === null) {
            await PaymentService.createStripeAccount();
          } else if (data.me.stripeAccount.accountOnboardingComplete) {
            this.$router.push({ name: URL_CREATE_CONTRACT, params: { id: this.conversationId } });
          }

          this.isLoading = false;
        }
      }
    }
  }
};
</script>

<graphql>
query Me {
  me {
    id
    stripeAccount {
      id
      accountOnboardingComplete
    }
  }
}
</graphql>