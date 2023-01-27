<template>
  <b-card :body-class="['rating-snippet', carousel ? 'px-4 pt-3 pb-5' : 'p-3 p-sm-4']">
    <b-container>
      <b-row>
        <b-col cols="12" sm="5">
          <div class="rating-snippet__rater mb-2 mr-sm-4">
            <div class="rating-snippet__img-container">
              <!-- TODO : Add profile img to user-profile-snippet props -->
              <img
                v-if="profilePicture"
                class="rating-snippet__img"
                alt=""
                :src="`${profilePicture}?mode=crop&width=200&height=200`"
              />
              <b-icon-person-circle v-else class="rating-snippet__img"></b-icon-person-circle>
            </div>
            <div class="rating-snippet__content">
              <p class="m-0 font-weight-bolder">
                <router-link
                  v-if="rating.raterUser"
                  :to="{ name: $consts.urls.URL_USER_PROFILE_DETAIL, params: { id: raterUser.profile.id } }"
                  >{{ raterUser.profile.publicName }}</router-link
                >
              </p>
              <p class="m-0 text-capitalize text-muted">
                <small>{{ reviewDate }}</small>
              </p>
            </div>
          </div>
        </b-col>
        <b-col cols="12" sm="7">
          <div class="rating-snippet__rating">
            <s-form-rating
              v-for="criteria in rating.criterias"
              :key="criteria.id"
              :value="convertRatingToInt(rating[criteria.propertyName])"
              readonly
              inline
              size="sm"
              margin="none"
              :label="$t(`label.${criteria.label}`)"
            />
          </div>
        </b-col>
      </b-row>
    </b-container>
  </b-card>
</template>

<script>
import SFormRating from "@/components/form/s-form-rating";
import { RATING } from "@/consts/rating";
import i18n from "@/helpers/i18n";

export default {
  components: { SFormRating },
  props: {
    rating: {
      type: Object,
      required: true
    },
    profilePicture: String,
    carousel: Boolean
  },
  computed: {
    raterUser: function() {
      return this.rating.raterUser;
    },
    reviewDate: function() {
      return i18n.getLocalizedMonthYear(this.rating.createdAt);
    }
  },
  methods: {
    convertRatingToInt(rating) {
      return RATING.indexOf(rating);
    }
  }
};
</script>

<style lang="scss">
.rating-snippet {
  $img-container-width: 52px;

  &__rater {
    align-items: center;
    display: flex;
  }

  &__img-container {
    align-items: center;
    display: flex;
    flex: 0 0 auto;
    width: $img-container-width;
    min-height: $img-container-width;
    position: relative;
    margin-right: $spacer;
  }

  &__img {
    display: block;
    width: 100%;
    height: 100%;
    border-radius: 50%;
    object-fit: cover;
    object-position: center;
  }

  &__content {
    display: block;
    flex: 1 1 auto;
    overflow: hidden;
  }

  &__rating {
    @include media-breakpoint-up(sm) {
      flex-shrink: 0;
    }
  }
}
</style>
