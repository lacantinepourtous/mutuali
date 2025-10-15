<template>
  <div class="w-100" v-if="conversation">
    <portal v-if="!isSubmitted" :to="$consts.enums.PORTAL_HEADER">
      <nav-close :to="{ name: $consts.urls.URL_CONVERSATION_DETAIL, params: { id: this.conversationId } }"></nav-close>
    </portal>

    <div v-if="!isSubmitted">
      <div class="section section--sm">
        <h1 class="mt-4 mb-3">{{ isEditMode ? $t("page-title.edit-rating") : $t("page-title.rate-conversation") }}</h1>
        <p>{{ isEditMode ? $t("edit-rating.text") : $t("rate-conversation.text") }}</p>
      </div>

      <s-form @submit="rateConversation">
        <template v-if="raterIsOwner">
          <div v-show="currentStep === $consts.ratingSteps.STEP_TENANT" class="rating-step">
            <div class="section section--sm mt-4">
              <h2 class="font-family-base font-weight-bold mb-4">{{ $t("rate-conversation.rate-tennant-title") }}</h2>
            </div>
            <hr class="my-0" />
            <div class="section section--sm pt-4 pb-3">
              <user-profile-snippet :id="requester.user.profile.id" sectionWidth="sm" />
            </div>
            <hr class="my-0" />
            <div class="section section--sm mb-4">
              <div class="mt-4 mb-5">
                <s-form-rating 
                  v-model="rating.user.respect" 
                  :label="$t('label.user-respect-rating')" 
                  :description="$t('description.user-respect-rating')"
                  size="lg" 
                  margin="sm" />
                <s-form-rating
                  v-model="rating.user.communication"
                  :label="$t('label.user-communication-rating')"
                  :description="$t('description.user-communication-rating')"
                  size="lg"
                  margin="sm"
                />
                <s-form-rating 
                  v-model="rating.user.overall" 
                  :label="$t('label.user-overall-experience-rating')" 
                  :description="$t('description.user-overall-experience-rating')"
                  size="lg" 
                  margin="sm" />
              </div>

              <div class="comment-section py-2">
                <s-form-textarea 
                  v-model="userComment" 
                  :label="$t('label.additional-comment')"
                  margin="sm" 
                />
              </div>

              <b-button :disabled="isSubmitted" type="submit" variant="primary" size="lg" block>{{
                isEditMode ? $t("btn-update-rating") : $t("btn-rate-conversation")
              }}</b-button>
            </div>
          </div>
        </template>

        <template v-else>
          <div v-show="currentStep === $consts.ratingSteps.STEP_EQUIPEMENT" class="rating-step">
            <div class="section section--sm mt-4">
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
                <s-form-rating 
                  v-model="rating.ad.compliance" 
                  :label="$t('label.ad-conformity-rating')"
                  :description="$t('description.ad-conformity-rating')" 
                  size="lg" 
                  margin="sm" 
                />
                <s-form-rating 
                  v-model="rating.ad.quality" 
                  :label="isResource ? $t('label.ad-quality-rating-resource') : $t('label.ad-quality-rating-material')"
                  :description="isResource ? $t('description.ad-quality-rating-resource') : $t('description.ad-quality-rating-material')" 
                  size="lg" 
                  margin="sm" />
                <s-form-rating 
                  v-model="rating.ad.overall"
                  :label="$t('label.ad-overall-experience-rating')"
                  :description="$t('description.ad-overall-experience-rating')"
                  size="lg" 
                  margin="sm" />
              </div>

              <div class="comment-section py-2">
                <s-form-textarea 
                  v-model="adComment" 
                  :label="$t('label.additional-comment')"
                  margin="sm"
                />
              </div>

              <b-button @click="currentStep = $consts.ratingSteps.STEP_OWNER" variant="admin" size="lg" block>
                {{ $t("btn-rate-next-step") }}
              </b-button>
              <b-button @click="resetAdRating()" variant="outline-primary" size="lg" block>
                <b-icon-arrow-counterclockwise />
                {{ $t("btn-rate-reset") }}
              </b-button>
            </div>
          </div>

          <div v-show="currentStep === $consts.ratingSteps.STEP_OWNER" class="rating-step">
            <div class="section section--sm mt-4">
              <h2 class="font-family-base font-weight-bold mt-1 mb-4">{{ $t("rate-conversation.rate-owner-title") }}</h2>
            </div>
            <hr class="my-0" />
            <!-- TODO : Add profile img to user-profile-snippet props -->
            <div class="section section--sm pt-4 pb-3">
              <user-profile-snippet :id="owner.user.profile.id" sectionWidth="sm" hide-rating />
            </div>
            <hr class="my-0" />
            <div class="section section--sm mb-4">
              <div class="mt-4 mb-5">
                <s-form-rating 
                  v-model="rating.user.respect" 
                  :label="$t('label.user-respect-rating')" 
                  :description="$t('description.user-respect-rating')"
                  size="lg" 
                  margin="sm" />
                <s-form-rating
                  v-model="rating.user.communication"
                  :label="$t('label.user-communication-rating')"
                  :description="$t('description.user-communication-rating')"
                  size="lg"
                  margin="sm"
                />
                <s-form-rating 
                  v-model="rating.user.overall" 
                  :label="$t('label.user-overall-experience-rating')" 
                  :description="$t('description.user-overall-experience-rating')"
                  size="lg" 
                  margin="sm" />
              </div>
              
              <div class="comment-section py-2">
                <s-form-textarea 
                  v-model="userComment" 
                  :label="$t('label.additional-comment')" 
                  margin="sm"
                />
              </div>
              
              <b-button :disabled="isSubmitted" type="submit" variant="admin" size="lg" block>
                {{ isEditMode ? $t("btn-update-rating") : $t("btn-rate-conversation") }}
              </b-button>
              <div class="d-flex flex-wrap flex-sm-nowrap justify-content-between mt-2 btns-wrapper">
                <b-button @click="currentStep = $consts.ratingSteps.STEP_EQUIPEMENT" variant="outline-primary"  size="lg" block>
                  <b-icon-arrow-left />
                  {{ $t("btn-rate-previous-step") }}
                </b-button>
                <b-button @click="resetUserRating()" class="mt-0" variant="outline-primary" size="lg" block>
                  <b-icon-arrow-counterclockwise />
                  {{ $t("btn-rate-reset") }}
                </b-button>
              </div>
              
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
import SFormTextarea from "@/components/form/s-form-textarea";
import FormComplete from "@/components/generic/form-complete";

import { CONTENT_LANG_FR } from "@/consts/langs";
import { CATEGORY_HUMAN_RESOURCES, CATEGORY_SUBCONTRACTING } from "@/consts/categories";
import { URL_CONVERSATION_DETAIL, URL_LIST_AD } from "@/consts/urls";
import { STEP_EQUIPEMENT, STEP_TENANT } from "@/consts/rating-steps";
import { RatingsCriterias } from "@/mixins/ratings-criterias";

import conversationService from "@/services/conversation";

export default {
  components: {
    FormComplete,
    SForm,
    SFormRating,
    SFormTextarea,
    NavClose,
    AdSnippet,
    UserProfileSnippet
  },
  mixins: [RatingsCriterias],
  data() {
    return {
      CATEGORY_HUMAN_RESOURCES,
      CATEGORY_SUBCONTRACTING,
      currentStep: "",
      isEditMode: false,
      targetUserProfile: null,
      existingUserRating: null,
      existingAdRating: null,
      rating: {
        ad: {
          compliance: 0,
          quality: 0,
          overall: 0
        },
        user: {
          respect: 0,
          communication: 0,
          overall: 0
        }
      },
      userComment: "",
      adComment: "",
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
    resetUserRating() {
      this.rating.user = {
        respect: 0,
        communication: 0,
        overall: 0
      };
    },
    resetAdRating() {
      this.rating.ad = {
        compliance: 0,
        quality: 0,
        overall: 0
      };
    },
    loadExistingAdRatingData: function() {
      if (!this.conversation || !this.me) {
        return;
      }

      if (!this.raterIsOwner) {
        this.existingAdRating = this.conversation.adRating.find((r) => r && r.raterUser && r.raterUser.id === this.me.id);
        if (this.existingAdRating) {
          this.rating.ad = {
            compliance: this.convertRatingToInt(this.existingAdRating.complianceRating) || 0,
            quality: this.convertRatingToInt(this.existingAdRating.qualityRating) || 0,
            overall: this.convertRatingToInt(this.existingAdRating.overallRating) || 0,
          };
          this.adComment = this.existingAdRating.comment || "";
          this.isEditMode = true;
        }
      }
    },
    loadExistingUserRatingData: function() {
      if (!this.me || !this.targetUserProfile || !this.targetUserProfile.user) {
        return;
      }

      this.existingUserRating = this.targetUserProfile.user.userRatings.find((r) => r && r.raterUser && r.raterUser.id === this.me.id);

      if (this.existingUserRating) {
        this.rating.user = {
          respect: this.convertRatingToInt(this.existingUserRating.respectRating) || 0,
          communication: this.convertRatingToInt(this.existingUserRating.communicationRating) || 0,
          overall: this.convertRatingToInt(this.existingUserRating.overallRating) || 0,
        };
        this.userComment = this.existingUserRating.comment || "";
        this.isEditMode = true;
      }
    },
    rateConversation: async function () {
      this.isSubmitted = true;
      let input = {
        userId: this.raterIsOwner ? this.requester.user.id : this.owner.user.id,
        adId: this.adId,
        conversationId: this.conversationId,
        userRating: this.rating.user,
        userComment: this.userComment,
        adComment: this.adComment
      };
      
      if (!this.raterIsOwner) {
        input.adRating = this.rating.ad;
      }
      
      if (this.isEditMode) {
        await conversationService.updateAdAndUserRating(input);
      } 
      else {
        await conversationService.rateAdAndUser(input);
      }
      window.scrollTo(0, 0);
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
    owner: function () {
      return this.conversation.participants.find(p => p.user && p.user.id === this.conversation.ad.user.id);
    },
    requester: function () {
      return this.conversation.participants.find(p => p.user && p.user.id !== this.conversation.ad.user.id);
    },
    raterIsOwner: function () {
      return this.me.id === this.owner.user.id;
    },
    isResource: function () {
      return this.conversation.ad.category === CATEGORY_HUMAN_RESOURCES || this.conversation.ad.category === CATEGORY_SUBCONTRACTING;
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
          this.loadExistingAdRatingData();
        }
      }
    },
    targetUserProfile: {
      query() {
        return this.$options.query.UserProfileByIdRatings;
      },
      variables() {
        return {
          id: this.raterIsOwner ? this.requester.user.profile.id : this.owner.user.profile.id
        };
      },
      update: function(data) {
        return data && data.userProfile ? data.userProfile : null;
      },
      skip() {
        return !this.me || !this.conversation;
      },
      result({ data }) {
        if (data) {
          this.loadExistingUserRatingData();
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
      category
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
      complianceRating
      qualityRating
      overallRating
      comment
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

query UserProfileByIdRatings($id: ID!) {
  userProfile(id: $id) {
    id
    user {
      id
      userRatings {
        id
        respectRating
        communicationRating
        overallRating
        comment
        raterUser {
          id
        }
      }
    }
  }
}
</graphql>

<style lang="scss" scoped>
.comment-section {
  border-top: 1px solid $gray-200;
}

.btns-wrapper {
  gap: $spacer / 2;
}
</style>