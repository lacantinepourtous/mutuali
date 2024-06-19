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
    :margin="margin"
    :className="inline ? 'form-group--inline' : ''"
  >
    <b-input-group :prepend="prepend" :append="append">
      <slot name="input-group-prepend" :sState="sState"></slot>
      <b-form-rating
        :id="`input-${name}`"
        v-model="computedValue"
        color="#f4b42b"
        :class="[{ 'b-rating--small': size === 'sm' }, { 'b-rating--readonly': readonly }]"
        :readonly="readonly"
        no-border
        :size="size"
      ></b-form-rating>
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
$small-star-width: 24px;
$big-star-width: 48px;

.b-rating {
  justify-content: flex-start;
  padding: 0;
  margin-left: -8px;

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
      max-width: $small-star-width;
      height: $small-star-width;
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
  }
}
</style>
