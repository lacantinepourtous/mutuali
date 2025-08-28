<template>
  <div v-if="userProfile" class="user-profile-snippet" :class="{ 'user-profile-snippet--big': isHeading }">
    <div class="user-profile-snippet__inside">
      <!-- <router-link :to="{ name: $consts.urls.URL_USER_PROFILE_DETAIL, params: { id: id } }">
        <div class="user-profile-snippet__img-container">
          <img
            v-if="profilePicture"
            class="user-profile-snippet__img"
            alt=""
            :src="`${profilePicture}?mode=crop&width=200&height=200`"
          />
          <b-icon-person-circle v-else class="user-profile-snippet__img"></b-icon-person-circle>
          <b-badge v-if="!hideRating && averageRating" class="user-profile-snippet__rating font-weight-bold" variant="warning"
            ><b-icon-star-fill class="mr-1" /> {{ averageRating }}</b-badge
          >
        </div>
      </router-link> -->
      <div class="user-profile-snippet__head">
        <svg v-if="!isHeading" class="user-profile-snippet__icon" xmlns="http://www.w3.org/2000/svg" width="31" height="31" viewBox="0 0 31 31" fill="none" aria-hidden="true">
          <path d="M10.4468 8.46328C10.4468 11.2533 12.71 13.5166 15.5001 13.5166C18.2901 13.5166 20.5534 11.2533 20.5534 8.46328C20.5534 5.67322 18.2901 3.37866 15.5001 3.37866C12.71 3.37866 10.4468 5.64191 10.4468 8.46328Z" fill="#414042"/>
          <path d="M15.5 14.5083C10.85 14.5083 7.06836 18.29 7.06836 22.94V25.6367C9.02161 26.8767 12.1517 27.6202 15.5 27.6202C18.8484 27.6202 21.9784 26.8767 23.9317 25.6367V22.94C23.9317 18.29 20.15 14.5083 15.5 14.5083Z" fill="#414042"/>
        </svg>

        <div class="user-profile-snippet__rating-container">
          <b-icon-person-fill v-if="!isHeading" class="user-profile-snippet__icon h4"></b-icon-person-fill>
          <b-badge v-if="!hideRating && averageRating" class="user-profile-snippet__rating font-weight-bold" variant="warning"
              ><b-icon-star-fill class="mr-1" /> {{ averageRating }}</b-badge
            >
        </div>

        <component
          :is="titleTag"
          class="m-0"
          :class="isHeading ? 'display-3' : 'line-height-none font-weight-bold lead'"
        >
          <component
            :is="hasLink ? 'RouterLink' : 'span'"
            class="d-inline-flex"
            :class="{ 'btn-link font-weight-bolder': hasLink }"
            :to="hasLink ? { name: $consts.urls.URL_USER_PROFILE_DETAIL, params: { id: id } } : null"
          >
            {{ userPublicName }}
          </component>
        </component>
      </div>

      <p v-if="!hideOrganization" class="m-0 responsive-text">{{ userOrganizationName }}</p>

      <p v-if="showPhoneNumber" class="m-0 text-primary">
        <small>{{ userPublicPhoneNumber }}</small>
      </p>
      <p v-if="showEmail" class="m-0 text-primary">
        <small><a :href="`mailto:${userPublicEmail}`">{{ userPublicEmail }}</a></small>
      </p>

      <div class="user-profile-snippet__member-stats border-top border-white pt-3 mt-3">
        <div class="d-flex align-items-start justify-content-between">
          <svg class="flex-shrink-0 mr-2" :class="isLoyalMember ? 'text-warning' : 'text-secondary'" xmlns="http://www.w3.org/2000/svg" width="32" height="37" viewBox="0 0 32 37" fill="currentColor" aria-hidden="true">
            <path d="M21.689 29.9343C21.3493 29.7977 20.9612 29.9381 20.7869 30.2609L19.4738 32.5973C19.3185 32.8691 19.0195 33.0248 18.7078 32.9968C18.396 32.9674 18.1301 32.7594 18.0283 32.4634L16.2266 27.2367C17.5003 27.0376 18.6404 26.3307 19.386 25.2767C19.4153 25.2346 19.4623 25.2103 19.5132 25.2103H19.5616C20.862 25.6531 22.2884 25.5051 23.4704 24.8058L25.2492 29.9636C25.351 30.2609 25.2696 30.5901 25.0418 30.8058C24.8128 31.0214 24.4807 31.0827 24.1906 30.964L21.689 29.9343ZM12.9589 32.4621L14.7607 27.2354V27.2367C13.487 27.0376 12.3469 26.3307 11.6013 25.2767C11.572 25.2346 11.5237 25.2104 11.474 25.2104H11.4244C10.1253 25.6531 8.69764 25.5051 7.51557 24.8058L5.75073 29.9636C5.65021 30.2609 5.73164 30.5889 5.95813 30.8032C6.18462 31.0189 6.5167 31.0814 6.80557 30.964L9.31094 29.9342C9.65068 29.7977 10.0388 29.9381 10.2131 30.2609L11.5415 32.6101H11.5427C11.7031 32.8717 11.9982 33.0184 12.3036 32.9865C12.6077 32.9546 12.8673 32.7517 12.9716 32.4633L12.9589 32.4621ZM4.48979 14.4572C5.13618 13.5843 5.13618 12.39 4.48979 11.5172C4.01134 10.8753 3.87395 10.0421 4.11952 9.28024C4.36637 8.51846 4.96569 7.92382 5.72785 7.68519C6.76486 7.36108 7.46595 6.39257 7.45451 5.30409C7.44688 4.50529 7.82605 3.75242 8.47117 3.28411C9.11629 2.8158 9.94845 2.69075 10.7017 2.94852C10.9549 3.03402 11.2196 3.07868 11.4868 3.07868C12.2846 3.07996 13.0315 2.69204 13.4934 2.03998C13.954 1.38792 14.7022 1 15.5 1C16.2978 1 17.046 1.38792 17.5066 2.03998C17.9685 2.69204 18.7154 3.07995 19.5132 3.07868C19.7804 3.07868 20.0451 3.03402 20.2983 2.94852C21.0554 2.68693 21.8914 2.81071 22.5403 3.28156C23.188 3.75242 23.5671 4.51038 23.5557 5.31304C23.5468 6.39639 24.2428 7.35853 25.2722 7.68519C26.0318 7.92381 26.6298 8.51718 26.8754 9.2777C27.1222 10.0369 26.9848 10.8689 26.5102 11.5108C25.8638 12.3836 25.8638 13.5779 26.5102 14.4508C26.9887 15.0913 27.1261 15.9259 26.8805 16.6877C26.6336 17.4495 26.0343 18.0441 25.2722 18.2827C24.2351 18.6068 23.5341 19.5741 23.5455 20.6638C23.5582 21.4665 23.179 22.2244 22.5301 22.6953C21.8812 23.1662 21.0452 23.2912 20.2881 23.0284C19.2587 22.6813 18.125 23.0513 17.4965 23.9407C17.0358 24.5928 16.2877 24.9807 15.4899 24.9807C14.6933 24.9807 13.9451 24.5928 13.4832 23.9407C12.8559 23.0513 11.7209 22.6813 10.6916 23.0284C9.93578 23.2912 9.09854 23.1662 8.45088 22.6953C7.80194 22.2245 7.42279 21.4665 7.43422 20.6638C7.44949 19.5805 6.75731 18.6133 5.72791 18.2827C4.96829 18.0428 4.37024 17.4495 4.12467 16.6902C3.87782 15.9297 4.01518 15.0977 4.48979 14.4572Z" />
          </svg>
          <div class="flex-grow-1">
            <h2 class="h6 mb-1">{{ isLoyalMember ? $t("profile-snippet.loyal-member") : $t("profile-snippet.verified-member") }}</h2>
            <ul class="user-profile-snippet__member-stats-list list-unstyled small">
              <li>{{ registeredSince }}</li>
              <li>{{ $tc("profile-snippet.ads-count", userAdsCount) }}</li>
              <li v-if="userProfile.user.isConfirmed && userProfile.user.phoneNumberConfirmed">{{ $t("profile-snippet.verified-phone-and-email") }}</li>
              <li v-else-if="userProfile.user.isConfirmed">{{ $t("profile-snippet.verified-email") }}</li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<graphql>
query UserProfileById($id: ID!) {
  userProfile(id: $id) {
    id
    firstName
    lastName
    organizationName
    publicName
    showPhoneNumber
    showEmail
    publicPhoneNumber
    publicEmail
    user {
      averageRating
      registrationDate
      isConfirmed
      phoneNumberConfirmed
      ads {
        id
        isPublish
      }
    }
  }
}
</graphql>

<script>
import i18n from "@/helpers/i18n";
import dayjs from "dayjs";

export default {
  props: {
    id: {
      type: String,
      required: true
    },
    hideRating: Boolean,
    hideOrganization: Boolean,
    profilePicture: String,
    sectionWidth: {
      type: String,
      default: "md"
    },
    isHeading: Boolean,
    titleTag: {
      type: String,
      default: "h2"
    },
    showContactInfo: Boolean,
    showUnderline: {
      type: Boolean,
      default: true
    },
    hasLink: {
      type: Boolean,
      default: true
    }
  },
  computed: {
    userPublicName: function () {
      return this.userProfile.publicName;
    },
    userOrganizationName: function () {
      return this.userProfile.organizationName;
    },
    averageRating: function () {
      return this.userProfile.user.averageRating;
    },
    userRegistrationDate: function () {
      return i18n.getLocalizedMonthYear(this.userProfile.user.registrationDate);
    },
    showPhoneNumber: function () {
      return this.showContactInfo && this.userProfile.showPhoneNumber && this.userPublicPhoneNumber;
    },
    showEmail: function () {
      return this.showContactInfo && this.userProfile && this.userProfile.showEmail && this.userPublicEmail;
    },
    userPublicPhoneNumber: function () {
      return this.userProfile.publicPhoneNumber;
    },
    userPublicEmail: function () {
      return this.userProfile.publicEmail;
    },
    isLoyalMember: function () {
      if (!this.userProfile.user.registrationDate) return false;
      const regDate = new Date(this.userProfile.user.registrationDate);
      const now = new Date();
      const diffMs = now - regDate;
      const diffDays = diffMs / (1000 * 60 * 60 * 24);
      return diffDays > 360;
    },
    registeredSince: function () {
      return this.userProfile.user ? this.fromNow(this.userProfile.user.registrationDate) : this.fromNow(new Date().toString());
    },
    userAdsCount: function () {
      return this.userProfile.user.ads.filter(ad => ad.isPublish).length;
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
    userProfile: {
      query() {
        return this.$options.query.UserProfileById;
      },
      variables() {
        return {
          id: this.id
        };
      }
    }
  }
};
</script>

<style lang="scss" scoped>
.user-profile-snippet {
  $img-container-width: #{"clamp(50px, 10vw, 70px)"};
  $small-img-container-width: 27px;

  & {
    display: block;

    /* &--big {
      $img-container-width: #{"clamp(60px, 10vw, 80px)"};
      .user-profile-snippet__content {
        padding-left: $spacer * 1.5;
      }
    } */
  }

  .router-link-exact-active {
    &:hover,
    &:focus,
    &:active {
      text-decoration: none;
      color: $primary;
    }
  }

  &__inside {
  }

  &__img-container {
    align-items: center;
    display: flex;
    flex: 0 0 auto;
    width: $img-container-width;
    min-height: $img-container-width;
    position: relative;

    &--small {
      margin-top: 5px;
      width: $small-img-container-width;
      min-height: $small-img-container-width;
    }
  }

  &__img {
    display: block;
    width: 100%;
    height: 100%;
    border-radius: 50%;
    object-fit: cover;
    object-position: center;
  }

  &__rating-container {
    position: relative;
    flex: 0 0 auto;
    width: 48px; // Largeur augmentée pour le SVG
    height: 48px; // Hauteur augmentée pour le SVG
    overflow: visible; // Permet au badge de déborder
  }

  &__icon {
    position: absolute;
    top: 0;
    left: 0;
    width: 48px;
    height: 48px;
  }

  &__rating {
    position: absolute;
    bottom: -5px;
    right: -5px;
    z-index: 1;
  }

  &__head {
    display: flex;
    align-items: center;
    flex: 1 1 auto;
  }

  &__icon {
    margin-right: 4px;
  }

  &__member-stats-list {
    li {
      &:before {
        content: "✔";
        display: inline-block;
        margin-right: 0.5em;
        position: relative;
        top: -0.1em;
      }
    }
  }
}
</style>
