<template>
  <s-field :id="id" :name="name" :inputId="inputId" :rules="rules" v-slot="{ sState }">
    <b-form-checkbox :id="inputId" v-model="computedValue" :required="required && value.length < 1" :state="sState">
      <span v-html="label" />
    </b-form-checkbox>
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
      type: [String, Boolean]
    },
    required: Boolean
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
