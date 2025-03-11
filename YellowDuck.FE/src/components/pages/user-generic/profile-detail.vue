<template>
  <div v-if="userProfile" class="w-100 mt-3 mt-md-5">
    <user-profile-snippet
      :id="userProfile.id"
      :hasLink="false"
      isHeading
      titleTag="h1"
      showContactInfo
      sectionWidth="sm"
      class="my-4"
    />
    <div class="section section--sm mt-3 mt-md-4 mb-5">
      <b-container>
        <b-row>
          <!--b-col>
            <b-card class="border-0 rounded-0 text-center" body-class="py-0 pl-0">
              <b-card-title class="display-4 mb-0 line-height-none">{{ transactionsCount }}</b-card-title>
              <b-card-text class="text-muted responsive-text">
                <small>{{ $tc("profile-detail.transactions-count", transactionsCount) }}</small>
              </b-card-text>
            </b-card>
          </b-col-->
          <b-col>
            <b-card class="border-0 rounded-0" body-class="py-0 px-0">
              <b-card-text>
                <p class="display-4 mb-1 font-weight-bolder text-green-dark">{{ registeredSince }}</p>
                <small class="text-muted responsive-text">{{ $t("profile-detail.registered-since") }}</small>
              </b-card-text>
              <b-card-text v-if="!isCurrentUser">
                <a
                  class="user__report"
                  :href="`mailto:${contactUsEmail}?subject=${$t('email.report-user-subject')}&body=${$t(
                    'email.report-user-body',
                    {
                      url: urlForReport
                    }
                  )}`"
                  >{{ $t("btn.report-user-ad") }}</a
                >
              </b-card-text>
            </b-card>
          </b-col>
        </b-row>
      </b-container>
    </div>

    <b-tabs
      nav-class="profile-tabs__nav-item section section--padding-x section--sm"
      active-nav-item-class="profile-tabs__nav-item--active"
    >
      <b-tab active>
        <template #title>
          <span class="profile-tabs__nav-item-title">{{ $t("profile-detail.ads") }}</span>
          <b-badge class="ml-1 font-weight-bold"> {{ publishedAds.length }} </b-badge>
        </template>
        <div class="section section--sm mt-3 mt-md-4">
          <div class="profile-tabs__title">
            <h2 class="my-0 mr-4 mr-md-6 font-family-base font-weight-bold">{{ $t("title.ad-publish") }}</h2>

            <b-button v-if="isCurrentUser" :to="{ name: $consts.urls.URL_MANAGE_ADS }" variant="outline-primary" size="md">
              {{ $t("btn.manage-my-ads") }}
            </b-button>
          </div>
        </div>

        <div v-if="publishedAds.length > 0" class="mb-5 mt-3">
          <ad-snippet
            v-for="ad in publishedAds"
            :key="ad.id"
            :id="ad.id"
            :title="ad.translationOrDefault.title"
            titleTag="h3"
            :image="ad.gallery[0]"
            :price-details="getPriceDetailsFromAd(ad)"
            :organization="ad.organization"
            sectionWidth="sm"
            smallTitle
            snippetIsLink
          />
        </div>
        <ad-no-content v-else class="my-5 py-sm-4" />
      </b-tab>
      <!-- Disable for Pilote version -->
      <!--b-tab :active="publishedAds.length == 0">
        <template #title>
          <span class="profile-tabs__nav-item-title">{{ $t("profile-detail.reviews") }}</span>
          <b-badge class="ml-1 font-weight-bold"> {{ ratings.length }} </b-badge>
        </template>
        <div class="section section--sm mb-5">
          <template v-if="ratings.length > 0">
            <rate :averageRating="averageRating" :ratingsCount="ratings.length" />
            <rating-card v-for="(rating, key) in ratings" :key="key" :rating="rating" class="mb-3" />
          </template>
          <div v-else class="no-review">
            <img class="no-review__img my-5" alt="" :src="require('@/assets/ambiance/flying-star.svg')" />
            <p>{{ $t("profile-detail.no-reviews") }}</p>
          </div>
        </div>
      </b-tab-->
    </b-tabs>
  </div>
</template>

<script>
/* Disable for Pilote version */
/*import Rate from "@/components/rating/rate";*/
/*import RatingCard from "@/components/rating/card.vue";*/

import dayjs from "dayjs";

import UserProfileSnippet from "@/components/user-profile/snippet";
import AdSnippet from "@/components/ad/snippet.vue";
import AdNoContent from "@/components/ad/no-content.vue";
import { RatingsCriterias } from "@/mixins/ratings-criterias";
import { PriceDetails } from "@/mixins/price-details";

import { CONTENT_LANG_FR } from "@/consts/langs";
import { URL_USER_PROFILE_DETAIL } from "@/consts/urls";
import { VUE_APP_MUTUALI_CONTACT_MAIL } from "@/helpers/env";

export default {
  mixins: [RatingsCriterias, PriceDetails],
  components: {
    /* Disable for Pilote version */
    /*RatingCard*/
    /*Rate,*/
    UserProfileSnippet,
    AdNoContent,
    AdSnippet
  },
  computed: {
    contactUsEmail: function () {
      return VUE_APP_MUTUALI_CONTACT_MAIL;
    },
    profileId: function () {
      return this.$route.params.id.split("-").last();
    },
    averageRating: function () {
      return this.userProfile.user ? String(this.userProfile.user.averageRating) : "-";
    },
    publishedAds: function () {
      return this.userProfile.user ? this.userProfile.user.ads.filter((x) => x.isPublish) : [];
    },
    ratings: function () {
      const ratings = this.userProfile.user ? this.userProfile.user.userRatings : [];
      return this.getRatingsWithCriterias(ratings, ["respect", "fiability", "communication"]);
    },
    registeredSince: function () {
      return this.userProfile.user ? this.fromNow(this.userProfile.user.registrationDate) : this.fromNow(new Date().toString());
    },
    transactionsCount: function () {
      return this.userProfile.user ? String(this.userProfile.user.transactionsCount) : "-";
    },
    isCurrentUser: function () {
      return this.me ? this.profileId === this.me.profile.id : false;
    },
    urlForReport: function () {
      return encodeURI(
        `${window.location.protocol}//${window.location.hostname}${
          this.$router.resolve({
            name: URL_USER_PROFILE_DETAIL,
            params: { id: this.profileId }
          }).href
        }`
      );
    }
  },
  methods: {
    fromNow(date) {
      const now = dayjs();
      const diff = now.diff(date, "M");
      if (diff < 12) return this.$tc("from-now.month", diff);
      return this.$tc("from-now.year", Math.floor(diff / 12));
    }
  },
  apollo: {
    me: {
      query() {
        return this.$options.query.Me;
      }
    },
    userProfile: {
      query() {
        return this.$options.query.UserProfileById;
      },
      variables() {
        return {
          id: this.profileId,
          language: CONTENT_LANG_FR
        };
      },
      skip() {
        return !this.me;
      }
    }
  }
};
</script>
<graphql>
query Me {
  me {
    id
    profile {
      id
    }
  }
}
query UserProfileById($id: ID!, $language: ContentLanguage!) {
  userProfile(id: $id) {
    id
    user {
      registrationDate
      ads {
        id
        isPublish
        isAvailableForRent
        isAvailableForSale
        isAvailableForTrade
        isAvailableForDonation
        gallery {
          id
          src
          alt
        }
        rentPrice
        rentPriceToBeDetermined
        salePrice
        salePriceToBeDetermined
        translationOrDefault(language: $language) {
          id
          title
          rentPriceDescription
          salePriceDescription
          tradeDescription
          donationDescription
        }
        organization
      }
      averageRating
      transactionsCount
      userRatings {
        id
        raterUser {
          id
          profile {
            id
            ... on UserProfileGraphType {
              publicName
            }
          }
        }
        respectRating
        fiabilityRating
        communicationRating
        createdAt
      }
    }
  }
}
</graphql>

<style lang="scss">
$no-review-img-width: 170px;

.profile-tabs {
  &__nav {
    padding-left: $spacer;

    @include media-breakpoint-up(md) {
      padding-left: $spacer * 2;
    }
  }

  &__title {
    display: flex;
    align-items: center;
  }

  &__nav-item {
    &-title {
      color: $text-muted;
    }

    .badge {
      background-color: $gray-400;
      color: $gray-900;
    }

    &--active {
      cursor: default;

      .profile-tabs__nav-item-title {
        color: $gray-900;
      }

      .badge {
        background-color: $yellow;
        color: $gray-900;
      }
    }
  }
}

.no-review {
  align-items: center;
  display: flex;
  flex-direction: column;
  justify-content: center;

  &__img {
    width: $no-review-img-width;
  }
}

.user__report {
  background: url("~@/assets/icons/flag-red.svg") no-repeat 0 0;
  background-size: 18px auto;
  padding: 0 $spacer 0 calc(#{$spacer} + 8px);
  color: #dc3545;
  &:hover,
  &:focus,
  &:active {
    color: #dc3545;
    text-decoration: underline;
  }
}
</style>
