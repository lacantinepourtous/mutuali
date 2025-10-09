<template>
  <div v-if="userProfile" class="user-profile-snippet" :class="{ 'user-profile-snippet--big': isHeading }">
    <div class="user-profile-snippet__inside">
      <div class="user-profile-snippet__head mb-4">
        <div class="user-profile-snippet__img-container">
          <img
            class="user-profile-snippet__img"
            alt=""
            :src="require('@/assets/icons/user-mutuali.svg')"
          />
          <b-badge v-if="!hideRating && averageRating" class="user-profile-snippet__rating font-weight-bold" variant="warning"
              ><b-icon-star-fill /> {{ averageRating }}</b-badge
            >
        </div>

        <div>
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

          <p v-if="!hideOrganization" class="m-0" :class="{ 'lead font-weight-normal' : isHeading }">{{ userOrganizationName }}</p>
        </div>
      </div>

      <div class="user-profile-snippet__bottom border-top border-white pt-3 mt-3">
        <div class="user-profile-snippet__member-stats">
          <h2 class="h6 mb-1">{{ $t("profile-snippet.verified-member") }}</h2>
          <ul class="user-profile-snippet__member-stats-list list-unstyled small mb-0">
            <li>{{ registeredSince }}</li>
            <li>{{ $tc("profile-snippet.ads-count", userAdsCount) }}</li>
            <li v-if="userProfile.user.isConfirmed && userProfile.user.phoneNumberConfirmed">{{ $t("profile-snippet.verified-phone-and-email") }}</li>
            <li v-else-if="userProfile.user.isConfirmed">{{ $t("profile-snippet.verified-email") }}</li>
          </ul>
        </div>

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
  }

  &__bottom {
    display: flex;
    flex-direction: column;
    gap: $spacer;
    margin: $spacer 0;
  }

  &--big {
    .user-profile-snippet__bottom {
      @include media-breakpoint-up(sm) {
        display: grid;
        grid-template-columns: 1fr 1fr; 
        gap: $spacer;
      }
    }
  }

  .router-link-exact-active {
    &:hover,
    &:focus,
    &:active {
      text-decoration: none;
      color: $primary;
    }
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
    gap: $spacer * 1.5;
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
