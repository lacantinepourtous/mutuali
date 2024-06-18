<template>
  <s-field
    :id="id"
    :label="label"
    :labelClass="labelClass"
    :name="name"
    :inputId="inputId"
    :rules="rules"
    :description="description"
    v-slot="{ sState }"
  >
    <b-form-select
      :id="inputId"
      v-model="computedValue"
      :options="computedOptions"
      :required="required"
      :state="sState"
    ></b-form-select>
  </s-field>
</template>

<script>
import SField from "@/components/form/s-field";

export default {
  props: {
    id: String,
    label: String,
    labelClass: {
      type: String,
      default: "label"
    },
    name: String,
    rules: {
      type: [String, Object]
    },
    options: Array,
    addDisableOptions: Boolean,
    value: String,
    placeholder: String,
    required: Boolean,
    description: String
  },
  components: {
    SField
  },
  computed: {
    computedOptions: function () {
      let options = [...this.options];
      if (this.addDisableOptions || this.placeholder) {
        options.unshift({
          value: "",
          disabled: true,
          text: this.placeholder ? this.placeholder : this.$t("select.disabled-option")
        });
      }

      return options;
    },
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

<style lang="scss">
.custom-select {
  &:invalid {
    color: $text-muted;
  }

  option {
    color: $gray-900;
  }
}
</style>
