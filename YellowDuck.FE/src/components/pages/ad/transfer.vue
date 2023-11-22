<template>
  <div class="transfer fab-container w-100 mt-4 mt-md-5">
    <template v-if="!adTransfered">
      <portal :to="$consts.enums.PORTAL_HEADER">
        <nav-close :to="{ name: $consts.urls.URL_AD_DETAIL, params: { id: this.adId } }"></nav-close>
      </portal>

      <div class="section section--md section--padding-x my-4">
        <h1 class="my-4">{{ $t("page-title.transfer-ad") }}</h1>
      </div>

      <s-form @submit="transferAd" class="transfer__form">
        <div class="transfer__form-content section section--md section--padding-x my-4 pb-3 rm-child-margin">
          <s-form-input
            v-model="userEmail"
            id="userEmail"
            :label="$t('label.ad-transfer-user-email')"
            name="userEmail"
            rules="email|required"
            :placeholder="$t('placeholder.ad-transfer-user-email')"
            required
          />
        </div>
        <div class="fab-container__fab">
          <div class="section section--md">
            <b-button :disabled="disabledBtn" type="submit" variant="admin" size="lg" block :aria-label="$t('sr.confirm')">
              {{ $t("btn.confirm-ad-transfer") }}
            </b-button>
          </div>
        </div>
      </s-form>
    </template>
    <form-complete
      v-else
      :title="$t('form-complete.transfer-ad.title')"
      :description="$t('form-complete.transfer-ad.description')"
      :image="require('@/assets/icons/checklist.png')"
      :ctas="formCompleteCtas"
    />
  </div>
</template>

<script>
import NavClose from "@/components/nav/close";
import SForm from "@/components/form/s-form";
import SFormInput from "@/components/form/s-form-input";
import FormComplete from "@/components/generic/form-complete";

import { URL_ROOT } from "@/consts/urls";

import { transferAd } from "@/services/ad";

export default {
  components: {
    NavClose,
    SForm,
    SFormInput,
    FormComplete
  },
  data() {
    return {
      userEmail: "",
      adTransfered: false,
      isSubmitted: false,
      formCompleteCtas: [{ action: () => this.$router.push({ name: URL_ROOT }), text: this.$t("btn.return-dashboard") }]
    };
  },
  computed: {
    adId() {
      return this.$route.params.id.split("-").last();
    },
    disabledBtn() {
      return this.userEmail === "";
    }
  },
  methods: {
    async transferAd() {
      this.isSubmitted = true;
      const input = {
        adId: this.adId,
        userEmail: this.userEmail
      };
      await transferAd(input);
      this.adTransfered = true;
      window.scrollTo(0, 0);
      this.isSubmitted = false;
    }
  },
  gqlErrors() {
    return {
      USER_NOT_FOUND(error) {
        return this.$t("error.transfer-user-not-found");
      }
    };
  }
};
</script>

<style lang="scss">
.transfer {
  &__form {
    .form-group > label {
      font-size: 20px;
      font-weight: 500;
      display: block;
      padding-top: 0;
      padding-bottom: calc(0.375rem + 1px);
    }
  }
}
</style>