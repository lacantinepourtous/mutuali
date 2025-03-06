<template>
  <s-field
    :id="id"
    :label="label"
    :name="name"
    :inputId="inputId"
    :labelSrOnly="labelSrOnly"
    :rules="rules"
    :description="description"
    v-slot="{ sState }"
  >
    <b-input-group :prepend="prepend" :append="append">
      <slot name="input-group-prepend" :sState="sState"></slot>
      <b-form-input
        :id="inputId"
        v-model="computedValue"
        :type="type"
        :placeholder="placeholder"
        :required="required"
        :state="sState"
        @change="$emit('change', $event)"
      ></b-form-input>
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
      type: [String, Number]
    },
    type: String,
    placeholder: String,
    required: Boolean,
    description: String,
    prepend: String,
    append: String,
    labelSrOnly: Boolean
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
    },
    inputId() {
      return `input-${this.name}`;
    }
  }
};
</script>
