<template>
  <div class="type-card">
    <div class="type-card__icon">
      <slot></slot>
    </div>
    <div class="type-card__bottom">
      <div class="type-card__title">{{ title }}</div>
      <div v-if="price || modality || description" class="type-card__text">
        <span v-if="price" class="type-card__price" :class="{ 'type-card__price--sm': priceToBeDetermined }">{{ price }}</span>
        <span v-if="modality" class="type-card__modality">{{ modality }}</span>
        <span v-if="description" class="type-card__desc">{{ description }}</span>
        <span v-if="footnote" class="type-card__footnote">{{ footnote }}</span>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    title: {
      type: String,
      required: true
    },
    price: {
      type: String,
      default: ""
    },
    priceToBeDetermined: {
      type: Boolean,
      default: false
    },
    modality: {
      type: String,
      default: ""
    },
    description: {
      type: String,
      default: ""
    },
    footnote: {
      type: String,
      default: ""
    }
  }
};
</script>

<style lang="scss">
.type-card {
  & {
    padding: $spacer;
    border: solid 2px $gray-300;
    border-radius: 12px;
    display: flex;
    column-gap: $spacer;

    @include media-breakpoint-up(lg) {
      flex-direction: column;
      height: 100%;
    }
  }

  &__bottom {
    @include media-breakpoint-up(lg) {
      margin-top: auto;
    }
  }

  &__title {
    font-size: 12px;
    line-height: 1.2;
    text-transform: uppercase;
    color: $gray-800;
    font-weight: 700;
  }

  &__text {
    font-size: 14px;
    color: $gray-700;
    line-height: 1.2;
    margin-top: 4px;
  }

  &__price {
    white-space: nowrap;
    font-size: 20px;
    font-weight: 700;
    margin-right: 4px;
    display: inline-block;

    &--sm {
      font-size: 14px;
      color: $gray-700;
      font-weight: 400;
    }
  }

  &__footnote {
    font-size: 12px;
    color: $gray-700;
    display: block;
    margin-top: 4px;
  }

  &__icon {
    @include media-breakpoint-up(lg) {
      margin-bottom: $spacer;
    }
  }
}
</style>
