<template>
  <s-field :id="id" :name="name" :rules="rules" v-slot="{ sState }">
    <span v-html="label" />
    <b-form-checkbox-group
      :id="`input-${name}`"
      v-model="computedValue"
      :required="required"
      :options="options"
      :state="sState"
      stacked
    >
    </b-form-checkbox-group>
  </s-field>
</template>

<script>
import SField from "@/components/form/s-field";

export default {
  props: {
    id: String,
    label: String,
    name: String,
    options: {
      type: Array,
      default() {
        return null;
      }
    },
    rules: {
      type: [String, Object]
    },
    value: {
      type: [Array, Boolean]
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
    }
  }
};
</script>
