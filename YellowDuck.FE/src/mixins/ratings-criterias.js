import { RATING_LABELS, RATING } from "@/consts/rating";

export const RatingsCriterias = {
  methods: {
    getRatingsWithCriterias: function(ratings, criteriasGroup) {
      let criterias = [];
      for (const criteria of criteriasGroup) {
        criterias.push({ ...RATING_LABELS.find((x) => x.id === criteria) });
      }

      let ratingsWithCriterias = [];
      for (const rating of ratings) {
        const ratingWithCriterias = {
          criterias,
          ...rating
        };
        ratingsWithCriterias.push(ratingWithCriterias);
      }
      return ratingsWithCriterias;
    },
    convertRatingToInt(rating) {
      return RATING.indexOf(rating);
    }
  }
};