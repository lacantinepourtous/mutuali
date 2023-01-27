<template>
  <spinner v-if="!checkoutComplete" />
  <form-complete
    v-else
    :title="$t('form-complete.checkout-complete.title')"
    :description="$t('form-complete.checkout-complete.description')"
    :image="require('@/assets/icons/delivery-yellow-circle.png')"
    :ctas="formCompleteCtas"
  />
</template>

<script>
import Spinner from "@/components/generic/spinner";
import FormComplete from "@/components/generic/form-complete";

import PaymentService from "@/services/payment";

import { URL_ROOT, URL_CONVERSATION_DETAIL } from "@/consts/urls";

export default {
  data() {
    return {
      checkoutComplete: false,
      formCompleteCtas: [
        {
          action: () => this.$router.push({ name: URL_CONVERSATION_DETAIL, params: { id: this.conversationId } }),
          text: this.$t("btn.return-conversation")
        },
        { action: () => this.$router.push({ name: URL_ROOT }), text: this.$t("btn.return-dashboard") }
      ]
    };
  },
  components: { Spinner, FormComplete },
  mounted() {
    this.validateCheckoutSessionIsComplete();
  },
  methods: {
    validateCheckoutSessionIsComplete: async function () {
      let isComplete = await PaymentService.getCheckoutSessionIsComplete(this.contractId);
      this.checkoutComplete = isComplete;

      if (!isComplete) {
        let vm = this;
        setTimeout(function () {
          vm.validateCheckoutSessionIsComplete();
        }, 1000);
      }
    }
  },
  computed: {
    conversationId: function () {
      return this.contract != null ? this.contract.conversation.id : null;
    },
    contractId: function () {
      return this.$route.query.contractId;
    }
  },
  apollo: {
    contract: {
      query() {
        return this.$options.query.ContractById;
      },
      variables() {
        return {
          id: this.contractId
        };
      }
    }
  }
};
</script>

<graphql>
query ContractById($id: ID!) {
  contract(id: $id) {
    id
    conversation {
      id
    }
  }
}
</graphql>