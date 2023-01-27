<template>
  <div class="w-100" v-if="contract">
    <portal v-if="!contractRated" :to="$consts.enums.PORTAL_HEADER">
      <nav-close :to="{ name: $consts.urls.URL_CONVERSATION_DETAIL, params: { id: this.conversationId } }"></nav-close>
    </portal>

    <div v-if="!contractRated">
      <div class="section section--sm">
        <h1 class="mt-4 mb-3">{{ $t("page-title.rate-contract") }}</h1>
        <p>{{ $t("rate-contract.text") }}</p>
      </div>

      <s-form @submit="rateContract">
        <template v-if="raterIsOwner">
          <div v-show="currentStep === $consts.ratingSteps.STEP_TENANT" class="rating-step">
            <div class="section section--sm mt-4">
              <h2 class="font-family-base font-weight-bold mb-4">{{ $t("rate-contract.rate-tennant-title") }}</h2>
            </div>
            <hr class="my-0" />
            <!-- TODO : Add profile img to user-profile-snippet props -->
            <user-profile-snippet :id="tenant.profile.id" sectionWidth="sm" class="section--border-bottom py-4" />
            <div class="section section--sm mb-4">
              <div class="mt-4 mb-5">
                <s-form-rating v-model="rating.user.respect" :label="$t('label.respect-rating')" size="lg" margin="sm" />
                <s-form-rating
                  v-model="rating.user.communication"
                  :label="$t('label.communication-rating')"
                  size="lg"
                  margin="sm"
                />
                <s-form-rating v-model="rating.user.fiability" :label="$t('label.fiability-rating')" size="lg" margin="sm" />
              </div>

              <b-button :disabled="isSubmitted" type="submit" variant="primary" size="lg" block>{{
                $t("btn-rate-contract")
              }}</b-button>
            </div>
          </div>
        </template>

        <template v-else>
          <div v-show="currentStep === $consts.ratingSteps.STEP_EQUIPEMENT" class="rating-step">
            <div class="section section--sm mt-4">
              <span><small>1/2</small></span>
              <h2 class="font-family-base font-weight-bold mt-1 mb-4">{{ $t("rate-contract.rate-equipement-title") }}</h2>
            </div>
            <hr class="my-0" />
            <ad-snippet
              :id="adId"
              :title="adTitle"
              :image="adImage"
              :organization="adOrganization"
              sectionWidth="sm"
              hide-rating
            />
            <div class="section section--sm mb-4">
              <div class="mt-4 mb-5">
                <s-form-rating v-model="rating.ad.compliance" :label="$t('label.compliance-rating')" size="lg" margin="sm" />
                <s-form-rating v-model="rating.ad.cleanliness" :label="$t('label.cleanliness-rating')" size="lg" margin="sm" />
                <s-form-rating v-model="rating.ad.security" :label="$t('label.security-rating')" size="lg" margin="sm" />
              </div>
              <b-button @click="currentStep = $consts.ratingSteps.STEP_OWNER" variant="primary" size="lg" block>{{
                $t("btn-rate-next-step")
              }}</b-button>
            </div>
          </div>

          <div v-show="currentStep === $consts.ratingSteps.STEP_OWNER" class="rating-step">
            <div class="section section--sm mt-4">
              <span><small>2/2</small></span>
              <h2 class="font-family-base font-weight-bold mt-1 mb-4">{{ $t("rate-contract.rate-owner-title") }}</h2>
            </div>
            <hr class="my-0" />
            <!-- TODO : Add profile img to user-profile-snippet props -->
            <user-profile-snippet :id="owner.profile.id" sectionWidth="sm" hide-rating class="section--border-bottom py-4" />
            <div class="section section--sm mb-4">
              <div class="mt-4 mb-5">
                <s-form-rating v-model="rating.user.respect" :label="$t('label.respect-rating')" size="lg" margin="sm" />
                <s-form-rating
                  v-model="rating.user.communication"
                  :label="$t('label.communication-rating')"
                  size="lg"
                  margin="sm"
                />
                <s-form-rating v-model="rating.user.fiability" :label="$t('label.fiability-rating')" size="lg" margin="sm" />
              </div>

              <b-button @click="currentStep = $consts.ratingSteps.STEP_EQUIPEMENT" size="lg" block>{{
                $t("btn-rate-previous-step")
              }}</b-button>
              <b-button :disabled="isSubmitted" type="submit" variant="primary" size="lg" block>{{
                $t("btn-rate-contract")
              }}</b-button>
            </div>
          </div>
        </template>
      </s-form>
    </div>
    <form-complete
      v-else
      :title="$t('form-complete.rate-contract.title')"
      :description="$t('form-complete.rate-contract.description')"
      :image="require('@/assets/icons/checklist.png')"
      :ctas="formCompleteCtas"
    />
  </div>
</template>

<script>
import NavClose from "@/components/nav/close";
import AdSnippet from "@/components/ad/snippet";
import UserProfileSnippet from "@/components/user-profile/snippet";
import SForm from "@/components/form/s-form";
import SFormRating from "@/components/form/s-form-rating";
import FormComplete from "@/components/generic/form-complete";

import { CONTENT_LANG_FR } from "@/consts/langs";
import { URL_CONVERSATION_DETAIL, URL_LIST_AD } from "@/consts/urls";
import { STEP_EQUIPEMENT, STEP_TENANT } from "@/consts/rating-steps";
import { CONTRACT_STATUS_CLOSED } from "@/consts/contract-status";

import { rateContract } from "@/services/contract";

export default {
  components: {
    FormComplete,
    SForm,
    SFormRating,
    NavClose,
    AdSnippet,
    UserProfileSnippet
  },
  data() {
    return {
      currentStep: "",
      rating: {
        ad: {
          compliance: 0,
          cleanliness: 0,
          security: 0
        },
        user: {
          respect: 0,
          communication: 0,
          fiability: 0
        }
      },
      isSubmitted: false,
      formCompleteCtas: [
        {
          action: () => this.$router.push({ name: URL_LIST_AD }),
          text: this.$t("btn.return-map")
        },
        {
          action: () => this.$router.push({ name: URL_CONVERSATION_DETAIL, params: { id: this.conversationId } }),
          text: this.$t("btn.return-conversation")
        }
      ]
    };
  },
  methods: {
    rateContract: async function() {
      this.isSubmitted = true;
      let input = {
        contractId: this.contractId,
        adRating: this.rating.ad,
        userRating: this.rating.user
      };
      await rateContract(input);
      window.scrollTo(0, 0);
      this.isSubmitted = false;
    }
  },
  computed: {
    contractId: function() {
      return this.$route.params.id.split("-").last();
    },
    conversationId: function() {
      return this.contract.conversation.id;
    },
    adId: function() {
      return this.contract.conversation.ad.id;
    },
    adOrganization: function() {
      return this.contract.conversation.ad.organization;
    },
    adTitle: function() {
      return this.contract.conversation.ad.translationOrDefault.title;
    },
    adImage: function() {
      return this.contract.conversation.ad.gallery[0];
    },
    contractRated: function() {
      return this.raterIsOwner ? this.hasAlreadyRateUser : Boolean(this.contract.adRating) || this.hasAlreadyRateUser;
    },
    tenant: function() {
      return this.contract.tenant;
    },
    owner: function() {
      return this.contract.owner;
    },
    raterIsOwner: function() {
      return this.me.id === this.owner.id;
    },
    hasAlreadyRateUser: function() {
      return this.contract.userRatings.some((x) => x.raterUser.id === this.me.id);
    }
  },
  apollo: {
    me: {
      query() {
        return this.$options.query.Me;
      }
    },
    contract: {
      query() {
        return this.$options.query.ContractById;
      },
      variables() {
        return {
          id: this.contractId,
          language: CONTENT_LANG_FR
        };
      },
      result({ data }) {
        if (data) {
          if (this.contract.status != CONTRACT_STATUS_CLOSED) {
            this.$router.replace({ name: this.$consts.urls.URL_404 });
          }
          this.currentStep = this.raterIsOwner ? STEP_TENANT : STEP_EQUIPEMENT;
        }
      }
    }
  }
};
</script>

<graphql>
query ContractById($id: ID!, $language: ContentLanguage!) {
  contract(id: $id) {
    id
    status
    conversation {
      id
      ad {
        id
        gallery {
          id
          src
          alt
        }
        translationOrDefault(language: $language) {
          id
          title
        }
        user {
          id
        }
        organization
      }
    }
    adRating {
      id
    }
    userRatings {
      id
      raterUser {
        id
      }
    }
    owner {
      id
      profile {
        id
        ... on UserProfileGraphType {
          publicName
        }
      }
    }
    tenant {
      id
      profile {
        id
        ... on UserProfileGraphType {
          publicName
        }
      }
    }
  }
}

query Me {
  me {
    id
  }
}
</graphql>
