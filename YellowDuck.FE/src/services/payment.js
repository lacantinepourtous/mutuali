import Apollo from "@/graphql/vue-apollo";
import {
  GetStripeOnboardingLink,
  GetCheckoutSessionIsComplete,
  CreateStripeAccount,
  CreateCheckoutSession,
  CancelCheckoutSession
} from "./payment.graphql";

export default {
  createStripeAccount: async function() {
    let result = await Apollo.instance.defaultClient.mutate({
      mutation: CreateStripeAccount,
      variables: {
        input: {}
      }
    });

    return result;
  },
  getStripeOnboardingLink: async function(redirectUrl) {
    let result = await Apollo.instance.defaultClient.query({
      fetchPolicy: "network-only",
      query: GetStripeOnboardingLink,
      variables: {
        redirectUrl
      }
    });

    return result.data.me.stripeAccount.accountOnboardingLink;
  },
  getCheckoutSessionIsComplete: async function(contractId) {
    let result = await Apollo.instance.defaultClient.query({
      fetchPolicy: "network-only",
      query: GetCheckoutSessionIsComplete,
      variables: {
        id: contractId
      }
    });

    return result.data.contract.checkoutSession.checkoutSessionComplete;
  },
  createCheckoutSession: async function(contractId) {
    let result = await Apollo.instance.defaultClient.mutate({
      mutation: CreateCheckoutSession,
      variables: {
        input: {
          contractId
        }
      }
    });

    return result;
  },
  cancelCheckoutSession: async function(contractId) {
    let result = await Apollo.instance.defaultClient.mutate({
      mutation: CancelCheckoutSession,
      variables: {
        input: {
          contractId
        }
      }
    });

    return result.data;
  }
};
