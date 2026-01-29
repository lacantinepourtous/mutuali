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
      <b-form-textarea
        :id="inputId"
        v-model="computedValue"
        :placeholder="placeholder"
        :required="required"
        :state="sState"
        rows="4"
        @change="$emit('change', $event)"
      ></b-form-textarea>
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
    placeholder: String,
    required: Boolean,
    description: String,
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
