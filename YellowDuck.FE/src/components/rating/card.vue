<template>
  <b-card class="rating-snippet" body-class="rating-snippet__body p-3 px-sm-4">
    <div class="rating-snippet__rater mb-2 mr-sm-4">
      <div class="rating-snippet__img-container">
        <img
          class="rating-snippet__img"
          alt=""
          :src="require('@/assets/icons/user-mutuali.svg')"
        />
      </div>
      <div class="rating-snippet__content">
        <p class="m-0 font-weight-bolder">
          <router-link
            v-if="rating.raterUser"
            :to="{ name: $consts.urls.URL_USER_PROFILE_DETAIL, params: { id: raterUser.profile.id } }"
            >{{ raterUser.profile.publicName }}</router-link
          >
        </p>
        <p class="m-0 text-capitalize text-muted rating-snippet__date">
          <small>{{ reviewDate }}</small>
        </p>
      </div>
    </div>
    <!-- TODO BE : Dynamiser commentaire review -->
    <p class="rating-snippet__comment">C'est une cuisine vraiment très bien aménagée.</p>
    <div class="rating-snippet__rating">
      <s-form-rating
        v-for="criteria in rating.criterias"
        :key="criteria.id"
        class="mb-1"
        :value="convertRatingToInt(rating[criteria.propertyName])"
        readonly
        inline
        size="sm"
        margin="none"
        :label="$t(`label.${criteria.label}`)"
      />
    </div>
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
  border: 2px solid $gray-200;

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
    max-width: 300px;
  }

  &__date {
    line-height: 1;
  }

  &__comment {
    font-style: italic;
    margin: 14px 0;
  }
}
</style>
