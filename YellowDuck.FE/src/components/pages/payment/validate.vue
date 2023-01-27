<template>
  <spinner />
</template>

<script>
import Spinner from "@/components/generic/spinner";

import NotificationService from "@/services/notification";

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
        return this.$options.query.IsAccountOnboardingComplete;
      },
      async result({ data }) {
        if (data) {
          if (data.me.stripeAccount.accountOnboardingComplete) {
            NotificationService.showSuccess(this.$t("notification.stripe-onboarding-complete"));
          } else {
            NotificationService.showSuccess(this.$t("notification.stripe-onboarding-not-complete"));
          }
          this.$router.push(this.redirectUrl);
        }
      }
    }
  }
};
</script>

<graphql>
query IsAccountOnboardingComplete {
  me {
    id
    stripeAccount {
      id
      accountOnboardingComplete
    }
  }
}
</graphql>