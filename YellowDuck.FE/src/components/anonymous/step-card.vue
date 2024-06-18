<template>
  <div class="step-card text-center">
    <div class="step-card__circle">
      <span class="step-card__number font-weight-bolder text-white">{{ counter }}</span>
      <img class="step-card__img" :class="{ 'step-card__img--alt-position': imgAltPos }" :src="imgSrc" alt="" />
    </div>
    <p class="h4 step-card__text">
      <slot></slot>
    </p>
  </div>
</template>

<script>
export default {
  props: {
    counter: {
      type: Number,
      default: 1
    },
    imgSrc: {
      type: String,
      default: ""
    },
    imgAltPos: Boolean
  }
};
</script>

<style lang="scss" scoped>
$circle-width: 100px;
$card-width: 400px;
.step-card {
  & {
    max-width: $card-width;
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: $spacer $spacer * 3 $spacer * 1.5;
    border-radius: 12px;
  }

  &--green {
    background-color: $green;

    .step-card__number {
      -webkit-text-stroke: 3px $green;
    }

    .step-card__text {
      color: white;
    }
  }

  &--yellow {
    background-color: $yellow;

    .step-card__number {
      -webkit-text-stroke: 3px $yellow;
    }
  }

  &--red {
    background-color: $red;

    .step-card__number {
      -webkit-text-stroke: 3px $red;
    }

    .step-card__text {
      color: white;
    }
  }

  &__circle {
    position: relative;
    width: $circle-width;
    height: $circle-width;
    background-color: white;
    border-radius: 50%;
    margin: $spacer * 2 0;
  }

  &__number {
    position: absolute;
    top: -24px;
    left: -20px;
    font-size: 96px;
    font-family: $font-family-brand;
    line-height: 1;
  }

  &__img {
    position: absolute;
    top: 34px;
    left: 34px;

    &--alt-position {
      top: 24px;
      left: 24px;
    }
  }
}
</style>
