<template>
  <s-field :id="id" :label="label" :name="name" :rules="rules" :description="description" v-slot="{ sState }">
    <b-input-group :prepend="prepend" :append="append">
      <slot name="input-group-prepend" :sState="sState"></slot>
      <b-form-datepicker
        :id="`input-${name}`"
        v-model="computedValue"
        :placeholder="placeholder"
        :required="required"
        :state="sState"
        :locale="locale"
        :initial-date="initialDate"
        :min="initialDate"
        :label-current-month="$t('label.form-datepicker.current-month')"
        :label-help="$t('label.form-datepicker.help')"
        :label-next-month="$t('label.form-datepicker.next-month')"
        :label-next-year="$t('label.form-datepicker.next-year')"
        :label-prev-month="$t('label.form-datepicker.prev-month')"
        :label-prev-year="$t('label.form-datepicker.prev-year')"
        :label-no-date-selected="$t('placeholder.ad-startDate')"
      ></b-form-datepicker>
      <slot name="input-group-append" :sState="sState"></slot>
    </b-input-group>
  </s-field>
</template>

<script>
import SField from "@/components/form/s-field";

import i18nHelpers from "@/helpers/i18n";

export default {
  props: {
    id: String,
    label: String,
    name: String,
    rules: {
      type: [String, Object]
    },
    value: {
      type: [String, Date]
    },
    placeholder: String,
    required: Boolean,
    description: String,
    prepend: String,
    append: String,
    initialDate: {
      type: [String, Date]
    }
  },
  components: {
    SField
  },
  computed: {
    locale: function () {
      return i18nHelpers.locale() === "fr" ? "fr-CA" : "en-CA";
    },
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