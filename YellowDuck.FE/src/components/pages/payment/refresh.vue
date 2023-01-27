<template>
  <spinner />
</template>

<script>
import Spinner from "@/components/generic/spinner";

export default {
  components: { Spinner },
  data() {
    return {
      redirectUrl: this.$route.query.redirectUrl
    };
  },
  apollo: {
    me: {
      query() {
        return this.$options.query.StripeOnboardingLink;
      },
      variables() {
        return {
          redirectUrl: this.redirectUrl
        };
      },
      async result({ data }) {
        if (data) {
          window.location = data.me.stripeAccount.accountOnboardingLink;
        }
      }
    }
  }
};
</script>

<graphql>
query StripeOnboardingLink($redirectUrl: String!) {
  me {
    id
    stripeAccount {
      id
      accountOnboardingLink(redirectUrl: $redirectUrl)
    }
  }
}
</graphql>