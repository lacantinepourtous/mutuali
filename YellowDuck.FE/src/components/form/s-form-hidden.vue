<template>
  <s-field :id="id" :name="name" :inputId="inputId" :rules="rules" v-slot="{ sState }">
    <input type="hidde" :value="value" :id="id" :name="name" :required="required" :state="sState" />
  </s-field>
</template>

<script>
import SField from "@/components/form/s-field";

export default {
  props: {
    id: String,
    name: String,
    rules: {
      type: [String, Object]
    },
    value: {
      type: [String, Number]
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
