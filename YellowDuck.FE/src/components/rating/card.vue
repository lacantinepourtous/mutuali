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
      <div v-if="isAdmin" class="rating-snippet__actions ml-auto">
        <b-button
          variant="danger"
          size="sm"
          @click="confirmDeleteRating"
          :disabled="isDeleting"
        >
          <b-icon icon="trash" class="mr-1" aria-hidden="true"></b-icon>
          {{ $t("btn.delete") }}
        </b-button>
      </div>
    </div>
    <p class="rating-snippet__comment">{{ rating.comment }}</p>
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
import { RatingsCriterias } from "@/mixins/ratings-criterias";
import i18n from "@/helpers/i18n";
import { deleteRating } from "@/services/rating";
import NotificationService from "@/services/notification";
import AuthentificationService from "@/services/authentification";
import { USER_TYPE_ADMIN } from "@/consts/enums";

export default {
  components: { SFormRating },
  mixins: [RatingsCriterias],
  props: {
    rating: {
      type: Object,
      required: true
    },
    carousel: Boolean
  },
  data() {
    return {
      isDeleting: false
    };
  },
  computed: {
    raterUser: function() {
      return this.rating.raterUser;
    },
    reviewDate: function() {
      return i18n.getLocalizedMonthYear(this.rating.createdAt);
    },
    isAdmin: function() {
      return this.user && AuthentificationService.getUserType() === USER_TYPE_ADMIN;
    }
  },
  methods: {
    async confirmDeleteRating() {
      const confirmed = await this.$bvModal.msgBoxConfirm(
        this.$t("text.confirm-delete-rating"),
        {
          title: this.$t("title.confirm-delete"),
          okVariant: "danger",
          okTitle: this.$t("btn.delete"),
          cancelTitle: this.$t("btn.cancel"),
          centered: true
        }
      );

      if (confirmed) {
        await this.deleteRating();
      }
    },

    async deleteRating() {
      if (this.isDeleting) return;
      this.isDeleting = true;

      try {
        const input = this.getRatingInput(this.rating);
        if (!input) throw new Error("Type de rating inconnu");

        const result = await deleteRating(input);

        if (result && result.data && result.data.deleteRating && result.data.deleteRating.success) {
          NotificationService.showSuccess(this.$t("notification.rating-deleted"));
          this.$emit("rating-deleted", this.rating.id);
        } else {
          throw new Error("Suppression échouée");
        }
      } catch (error) {
        NotificationService.showError(this.$t("error.unexpected"));
      } finally {
        this.isDeleting = false;
      }
    },

    getRatingInput(rating) {
      if ("complianceRating" in rating || "qualityRating" in rating) {
        return { adRatingId: rating.id };
      }
      if ("respectRating" in rating || "communicationRating" in rating) {
        return { userRatingId: rating.id };
      }
      return null;
    }
  },
  apollo: {
    user: {
      query() {
        return this.$options.query.LocalUser;
      }
    }
  }
};
</script>

<graphql>
query LocalUser {
  user @client {
    isConnected
  }
}
</graphql>

<style lang="scss">
.rating-snippet {
  $img-container-width: 52px;
  border: 2px solid $gray-200;

  &__rater {
    align-items: center;
    display: flex;
  }

  &__actions {
    display: flex;
    align-items: center;
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
