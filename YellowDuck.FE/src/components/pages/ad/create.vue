<template>
  <div class="w-100">
    <portal v-if="!adCreated" :to="$consts.enums.PORTAL_HEADER">
      <nav-close :to="{ name: $consts.urls.URL_LIST_AD }"></nav-close>
    </portal>
    <template v-if="!adCreated">
      <div class="section section--md section--padding-x section--border-bottom my-4">
        <h1 class="my-4">{{ $t("page-title.create-ad") }}</h1>
      </div>

      <div v-if="!phoneNumberConfirmed" class="alert alert-warning mx-4">
        {{ $t("warnings.phone-verification-required") }}
        <router-link :to="{ name: $consts.urls.URL_PROFILE_EDIT, query: { action: 'validate-phone' } }">
          {{ $t("warnings.phone-verification-required.link") }}
        </router-link>
      </div>

      <ad-form @submitForm="createAd" :disabledBtn="isSubmitted || !phoneNumberConfirmed" :btnLabel="$t('btn.create-ad')" />
    </template>

    <form-complete
      v-else
      :title="$t('form-complete.create-ad.title')"
      :description="$t('form-complete.create-ad.description')"
      :image="require('@/assets/icons/checklist-yellow.svg')"
      :ctas="formCompleteCtas"
    />
  </div>
</template>

<script>
import NavClose from "@/components/nav/close";
import FormComplete from "@/components/generic/form-complete";
import AdForm from "@/components/ad/form";

import { URL_ROOT, URL_AD_DETAIL } from "@/consts/urls";

import { createAd } from "@/services/ad";

export default {
  components: {
    AdForm,
    FormComplete,
    NavClose
  },
  data() {
    return {
      adCreated: false,
      adCreatedId: "",
      isSubmitted: false,
      phoneNumberConfirmed: true,
      formCompleteCtas: [
        {
          action: () => this.$router.push({ name: URL_AD_DETAIL, params: { id: this.adCreatedId } }),
          text: this.$t("btn.display-detail-ad")
        },
        { action: () => this.$router.push({ name: URL_ROOT }), text: this.$t("btn.return-dashboard") }
      ]
    };
  },
  gqlErrors() {
    return {
      IMAGE_NOT_FOUND(error) {
        return this.$t("error.image-upload");
      }
    };
  },
  apollo: {
    me: {
      query() {
        return this.$options.query.Me;
      },
      result({ data }) {
        if (data && data.me) {
          this.phoneNumberConfirmed = data.me.phoneNumberConfirmed;
        }
      }
    }
  },
  methods: {
    createAd: async function (input) {
      this.isSubmitted = true;
      let result = await createAd(input);

      if (result) {
        this.adCreatedId = result.data.createAd.ad.id;
        this.adCreated = true;
        window.scrollTo(0, 0);
      }
      this.isSubmitted = false;
    }
  }
};
</script>

<graphql>
  query Me {
    me {
      id
      phoneNumberConfirmed
    }
  }
  </graphql>
