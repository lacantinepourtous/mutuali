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
        <h2 class="h6 mb-1">{{ $t("profile-snippet.verified-member") }}</h2>
        <ul class="user-profile-snippet__member-stats-list list-unstyled small">
          <li>{{ registeredSince }}</li>
          <li>{{ $tc("profile-snippet.ads-count", userAdsCount) }}</li>
          <li v-if="userProfile.user.isConfirmed && userProfile.user.phoneNumberConfirmed">{{ $t("profile-snippet.verified-phone-and-email") }}</li>
          <li v-else-if="userProfile.user.isConfirmed">{{ $t("profile-snippet.verified-email") }}</li>
        </ul>
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

  &__rating {
    position: absolute;
    bottom: $spacer / -2;
    right: $spacer / -2;
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
