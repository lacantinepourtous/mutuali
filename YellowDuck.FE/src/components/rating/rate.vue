<template>
  <span class="rate line-height-none mt-4 mb-3">
    <b-icon-star-fill class="rate__star mr-2" />
    <span class="rate__average font-weight-bold mr-2 line-height-none">{{ averageRating }}</span>
    <small class="rate__count">({{ $tc("profile-details.rating-count", ratingsCount) }})</small>
  </span>
</template>

<script>
export default {
  props: {
    ratingsCount: Number,
    averageRating: Number
  }
};
</script>

<style lang="scss">
$star-width: 24px;

.rate {
  display: flex;
  align-items: center;

  &__average {
    font-size: 31px;
  }

  &__count {
    margin: auto 0 2px;
    color: $gray-600;
  }

  &__star {
    color: $yellow;
    display: inline-block;
    width: $star-width;
    height: $star-width;
  }
}
</style>
