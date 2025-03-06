<template>
  <validation-provider
    :name="name"
    :rules="rules"
    v-slot="{ errors, valid, failed, validate }"
    events="['change|blur']"
    :mode="mode"
    slim
    :class="[{ 'my-0': margin === 'none' }, { 'my-3': margin === 'sm' }, { 'my-4': margin === 'md' }]"
  >
    <b-form-group
      :id="id"
      :class="className || ''"
      :label="label"
      :label-for="inputId"
      :label-cols="labelColsWithDefault"
      :label-cols-sm="labelColsSm || labelCols"
      :label-class="labelClass || ''"
      :label-sr-only="labelSrOnly"
      :invalidFeedback="errors[0]"
      :state="valid"
    >
      <template v-if="description && description.trim() != ''" #description>
        <slot name="description"><span class="text-muted" v-html="description"></span></slot>
      </template>
      <slot :sValidate="validate" :sState="getSlotState(failed)" />
    </b-form-group>
  </validation-provider>
</template>
<script>
export default {
  props: {
    id: String,
    inputId: String,
    className: String,
    name: String,
    label: String,
    labelClass: String,
    labelCols: Number,
    labelColsSm: Number,
    description: String,
    labelSrOnly: Boolean,
    margin: {
      type: String,
      default: "md"
    },
    rules: {
      type: [String, Object]
    },
    mode: {
      default: "eager"
    }
  },
  computed: {
    labelColsWithDefault() {
      if (this.label) return this.labelCols || 12;
      return null;
    }
  },
  methods: {
    getSlotState(failed) {
      return failed ? false : null;
    }
  }
};
</script>
<style lang="scss">
.form-group {
  &--inline {
    .form-row {
      align-items: center;
      flex-wrap: nowrap;
    }

    .col-form-label {
      line-height: 1.1;
      overflow: hidden;
      text-overflow: ellipsis;
    }
  }
}
</style>
