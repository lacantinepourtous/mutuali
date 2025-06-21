<template>
  <div class="w-100" v-if="conversation">
    <portal v-if="!conversationRated" :to="$consts.enums.PORTAL_HEADER">
      <nav-close :to="{ name: $consts.urls.URL_CONVERSATION_DETAIL, params: { id: this.conversationId } }"></nav-close>
    </portal>

    <div v-if="!conversationRated">
      <div class="section section--sm">
        <h1 class="mt-4 mb-3">{{ $t("page-title.rate-conversation") }}</h1>
        <p>{{ $t("rate-conversation.text") }}</p>
      </div>

      <s-form @submit="rateConversation">
        <template v-if="raterIsOwner">
          <div v-show="currentStep === $consts.ratingSteps.STEP_TENANT" class="rating-step">
            <div class="section section--sm mt-4">
              <h2 class="font-family-base font-weight-bold mb-4">{{ $t("rate-conversation.rate-tennant-title") }}</h2>
            </div>
            <hr class="my-0" />
            <user-profile-snippet :id="requester.user.profile.id" sectionWidth="sm" class="section--border-bottom py-4" />
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
                $t("btn-rate-conversation")
              }}</b-button>
            </div>
          </div>
        </template>

        <template v-else>
          <div v-show="currentStep === $consts.ratingSteps.STEP_EQUIPEMENT" class="rating-step">
            <div class="section section--sm mt-4">
              <span><small>1/2</small></span>
              <h2 class="font-family-base font-weight-bold mt-1 mb-4">{{ $t("rate-conversation.rate-equipement-title") }}</h2>
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
              <h2 class="font-family-base font-weight-bold mt-1 mb-4">{{ $t("rate-conversation.rate-owner-title") }}</h2>
            </div>
            <hr class="my-0" />
            <!-- TODO : Add profile img to user-profile-snippet props -->
            <user-profile-snippet :id="owner.user.profile.id" sectionWidth="sm" hide-rating class="section--border-bottom py-4" />
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
                $t("btn-rate-conversation")
              }}</b-button>
            </div>
          </div>
        </template>
      </s-form>
    </div>
    <form-complete
      v-else
      :title="$t('form-complete.rate-conversation.title')"
      :description="$t('form-complete.rate-conversation.description')"
      :image="require('@/assets/icons/checklist-yellow.svg')"
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

import conversationService from "@/services/conversation";

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
    rateConversation: async function () {
      this.isSubmitted = true;
      let input = {
        userId: this.raterIsOwner ? this.requester.user.id : this.owner.user.id,
        adId: this.adId,
        conversationId: this.conversationId,
        userRating: this.rating.user
      };
      
      if (!this.raterIsOwner) {
        input.adRating = this.rating.ad;
      }
      
      await conversationService.rateAdAndUser(input);
      window.scrollTo(0, 0);
      this.isSubmitted = false;
    }
  },
  computed: {
    conversationId: function () {
      return this.$route.params.id;
    },
    adId: function () {
      return this.conversation.ad.id;
    },
    adOrganization: function () {
      return this.conversation.ad.organization;
    },
    adTitle: function () {
      return this.conversation.ad.translationOrDefault.title;
    },
    adImage: function () {
      return this.conversation.ad.gallery[0];
    },
    conversationRated: function () {
      return this.raterIsOwner ? this.hasAlreadyRateUser : Boolean(this.conversation.adRating && this.conversation.adRating.length > 0) || this.hasAlreadyRateUser;
    },
    owner: function () {
      return this.conversation.participants.find(p => p.user && p.user.id === this.conversation.ad.user.id);
    },
    requester: function () {
      return this.conversation.participants.find(p => p.user && p.user.id !== this.conversation.ad.user.id);
    },
    raterIsOwner: function () {
      return this.me.id === this.owner.user.id;
    },
    hasAlreadyRateUser: function () {
      return this.conversation.userRatings.some((x) => x.raterUser.id === this.me.id);
    }
  },
  apollo: {
    me: {
      query() {
        return this.$options.query.Me;
      }
    },
    conversation: {
      query() {
        return this.$options.query.ConversationById;
      },
      variables() {
        return {
          id: this.conversationId,
          language: CONTENT_LANG_FR
        };
      },
      result({ data }) {
        if (data) {
          this.currentStep = this.raterIsOwner ? STEP_TENANT : STEP_EQUIPEMENT;
        }
      }
    }
  }
};
</script>

<graphql>
query ConversationById($id: ID!, $language: ContentLanguage!) {
  conversation(id: $id) {
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
    participants {
      id
      user {
        id
        profile {
          id
          ... on UserProfileGraphType {
            publicName
          }
        }
      }
    }
    adRating {
      id
      raterUser {
        id
      }
    }
    userRatings {
      id
      raterUser {
        id
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
