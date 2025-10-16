<template>
  <div v-if="ratings.length > 0" class="mb-5">
    <rate :averageRating="averageRating" :ratingsCount="ratings.length" />
    <carousel v-if="ratings.length > 1" class="px-2 py-5 mt-n5">
      <b-carousel-slide v-for="(rating, key) in ratings" :key="key">
        <template #img>
          <rating-card :rating="rating" carousel />
        </template>
      </b-carousel-slide>
    </carousel>
    <rating-card v-else :rating="ratings[0]" carousel />
  </div>
</template>

<script>
import Carousel from "@/components/generic/carousel";
import Rate from "@/components/rating/rate";
import RatingCard from "@/components/rating/card.vue";
import { RatingsCriterias } from "@/mixins/ratings-criterias";

export default {
  mixins: [RatingsCriterias],
  components: {
    Carousel,
    RatingCard,
    Rate
  },
  props: {
    id: {
      type: String,
      required: true
    }
  },
  computed: {
    averageRating: function () {
      return this.ad ? this.ad.averageRating : 0;
    },
    ratings: function () {
      const ratings = this.ad && this.ad.adRatings ? this.ad.adRatings : [];
      const filledRatings = ratings.filter((r) => {
        const hasPositive = this.convertRatingToInt(r.complianceRating) > 0
          || this.convertRatingToInt(r.qualityRating) > 0
          || this.convertRatingToInt(r.overallRating) > 0
          || r.comment;
        return hasPositive;
      });
      return this.getRatingsWithCriterias(filledRatings, ["compliance", "quality", "ad-overall"]);
    }
  },
  apollo: {
    ad: {
      query() {
        return this.$options.query.AdById;
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

<graphql>
query AdById($id: ID!) {
  ad(id: $id) {
    id
    averageRating
    adRatings {
      id
      complianceRating
      qualityRating
      overallRating
      comment
      createdAt
      raterUser {
        id
        profile {
          id
          ... on UserProfileGraphType {
            publicName
          }
        }
      }
    }
  }
}
</graphql>