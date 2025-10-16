<template>
  <s-field
    :id="id"
    :label="label"
    :name="name"
    :rules="rules"
    :description="description"
    v-slot="{ sState }"
    :inline="inline"
    :label-cols="inline ? 6 : 12"
    :label-cols-sm="inline ? 6 : 12"
    :label-class="{ 'rating-label-large': size !== 'sm' }"
    :margin="margin"
    :className="inline ? 'form-group--inline' : ''"
  >
    <b-input-group :prepend="prepend" :append="append">
      <slot name="input-group-prepend" :sState="sState"></slot>
      <b-form-rating
        v-if="!readonly || computedValue > 0"
        :id="`input-${name}`"
        v-model="computedValue"
        color="#f4b42b"
        :class="[{ 'b-rating--small': size === 'sm' }, { 'b-rating--readonly': readonly }]"
        :readonly="readonly"
        no-border
        :size="size"
      ></b-form-rating>
      <span v-else class="text-muted font-italic">{{ $t("label.not-rated") }}</span>
      <slot name="input-group-append" :sState="sState"></slot>
    </b-input-group>
  </s-field>
</template>

<script>
import SField from "@/components/form/s-field";

export default {
  props: {
    id: String,
    label: String,
    name: String,
    rules: {
      type: [String, Object]
    },
    value: {
      type: [Number]
    },
    color: String,
    readonly: Boolean,
    inline: Boolean,
    description: String,
    prepend: String,
    append: String,
    size: String,
    margin: {
      type: String,
      default: "md"
    }
  },
  components: {
    SField
  },
  computed: {
    computedValue: {
      get() {
        return this.value;
      },
      set(val) {
        this.$emit("input", val);
      }
    }
  }
};
</script>
<style lang="scss">
$xsmall-star-width: 16px;
$small-star-width: 24px;
$big-star-width: 40px;

.rating-label-large {
  font-size: 1.25rem;
  font-weight: 600;
}

.b-rating {
  justify-content: flex-start;
  padding: 0;
  margin-left: -8px;
  margin-bottom: 8px !important;

  .b-rating-star {
    flex-grow: 0 !important;
    padding: 0 6px;
  }

  &-icon svg {
    width: $big-star-width;
    height: $big-star-width;
  }

  &--small {
    .b-rating-icon svg {
      max-width: $xsmall-star-width;
      height: $xsmall-star-width;

      @include media-breakpoint-up(sm) {
        max-width: $small-star-width;
        height: $small-star-width;
      }
    }

    .b-rating-star {
      flex-shrink: 1;
      padding: 0 2px;
    }
  }

  &--readonly {
    .b-rating-star-empty {
      display: none;
    }
  }
}

.form-group--inline {
  .b-rating {
    margin-left: 0;
    margin-bottom: 0 !important;
  }
}
</style>
