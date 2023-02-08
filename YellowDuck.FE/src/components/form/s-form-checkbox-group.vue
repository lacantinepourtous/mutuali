<template>
  <s-field
    class="checkbox-group"
    :id="id"
    :name="name"
    :rules="rules"
    :label="label"
    :labelClass="labelClass"
    v-slot="{ sState }"
  >
    <b-form-checkbox-group
      :id="`input-${name}`"
      class="mt-2"
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
    labelClass: String,
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
<style lang="scss">
.checkbox-group {
  .custom-checkbox {
    margin-bottom: 10px;
  }
}
</style>
