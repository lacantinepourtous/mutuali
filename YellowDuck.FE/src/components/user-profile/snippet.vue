<template>
  <div v-if="userProfile" class="user-profile-snippet" :class="{ 'user-profile-snippet--big': isHeading }">
    <div class="section" :class="`section--${sectionWidth}`">
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
        <div class="user-profile-snippet__content" :class="{ 'user-profile-snippet__content--with-icon': !isHeading }">
          <b-icon-person-fill v-if="!isHeading" class="user-profile-snippet__icon h4"></b-icon-person-fill>
          <component
            :is="titleTag"
            class="m-0"
            :class="isHeading ? 'display-3' : 'line-height-none mt-1 text-green font-weight-bolder responsive-text'"
          >
            <component
              :is="hasLink ? 'RouterLink' : 'span'"
              class="d-inline-flex"
              :class="{ 'btn-link font-weight-bolder': hasLink }"
              :to="hasLink ? { name: $consts.urls.URL_USER_PROFILE_DETAIL, params: { id: id } } : null"
            >
              {{ userPublicName }}</component
            >
          </component>
          <p v-if="!hideOrganization" class="m-0 responsive-text">{{ userOrganizationName }}</p>
          <p v-if="showRegistrationDate" class="m-0 text-primary">
            <small>{{ $t("profile-snipet.member-since", { date: userRegistrationDate }) }}</small>
          </p>
          <p v-if="showPhoneNumber" class="m-0 text-primary">
            <small>{{ userPublicPhoneNumber }}</small>
          </p>
          <p v-if="showEmail" class="m-0 text-primary">
            <small
              ><a :href="`mailto:${userPublicEmail}`">{{ userPublicEmail }}</a></small
            >
          </p>
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
    }
  }
}
</graphql>

<script>
import i18n from "@/helpers/i18n";
export default {
  props: {
    id: {
      type: String,
      required: true
    },
    hideRating: Boolean,
    showRegistrationDate: Boolean,
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

<style lang="scss">
.user-profile-snippet {
  $img-container-width: #{"clamp(50px, 10vw, 70px)"};
  $small-img-container-width: 27px;

  & {
    background-color: $body-bg;
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
    /* align-items: center; */
    display: flex;
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

  &__content {
    display: block;
    flex: 1 1 auto;
    overflow: hidden;
    /* padding-left: $spacer; */
    padding-left: 0;

    &--with-icon {
      position: relative;
      padding-left: calc(#{$spacer} + 18px);
    }
  }

  &__icon {
    position: absolute;
    top: 5px;
    left: -4px;
  }
}
</style>
